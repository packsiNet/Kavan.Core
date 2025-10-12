using ApplicationLayer.Interfaces;
using ApplicationLayer.Interfaces.Binance;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApplicationLayer.Services;

public class CandleAggregatorService(IUnitOfWork _unitOfWork,
    IRepository<Cryptocurrency> _repository,
    IRepository<Candle_1m> _candles_1m,
    IRepository<Candle_5m> _candles_5m,
    IRepository<Candle_1h> _candles_1h,
    IRepository<Candle_4h> _candles_4h,
    IRepository<Candle_1d> _candles_1d
    ) : ICandleAggregatorService
{
    public async Task AggregateCandlesAsync(string targetInterval)
    {
        var symbols = await _repository.Query().ToListAsync();

        foreach (var symbol in symbols)
        {
            switch (targetInterval)
            {
                case "5m":
                    await AggregateAsync(symbol.Id, _candles_1m, _candles_5m, 5);
                    break;
                case "15m":
                    // Not supported yet (no Candle_15m entity). Intentionally skipped.
                    break;
                case "1h":
                    await AggregateAsync(symbol.Id, _candles_5m, _candles_1h, 12);
                    break;
                case "4h":
                    await AggregateAsync(symbol.Id, _candles_1h, _candles_4h, 4);
                    break;
                case "1d":
                    await AggregateAsync(symbol.Id, _candles_4h, _candles_1d, 6);
                    break;
                default:
                    throw new ArgumentException($"Unsupported target interval: {targetInterval}");
            }
        }
    }

    private async Task AggregateAsync<TSource, TTarget>(
        int cryptoId,
        IRepository<TSource> sourceRepo,
        IRepository<TTarget> targetRepo,
        int groupSize)
        where TSource : CandleBase
        where TTarget : CandleBase, new()
    {
        var sourceCandles = await sourceRepo.Query()
            .Where(c => c.CryptocurrencyId == cryptoId)
            .OrderBy(c => c.OpenTime)
            .ToListAsync();

        if (sourceCandles.Count == 0)
            return;

        // Fetch existing target candle open times to avoid duplicate inserts on re-aggregation
        var existingOpenTimes = await targetRepo.Query()
            .Where(t => t.CryptocurrencyId == cryptoId)
            .Select(t => t.OpenTime)
            .ToListAsync();

        var existingSet = new HashSet<DateTime>(existingOpenTimes);

        // Create aggregated candles only for full-size chunks and skip ones already present
        var grouped = sourceCandles
            .Chunk(groupSize)
            .Where(chunk => chunk.Length == groupSize && !existingSet.Contains(chunk[0].OpenTime))
            .Select(chunk => new TTarget
            {
                CryptocurrencyId = cryptoId,
                OpenTime = chunk.First().OpenTime,
                CloseTime = chunk.Last().CloseTime,
                Open = chunk.First().Open,
                Close = chunk.Last().Close,
                High = chunk.Max(c => c.High),
                Low = chunk.Min(c => c.Low),
                Volume = chunk.Sum(c => c.Volume)
            })
            .ToList();

        await targetRepo.AddRangeAsync(grouped);
        await _unitOfWork.SaveChangesAsync();
    }
}