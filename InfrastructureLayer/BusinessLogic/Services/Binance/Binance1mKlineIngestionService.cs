using ApplicationLayer.Interfaces;
using ApplicationLayer.Interfaces.Services;
using DomainLayer.Entities;
using InfrastructureLayer.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace InfrastructureLayer.BusinessLogic.Services.Binance;

public class Binance1mKlineIngestionService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<Binance1mKlineIngestionService> _logger;
    private const string BinanceWsBaseUrl = "wss://stream.binance.com:9443/stream?streams=";
    private const int ReconnectDelaySeconds = 5;
    
    // Cache for Symbol -> ID mapping
    private readonly ConcurrentDictionary<string, int> _symbolIdMap = new(StringComparer.OrdinalIgnoreCase);

    public Binance1mKlineIngestionService(
        IServiceScopeFactory scopeFactory,
        ILogger<Binance1mKlineIngestionService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting Binance 1m Kline Ingestion Service...");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // 1. Load Symbols & ID Map
                var symbols = await LoadActiveSymbolsAsync(stoppingToken);
                if (symbols.Count == 0)
                {
                    _logger.LogWarning("No active symbols found. Waiting...");
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                    continue;
                }

                // 2. Backfill Missing Data (Critical Requirement)
                await BackfillMissingDataAsync(symbols, stoppingToken);

                // 3. Connect & Stream (Real-time Requirement)
                await RunWebSocketLoopAsync(symbols, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Critical error in ingestion service. Restarting loop in {Delay}s", ReconnectDelaySeconds);
                await Task.Delay(TimeSpan.FromSeconds(ReconnectDelaySeconds), stoppingToken);
            }
        }

        _logger.LogInformation("Binance 1m Kline Ingestion Service stopped.");
    }

    private async Task<List<Cryptocurrency>> LoadActiveSymbolsAsync(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IRepository<Cryptocurrency>>();
        var list = await repo.GetDbSet()
            .AsNoTracking()
            .Where(c => c.IsActive)
            .ToListAsync(ct);

        _symbolIdMap.Clear();
        foreach (var item in list)
        {
            _symbolIdMap[item.Symbol] = item.Id;
        }
        return list;
    }

    private async Task BackfillMissingDataAsync(List<Cryptocurrency> symbols, CancellationToken ct)
    {
        _logger.LogInformation("Starting Gap Analysis & Backfill for {Count} symbols...", symbols.Count);
        
        using var scope = _scopeFactory.CreateScope();
        var candleRepo = scope.ServiceProvider.GetRequiredService<IRepository<Candle_1m>>();

        foreach (var crypto in symbols)
        {
            if (ct.IsCancellationRequested) break;

            try
            {
                // Find last candle time
                var lastCandleTime = await candleRepo.GetDbSet()
                    .Where(c => c.CryptocurrencyId == crypto.Id)
                    .OrderByDescending(c => c.OpenTime)
                    .Select(c => c.OpenTime)
                    .FirstOrDefaultAsync(ct);

                // If no data, fetch last 1000 candles (Bootstrap)
                // If data exists, check if gap > 1 minute
                
                var now = DateTime.UtcNow;
                var cutoff = now.AddMinutes(-1); // Allow 1 min latency

                if (lastCandleTime == default)
                {
                    _logger.LogInformation("Bootstrapping history for {Symbol}...", crypto.Symbol);
                    await FetchAndInsertKlinesAsync(crypto, limit: 1000, startTime: null, ct);
                }
                else if (lastCandleTime < cutoff)
                {
                    var missingMinutes = (now - lastCandleTime).TotalMinutes;
                    if (missingMinutes > 1.5) // Grace buffer
                    {
                        _logger.LogInformation("Gap detected for {Symbol}: Last={Last}, Missing={Min}m. Backfilling...", 
                            crypto.Symbol, lastCandleTime, (int)missingMinutes);

                        // Fetch from Last + 1m
                        var startTime = new DateTimeOffset(lastCandleTime.AddMinutes(1)).ToUnixTimeMilliseconds();
                        await FetchAndInsertKlinesAsync(crypto, limit: 1000, startTime: startTime, ct);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error backfilling {Symbol}", crypto.Symbol);
            }
        }
    }

    private async Task FetchAndInsertKlinesAsync(Cryptocurrency crypto, int limit, long? startTime, CancellationToken ct)
    {
        // Loop until up to date if gap is large? 
        // For simplicity, we fetch up to 1000. If more needed, subsequent runs or loop here can handle it.
        // User requirement: "It MUST backfill the missing candles" -> implied full backfill.
        // I'll implement a loop for large gaps.
        
        long? currentStartTime = startTime;
        
        using var scope = _scopeFactory.CreateScope();
        var binanceRestService = scope.ServiceProvider.GetRequiredService<IBinanceKlineService>();

        while (true)
        {
            var klines = await binanceRestService.GetKlinesAsync(crypto.Symbol, limit, currentStartTime);
            if (klines == null || klines.Count == 0) break;

            await BatchInsertCandlesAsync(crypto.Id, klines, ct);
            
            if (klines.Count < limit) break; // Reached end
            
            // Next batch starts after last candle
            currentStartTime = klines.Last().CloseTime + 1;
            
            // Safety break if we are caught up
            if (currentStartTime > DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()) break;
        }
    }

    private async Task BatchInsertCandlesAsync(int cryptoId, List<BinanceKlineDto> klines, CancellationToken ct)
    {
        if (klines.Count == 0) return;

        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Use bulk insert or individual upserts. 
        // For backfill of finalized candles, simple INSERT OR IGNORE / MERGE is best.
        // I will use the same MERGE logic for consistency.
        
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            using var transaction = await dbContext.Database.BeginTransactionAsync(ct);
            try
            {
                foreach (var k in klines)
                {
                    await ExecuteUpsertAsync(dbContext, cryptoId, k, isFinal: true, ct);
                }
                await transaction.CommitAsync(ct);
            }
            catch
            {
                await transaction.RollbackAsync(ct);
                throw;
            }
        });
        
        _logger.LogInformation("Backfilled {Count} candles for ID {Id}", klines.Count, cryptoId);
    }

    private async Task RunWebSocketLoopAsync(List<Cryptocurrency> symbols, CancellationToken ct)
    {
        // Chunk symbols if too many (Binance URL length limit ~4096 chars)
        // A safe chunk size is ~50-100 symbols.
        var chunks = symbols.Chunk(50).ToList();
        
        var tasks = new List<Task>();
        foreach (var chunk in chunks)
        {
            tasks.Add(ConnectAndConsumeChunkAsync(chunk, ct));
        }

        await Task.WhenAll(tasks);
    }

    private async Task ConnectAndConsumeChunkAsync(Cryptocurrency[] symbols, CancellationToken ct)
    {
        var streams = string.Join('/', symbols.Select(s => $"{s.Symbol.ToLowerInvariant()}@kline_1m"));
        var uri = new Uri(BinanceWsBaseUrl + streams);

        while (!ct.IsCancellationRequested)
        {
            using var ws = new ClientWebSocket();
            ws.Options.KeepAliveInterval = TimeSpan.FromMinutes(2); // Keep alive

            try
            {
                _logger.LogInformation("Connecting WS for {Count} symbols...", symbols.Length);
                await ws.ConnectAsync(uri, ct);
                _logger.LogInformation("WS Connected.");

                var buffer = new byte[1024 * 128]; // 128kb buffer
                var ms = new MemoryStream();

                while (ws.State == WebSocketState.Open && !ct.IsCancellationRequested)
                {
                    ms.SetLength(0);
                    WebSocketReceiveResult result;
                    do
                    {
                        result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), ct);
                        if (result.MessageType == WebSocketMessageType.Close)
                        {
                            await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Server closed", ct);
                            break;
                        }
                        ms.Write(buffer, 0, result.Count);
                    }
                    while (!result.EndOfMessage);

                    if (ws.State != WebSocketState.Open) break;

                    var json = Encoding.UTF8.GetString(ms.ToArray());
                    await ProcessMessageAsync(json, ct);
                }
            }
            catch (Exception ex) when (!ct.IsCancellationRequested)
            {
                _logger.LogError(ex, "WS Error. Reconnecting in {Delay}s...", ReconnectDelaySeconds);
            }
            
            if (!ct.IsCancellationRequested)
                await Task.Delay(TimeSpan.FromSeconds(ReconnectDelaySeconds), ct);
        }
    }

    private async Task ProcessMessageAsync(string json, CancellationToken ct)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            
            // Format: {"stream":"...","data":{...}}
            if (!root.TryGetProperty("data", out var data)) return;
            if (!data.TryGetProperty("k", out var k)) return;
            if (!data.TryGetProperty("s", out var s)) return;

            var symbol = s.GetString();
            if (string.IsNullOrEmpty(symbol) || !_symbolIdMap.TryGetValue(symbol, out var cryptoId)) return;

            // Extract fields
            var klineDto = new BinanceKlineDto
            {
                OpenTime = k.GetProperty("t").GetInt64(),
                CloseTime = k.GetProperty("T").GetInt64(),
                OpenPrice = decimal.Parse(k.GetProperty("o").GetString()!),
                HighPrice = decimal.Parse(k.GetProperty("h").GetString()!),
                LowPrice = decimal.Parse(k.GetProperty("l").GetString()!),
                ClosePrice = decimal.Parse(k.GetProperty("c").GetString()!),
                Volume = decimal.Parse(k.GetProperty("v").GetString()!),
                NumberOfTrades = (int)k.GetProperty("n").GetInt64()
            };
            var isFinal = k.GetProperty("x").GetBoolean();

            // Upsert
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await ExecuteUpsertAsync(dbContext, cryptoId, klineDto, isFinal, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing WS message");
        }
    }

    private async Task ExecuteUpsertAsync(ApplicationDbContext db, int cryptoId, BinanceKlineDto k, bool isFinal, CancellationToken ct)
    {
        // SQL MERGE (Upsert)
        // Ensure atomic update for forming candle
        var openTime = DateTimeOffset.FromUnixTimeMilliseconds(k.OpenTime).UtcDateTime;
        var closeTime = DateTimeOffset.FromUnixTimeMilliseconds(k.CloseTime).UtcDateTime;
        var lastUpdated = DateTime.UtcNow;

        var sql = @"
            MERGE INTO [dbo].[Candle_1m] AS target
            USING (SELECT {0} AS CryptocurrencyId, {1} AS OpenTime) AS source
            ON (target.CryptocurrencyId = source.CryptocurrencyId AND target.OpenTime = source.OpenTime)
            WHEN MATCHED THEN
                UPDATE SET 
                    [CloseTime] = {2},
                    [Open] = {3},
                    [High] = {4},
                    [Low] = {5},
                    [Close] = {6},
                    [Volume] = {7},
                    [NumberOfTrades] = {8},
                    [IsFinal] = {9},
                    [LastUpdatedUtc] = {10}
            WHEN NOT MATCHED THEN
                INSERT ([CryptocurrencyId], [OpenTime], [CloseTime], [Open], [High], [Low], [Close], [Volume], [NumberOfTrades], [IsFinal], [LastUpdatedUtc], [IsActive], [IsDeleted], [CreatedAt])
                VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, 1, 0, {10});
        ";

        await db.Database.ExecuteSqlRawAsync(sql, 
            cryptoId, 
            openTime, 
            closeTime, 
            k.OpenPrice, 
            k.HighPrice, 
            k.LowPrice, 
            k.ClosePrice, 
            k.Volume, 
            k.NumberOfTrades, 
            isFinal, 
            lastUpdated);
    }
}
