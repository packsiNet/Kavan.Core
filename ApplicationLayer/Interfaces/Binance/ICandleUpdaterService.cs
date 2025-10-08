namespace ApplicationLayer.Interfaces.Binance;

public interface ICandleUpdaterService
{
    Task UpdateCandlesAsync(string symbol, string interval, CancellationToken cancellationToken = default);
}