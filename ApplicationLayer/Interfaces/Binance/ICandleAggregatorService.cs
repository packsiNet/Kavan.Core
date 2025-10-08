using ApplicationLayer.Dto.Cabdles;

namespace ApplicationLayer.Interfaces.Binance;

public interface ICandleAggregatorService
{
    IEnumerable<CandleDto> Aggregate(IEnumerable<CandleDto> candles, int sourceMinutes, int targetMinutes);
}