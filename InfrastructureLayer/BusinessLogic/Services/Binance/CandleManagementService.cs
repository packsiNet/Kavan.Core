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

            // حذف کندل در حال تشکیل (ایندکس 4)
            var completedKlines = klines.Take(4).ToList();

            // دریافت آخرین کندل ذخیره شده
            var lastCandle = await _repository.Query()
                .Where(c => c.CryptocurrencyId == cryptocurrency.Id)
                .OrderByDescending(c => c.CloseTime)
                .FirstOrDefaultAsync();

            if (lastCandle == null)
            {
                // اگر هیچ کندلی ذخیره نشده، به‌صورت تنظیم‌پذیر فقط بخشی از کندل‌ها را ذخیره می‌کنیم
                var initialInsertCount = Math.Max(1, _configuration.GetValue<int>("CandleFetcher:InitialInsertCount", 1));
                var toInsert = completedKlines.TakeLast(Math.Min(initialInsertCount, completedKlines.Count)).ToList();
                _logger.LogInformation("No existing candles found. Inserting {Count} candles for symbol: {Symbol}",
                    toInsert.Count, cryptocurrency.Symbol);
                await InsertCandlesAsync(cryptocurrency.Id, toInsert);
                return;
            }

            // بررسی پیوستگی از ایندکس 2 شروع می‌شود
            var candlesToInsert = new List<BinanceKlineDto>();
            var gapDetected = false;
            long closeTimeTs = new DateTimeOffset(lastCandle.CloseTime).ToUnixTimeMilliseconds();

            for (int i = 2; i < completedKlines.Count; i++)
            {
                var currentKline = completedKlines[i];
                // بررسی پیوستگی: آیا این کندل بلافاصله بعد از آخرین کندل ذخیره شده است؟
                if (currentKline.OpenTime == closeTimeTs + 60_000)
                {
                    // پیوستگی برقرار است - تمام کندل‌های از این ایندکس به بعد را ذخیره می‌کنیم
                    candlesToInsert = [.. completedKlines.Skip(i)];
                    _logger.LogInformation("Continuity found at index {Index}. Inserting {Count} candles for symbol: {Symbol}",
                        i, candlesToInsert.Count, cryptocurrency.Symbol);
                    break;
                }
                else if (i == completedKlines.Count - 1)
                {
                    // اگر تا آخر هیچ پیوستگی نیافتیم، گپ وجود دارد
                    gapDetected = true;
                    _logger.LogWarning("Gap detected for symbol: {Symbol}. Last candle close time: {LastCloseTime}, First new candle open time: {NewOpenTime}",
                        cryptocurrency.Symbol, lastCandle.CloseTime, completedKlines[0].OpenTime);
                }
            }

            if (candlesToInsert.Count > 0)
            {
                await InsertCandlesAsync(cryptocurrency.Id, candlesToInsert);
            }
            else if (gapDetected)
            {
                // دریافت کندل‌های از دست رفته
                await FetchMissingCandlesAsync(
                    cryptocurrency.Id,
                    cryptocurrency.Symbol,
                    closeTimeTs,
                    completedKlines[0].OpenTime);

                // بعد از پر کردن گپ، کندل‌های جدید را هم ذخیره می‌کنیم
                await InsertCandlesAsync(cryptocurrency.Id, completedKlines);
            }
            else
            {
                _logger.LogInformation("No new candles to insert for symbol: {Symbol}", cryptocurrency.Symbol);
            }
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