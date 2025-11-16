using ApplicationLayer.Interfaces;
using ApplicationLayer.Interfaces.Services;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace InfrastructureLayer.BusinessLogic.Services.Binance;

[InjectAsScoped]
public class KlineWebSocketService(
    IUnitOfWork uow,
    IRepository<Candle_1m> c1m,
    IRepository<Candle_5m> c5m,
    IRepository<Candle_1h> c1h,
    IRepository<Candle_4h> c4h,
    IRepository<Candle_1d> c1d,
    ILogger<KlineWebSocketService> logger,
    IConfiguration configuration) : IKlineWebSocketService
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = false
    };

    public async Task RunStreamsForSymbolAsync(Cryptocurrency cryptocurrency, CancellationToken cancellationToken)
    {
        var symbol = cryptocurrency.Symbol.ToLowerInvariant();
        var baseUrl = configuration["BinanceWebSocket:BaseUrl"] ?? "wss://stream.binance.com:9443/stream?streams=";
        var reconnectDelaySeconds = int.TryParse(configuration["BinanceWebSocket:ReconnectDelaySeconds"], out var rds) ? rds : 5;

        var intervals = new[] { "1m", "5m", "1h", "4h", "1d" };
        var streamsPath = string.Join('/', intervals.Select(i => $"{symbol}@kline_{i}"));
        var uri = new Uri(baseUrl + streamsPath);

        while (!cancellationToken.IsCancellationRequested)
        {
            using var ws = new ClientWebSocket();
            ws.Options.KeepAliveInterval = TimeSpan.FromSeconds(30);
            try
            {
                logger.LogInformation("Connecting Binance WS for {Symbol} -> {Streams}", cryptocurrency.Symbol, streamsPath);
                await ws.ConnectAsync(uri, cancellationToken);
                logger.LogInformation("Connected WS for {Symbol}", cryptocurrency.Symbol);

                await ReceiveLoopAsync(ws, cryptocurrency, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // graceful shutdown
                break;
            }
            catch (WebSocketException ex)
            {
                logger.LogWarning(ex, "WebSocket error for {Symbol}, will reconnect in {Delay}s", cryptocurrency.Symbol, reconnectDelaySeconds);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error in WS for {Symbol}, will reconnect in {Delay}s", cryptocurrency.Symbol, reconnectDelaySeconds);
            }
            finally
            {
                try
                {
                    if (ws.State == WebSocketState.Open || ws.State == WebSocketState.CloseReceived)
                    {
                        await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "closing", CancellationToken.None);
                    }
                }
                catch { /* ignore */ }
            }

            // Backoff before reconnect
            await Task.Delay(TimeSpan.FromSeconds(reconnectDelaySeconds), cancellationToken);
        }
    }

    private async Task ReceiveLoopAsync(ClientWebSocket ws, Cryptocurrency crypto, CancellationToken ct)
    {
        var buffer = new byte[32 * 1024];
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
                    logger.LogInformation("WS closed by server for {Symbol}", crypto.Symbol);
                    return;
                }
                ms.Write(buffer, 0, result.Count);
            }
            while (!result.EndOfMessage);

            var json = Encoding.UTF8.GetString(ms.ToArray());

            try
            {
                var msg = JsonSerializer.Deserialize<BinanceCombinedStream>(json, _jsonOptions);
                if (msg?.Data?.Kline == null)
                    continue;

                var k = msg.Data.Kline;
                if (!k.IsFinal)
                    continue; // only final candles

                await PersistFinalCandleAsync(crypto.Id, k, ct);
            }
            catch (JsonException jex)
            {
                logger.LogWarning(jex, "Failed to parse WS message for {Symbol}: {Snippet}", crypto.Symbol, json.Length > 200 ? json[..200] + "..." : json);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error handling WS message for {Symbol}", crypto.Symbol);
            }
        }
    }

    private async Task PersistFinalCandleAsync(int cryptocurrencyId, BinanceKline k, CancellationToken ct)
    {
        var parseCulture = System.Globalization.CultureInfo.InvariantCulture;

        var interval = k.Interval?.ToLowerInvariant();

        switch (interval)
        {
            case "1m":
                await UpsertAsync(c1m, cryptocurrencyId, k, ct);
                break;

            case "5m":
                await UpsertAsync(c5m, cryptocurrencyId, k, ct);
                break;

            case "1h":
                await UpsertAsync(c1h, cryptocurrencyId, k, ct);
                break;

            case "4h":
                await UpsertAsync(c4h, cryptocurrencyId, k, ct);
                break;

            case "1d":
                await UpsertAsync(c1d, cryptocurrencyId, k, ct);
                break;

            default:
                logger.LogWarning("Unknown interval {Interval} for cryptoId={Id}", interval, cryptocurrencyId);
                break;
        }
    }

    private async Task UpsertAsync<TCandle>(IRepository<TCandle> repo, int cryptoId, BinanceKline k, CancellationToken ct)
        where TCandle : CandleBase, new()
    {
        // Use k.OpenStart (t) as OpenTime and k.CloseEnd (T) as CloseTime
        var openTimeUtc = DateTimeOffset.FromUnixTimeMilliseconds(k.OpenStart).UtcDateTime;
        var closeTimeUtc = DateTimeOffset.FromUnixTimeMilliseconds(k.CloseEnd).UtcDateTime;

        // Avoid duplicates
        var exists = await repo.GetDbSet()
            .AsNoTracking()
            .AnyAsync(c => c.CryptocurrencyId == cryptoId && c.OpenTime == openTimeUtc, ct);
        if (exists)
            return;

        var culture = System.Globalization.CultureInfo.InvariantCulture;
        var entity = new TCandle
        {
            CryptocurrencyId = cryptoId,
            OpenTime = openTimeUtc,
            CloseTime = closeTimeUtc,
            Open = decimal.Parse(k.Open, culture),
            High = decimal.Parse(k.High, culture),
            Low = decimal.Parse(k.Low, culture),
            Close = decimal.Parse(k.Close, culture),
            Volume = decimal.Parse(k.Volume ?? "0", culture),
            NumberOfTrades = (int)k.NumberOfTrades
        };

        await repo.AddAsync(entity);
        await uow.SaveChangesAsync(ct);
        logger.LogInformation("Inserted {Interval} candle for cryptoId={Id} at {OpenTime}", k.Interval, cryptoId, openTimeUtc);
    }

    private record BinanceCombinedStream
    (
        [property: JsonPropertyName("stream")] string Stream,
        [property: JsonPropertyName("data")] BinanceData Data
    );

    private record BinanceData
    (
        [property: JsonPropertyName("e")] string EventType,
        [property: JsonPropertyName("E")] long? EventTime,
        [property: JsonPropertyName("s")] string Symbol,
        [property: JsonPropertyName("k")] BinanceKline Kline
    );

    private record BinanceKline
    (
        [property: JsonPropertyName("t")] long OpenStart,
        [property: JsonPropertyName("T")] long CloseEnd,
        [property: JsonPropertyName("s")] string Symbol,
        [property: JsonPropertyName("i")] string Interval,
        [property: JsonPropertyName("o")] string Open,
        [property: JsonPropertyName("h")] string High,
        [property: JsonPropertyName("l")] string Low,
        [property: JsonPropertyName("c")] string Close,
        [property: JsonPropertyName("v")] string Volume,
        [property: JsonPropertyName("n")] long NumberOfTrades,
        [property: JsonPropertyName("x")] bool IsFinal,
        [property: JsonPropertyName("V")] string TakerBuyBaseVolume,
        [property: JsonPropertyName("q")] string QuoteVolume,
        [property: JsonPropertyName("Q")] string TakerBuyQuoteVolume,
        [property: JsonPropertyName("B")] object Ignore
    );
}