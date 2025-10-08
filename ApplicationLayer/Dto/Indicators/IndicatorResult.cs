namespace ApplicationLayer.Dto.Indicators;

public class IndicatorResult
{
    public string Symbol { get; set; } = string.Empty;
    public string TimeFrame { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public DateTime Timestamp { get; set; }
}