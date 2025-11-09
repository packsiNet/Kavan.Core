using ApplicationLayer.Interfaces;
using ApplicationLayer.Interfaces.Services;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace InfrastructureLayer.BusinessLogic.Services.Binance;

[InjectAsScoped]
public class CandleManagementService : ICandleManagementService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBinanceKlineService _binanceKlineService;
    private readonly IRepository<Candle_1m> _repository;
    private readonly ILogger<CandleManagementService> _logger;
    private readonly IConfiguration _configuration;

    public CandleManagementService(
        IUnitOfWork unitOfWork,
        IBinanceKlineService binanceKlineService,
        IRepository<Candle_1m> repository,
        ILogger<CandleManagementService> logger,
        IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _binanceKlineService = binanceKlineService;
        _repository = repository;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task ProcessCandlesForSymbolAsync(Cryptocurrency cryptocurrency)
    {
        try
        {
            _logger.LogInformation("Processing candles for symbol: {Symbol}", cryptocurrency.Symbol);

            // دریافت 5 کندل از Binance (ایندکس‌های 0-4)
            var klines = await _binanceKlineService.GetKlinesAsync(cryptocurrency.Symbol, limit: 5);

            if (klines == null || klines.Count == 0)
            {
                _logger.LogWarning("No klines received for symbol: {Symbol}", cryptocurrency.Symbol);
                return;
            }

            // فقط کندل کامل با ایندکس 3 را درج می‌کنیم (ایندکس 4 در حال تشکیل است)
            if (klines.Count < 4)
            {
                _logger.LogWarning("Insufficient klines received for symbol: {Symbol}", cryptocurrency.Symbol);
                return;
            }

            var targetKline = klines[3];
            var targetOpenTimeUtc = DateTimeOffset.FromUnixTimeMilliseconds(targetKline.OpenTime).UtcDateTime;

            var exists = await _repository.Query()
                .Where(c => c.CryptocurrencyId == cryptocurrency.Id && c.OpenTime == targetOpenTimeUtc)
                .AsNoTracking()
                .AnyAsync();

            if (exists)
            {
                _logger.LogInformation("1m candle already exists for {Symbol} at {OpenTime}", cryptocurrency.Symbol, targetOpenTimeUtc);
                return;
            }

            await InsertCandlesAsync(cryptocurrency.Id, new List<BinanceKlineDto> { targetKline });
            _logger.LogInformation("Inserted 1m candle (index=3) for {Symbol} at {OpenTime}", cryptocurrency.Symbol, targetOpenTimeUtc);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing candles for symbol: {Symbol}", cryptocurrency.Symbol);
            throw;
        }
    }

    public async Task FetchMissingCandlesAsync(int cryptocurrencyId, string symbol, long fromCloseTime, long toOpenTime)
    {
        try
        {
            _logger.LogInformation("Fetching missing candles for symbol: {Symbol} from {From} to {To}",
                symbol, fromCloseTime, toOpenTime);

            // محاسبه تعداد کندل‌های از دست رفته (هر کندل 1 دقیقه = 60000 میلی‌ثانیه)
            var timeDiff = toOpenTime - fromCloseTime - 1;
            var expectedCandles = (int)(timeDiff / 60000);

            // محدودسازی بک‌فیل به‌صورت تنظیم‌پذیر
            var maxBackfillMinutes = _configuration.GetValue<int>("CandleFetcher:MaxBackfillMinutes", 0);
            if (maxBackfillMinutes <= 0)
            {
                _logger.LogInformation("Backfill disabled (MaxBackfillMinutes={MaxBackfillMinutes}) for symbol: {Symbol}", maxBackfillMinutes, symbol);
                return;
            }
            expectedCandles = Math.Min(expectedCandles, maxBackfillMinutes);

            if (expectedCandles <= 0)
            {
                _logger.LogWarning("No missing candles detected for symbol: {Symbol}", symbol);
                return;
            }

            _logger.LogInformation("Expected {Count} missing candles for symbol: {Symbol}", expectedCandles, symbol);

            // Binance حداکثر 1000 کندل در هر درخواست برمی‌گرداند
            var limit = Math.Min(expectedCandles, 1000);
            var missingKlines = await _binanceKlineService.GetKlinesAsync(
                symbol,
                limit,
                fromCloseTime + 1,
                toOpenTime - 1);

            if (missingKlines != null && missingKlines.Count > 0)
            {
                // فقط کندل‌های کامل را ذخیره می‌کنیم (بدون آخرین کندل در حال تشکیل)
                var completedMissingKlines = missingKlines
                    .Where(k => k.CloseTime < toOpenTime)
                    .ToList();

                await InsertCandlesAsync(cryptocurrencyId, completedMissingKlines);
                _logger.LogInformation("Inserted {Count} missing candles for symbol: {Symbol}",
                    completedMissingKlines.Count, symbol);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching missing candles for symbol: {Symbol}", symbol);
            throw;
        }
    }

    private async Task InsertCandlesAsync(int cryptocurrencyId, List<BinanceKlineDto> klines)
    {
        if (klines == null || klines.Count == 0)
            return;

        var candles = klines.Select(k => new Candle_1m
        {
            CryptocurrencyId = cryptocurrencyId,
            OpenTime = DateTimeOffset.FromUnixTimeMilliseconds(k.OpenTime).UtcDateTime,
            CloseTime = DateTimeOffset.FromUnixTimeMilliseconds(k.CloseTime).UtcDateTime,
            Open = k.OpenPrice,
            High = k.HighPrice,
            Low = k.LowPrice,
            Close = k.ClosePrice,
            Volume = k.Volume,
            NumberOfTrades = k.NumberOfTrades
        }).ToList();

        await _repository.AddRangeAsync(candles);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Successfully inserted {Count} candles", candles.Count);
    }
}