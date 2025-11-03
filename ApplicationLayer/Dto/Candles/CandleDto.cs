namespace ApplicationLayer.Dto.Candles;

public class CandleDto
{
    public DateTime OpenTime { get; set; }

    public DateTime CloseTime { get; set; }

    public decimal Open { get; set; }

    public decimal High { get; set; }

    public decimal Low { get; set; }

    public decimal Close { get; set; }

    public decimal Volume { get; set; }

    public string Symbol { get; set; } = string.Empty;

    public string Interval { get; set; } = string.Empty;
}