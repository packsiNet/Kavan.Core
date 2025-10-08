using ApplicationLayer.Dto.Cabdles;
using ApplicationLayer.Interfaces;
using ApplicationLayer.Interfaces.Binance;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using InfrastructureLayer.Extensions;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureLayer.BusinessLogic.Services.Binance;

[InjectAsScoped]
public class CandleStorageService(IUnitOfWork _unitOfWork,
    IRepository<Candle_1m> candles_1m,
    IRepository<Candle_5m> candles_5m,
    IRepository<Candle_1h> candles_1h,
    IRepository<Candle_4h> candles_4h,
    IRepository<Candle_1d> candles_1d
    ) : ICandleStorageService
{
    public async Task SaveCandlesAsync(int cryptocurrencyId, string interval, IEnumerable<CandleDto> candles, CancellationToken cancellationToken = default)
    {
        var list = candles
            .Where(c => c != null)
            .OrderBy(c => c.OpenTime)
            .ToList();

        if (!list.Any()) return;

        // dispatch by interval
        switch (interval)
        {
            case "1m":
                await SaveToDbAsync<Candle_1m>(cryptocurrencyId, list, c => MapToCandle1m(c, cryptocurrencyId), candles_1m.GetDbSet(), cancellationToken);
                return;

            case "5m":
                await SaveToDbAsync<Candle_5m>(cryptocurrencyId, list, c => MapToCandle5m(c, cryptocurrencyId), candles_5m.GetDbSet(), cancellationToken);
                return;

            case "1h":
                await SaveToDbAsync<Candle_1h>(cryptocurrencyId, list, c => MapToCandle1h(c, cryptocurrencyId), candles_1h.GetDbSet(), cancellationToken);
                return;

            case "4h":
                await SaveToDbAsync<Candle_4h>(cryptocurrencyId, list, c => MapToCandle4h(c, cryptocurrencyId), candles_4h.GetDbSet(), cancellationToken);
                return;

            case "1d":
                await SaveToDbAsync<Candle_1d>(cryptocurrencyId, list, c => MapToCandle1d(c, cryptocurrencyId), candles_1d.GetDbSet(), cancellationToken);
                return;

            default:
                throw new NotSupportedException($"Interval '{interval}' is not supported by CandleStorageService. Add mapping for this interval.");
        }
    }

    // Generic saver: می‌گیرد DbSet مربوطه، رکوردهای موجود را می‌پرسد و فقط رکوردهای جدید را درج می‌کند
    private async Task SaveToDbAsync<TEntity>(
        int cryptocurrencyId,
        List<CandleDto> candles,
        Func<CandleDto, TEntity> map,
        DbSet<TEntity> dbSet,
        CancellationToken cancellationToken)
        where TEntity : class
    {
        // گرفتن لیست OpenTime فعلی برای این crypto (داخل بازه ورودی برای بهبود کارایی)
        var openTimes = candles.Select(c => c.OpenTime).Distinct().ToList();

        // Query ایمن: فقط OpenTime هایی که وجود دارند بگیر
        // توجه: فرض می‌کنم همه entityها فیلد OpenTime و CryptocurrencyId دارند؛ اگر Base class متفاوت است، ممکن است نیاز به reflection یا expression داشته باشی.
        var existingOpenTimes = await dbSet
            .WhereDynamicOpenTimeAndCrypto(cryptocurrencyId, openTimes) // helper extension پایین‌تر
            .SelectDynamicOpenTime() // helper extension پایین‌تر
            .ToListAsync(cancellationToken);

        var existingSet = new HashSet<DateTime>(existingOpenTimes);

        var toInsert = candles
            .Where(c => !existingSet.Contains(c.OpenTime))
            .Select(map)
            .ToList();

        if (!toInsert.Any()) return;

        // اگر EFCore.BulkExtensions نصب داری، اینجا BulkInsert خیلی سریعتره:
        // await _context.BulkInsertAsync(toInsert, cancellationToken: cancellationToken);

        dbSet.AddRange(toInsert);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    #region Mapping helpers: CandleDto -> specific entity

    private static Candle_1m MapToCandle1m(CandleDto d, int cryptoId) => new Candle_1m
    {
        CryptocurrencyId = cryptoId,
        OpenTime = d.OpenTime,
        CloseTime = d.CloseTime,
        Open = d.Open,
        High = d.High,
        Low = d.Low,
        Close = d.Close,
        Volume = d.Volume
    };

    private static Candle_5m MapToCandle5m(CandleDto d, int cryptoId) => new Candle_5m
    {
        CryptocurrencyId = cryptoId,
        OpenTime = d.OpenTime,
        CloseTime = d.CloseTime,
        Open = d.Open,
        High = d.High,
        Low = d.Low,
        Close = d.Close,
        Volume = d.Volume
    };

    private static Candle_1h MapToCandle1h(CandleDto d, int cryptoId) => new Candle_1h
    {
        CryptocurrencyId = cryptoId,
        OpenTime = d.OpenTime,
        CloseTime = d.CloseTime,
        Open = d.Open,
        High = d.High,
        Low = d.Low,
        Close = d.Close,
        Volume = d.Volume
    };

    private static Candle_4h MapToCandle4h(CandleDto d, int cryptoId) => new Candle_4h
    {
        CryptocurrencyId = cryptoId,
        OpenTime = d.OpenTime,
        CloseTime = d.CloseTime,
        Open = d.Open,
        High = d.High,
        Low = d.Low,
        Close = d.Close,
        Volume = d.Volume
    };

    private static Candle_1d MapToCandle1d(CandleDto d, int cryptoId) => new Candle_1d
    {
        CryptocurrencyId = cryptoId,
        OpenTime = d.OpenTime,
        CloseTime = d.CloseTime,
        Open = d.Open,
        High = d.High,
        Low = d.Low,
        Close = d.Close,
        Volume = d.Volume
    };

    #endregion Mapping helpers: CandleDto -> specific entity
}