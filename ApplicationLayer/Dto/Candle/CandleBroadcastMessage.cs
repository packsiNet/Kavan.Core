using ApplicationLayer.Dto.Candle;

namespace ApplicationLayer.Dto.Candle;

public record CandleBroadcastMessage(
    string Symbol,
    string Timeframe,
    CandleDto Candle
);
