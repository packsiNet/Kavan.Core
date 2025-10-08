using ApplicationLayer.Dto.Cabdles;
using ApplicationLayer.Interfaces.Binance;

namespace ApplicationLayer.Services;

public class CandleAggregatorService : ICandleAggregatorService
{
    public IEnumerable<CandleDto> Aggregate(IEnumerable<CandleDto> candles, int sourceMinutes, int targetMinutes)
    {
        int groupSize = targetMinutes / sourceMinutes;
        return candles
            .OrderBy(c => c.OpenTime)
            .Chunk(groupSize)
            .Select(chunk => new CandleDto
            {
                OpenTime = chunk.First().OpenTime,
                CloseTime = chunk.Last().CloseTime,
                Open = chunk.First().Open,
                Close = chunk.Last().Close,
                High = chunk.Max(c => c.High),
                Low = chunk.Min(c => c.Low),
                Volume = chunk.Sum(c => c.Volume)
            });
    }
}