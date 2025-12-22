namespace ApplicationLayer.Dto.Candle;

public record CandleDto(
    long OpenTime,
    long CloseTime,
    decimal Open,
    decimal High,
    decimal Low,
    decimal Close,
    decimal Volume,
    bool IsFinal
);
