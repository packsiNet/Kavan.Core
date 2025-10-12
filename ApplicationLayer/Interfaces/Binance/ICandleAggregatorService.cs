namespace ApplicationLayer.Interfaces.Binance;

public interface ICandleAggregatorService
{
    Task AggregateCandlesAsync(string targetInterval);
}