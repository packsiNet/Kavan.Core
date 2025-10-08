using ApplicationLayer.Dto.Cabdles;

namespace ApplicationLayer.Interfaces.Binance;

public interface ICandleStorageService
{
    /// <summary>
    /// Save or upsert candles for a given cryptocurrency and interval.
    /// cryptocurrencyId => FK to Cryptocurrencies table.
    /// interval => "1m","5m","1h","4h","1d", ...
    /// </summary>
    Task SaveCandlesAsync(int cryptocurrencyId, string interval, IEnumerable<CandleDto> candles, CancellationToken cancellationToken = default);
}