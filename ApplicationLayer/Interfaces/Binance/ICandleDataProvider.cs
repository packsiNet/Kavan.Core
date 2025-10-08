using ApplicationLayer.Dto.Cabdles;

namespace ApplicationLayer.Interfaces.Binance;

/// <summary>
/// Provides market candle (OHLCV) data from external data sources (e.g., Binance).
/// </summary>
public interface ICandleDataProvider
{
    /// <summary>
    /// Retrieves candle data (klines) for a given symbol and interval.
    /// </summary>
    /// <param name="symbol">Trading pair symbol, e.g. BTCUSDT.</param>
    /// <param name="interval">Candle interval such as 1m, 5m, 1h, 1d, etc.</param>
    /// <param name="limit">Maximum number of candles to fetch (default 500–1000).</param>
    /// <param name="startTime">Optional start time (UTC).</param>
    /// <param name="endTime">Optional end time (UTC).</param>
    /// <returns>List of candle data for the requested interval and time range.</returns>
    Task<IEnumerable<CandleDto>> GetKlinesAsync(
        string symbol,
        string interval,
        int limit = 500,
        DateTime? startTime = null,
        DateTime? endTime = null);
}