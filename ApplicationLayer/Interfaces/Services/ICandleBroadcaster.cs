using ApplicationLayer.Dto.Candle;

namespace ApplicationLayer.Interfaces.Services;

public interface ICandleBroadcaster
{
    Task BroadcastCandleAsync(string symbol, string timeframe, CandleDto candle, CancellationToken ct = default);
}
