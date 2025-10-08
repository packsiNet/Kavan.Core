namespace ApplicationLayer.Dto.Signals;

public class SignalDto
{
    public string Symbol { get; set; } = string.Empty;
    public string TimeFrame { get; set; } = string.Empty;
    public string SignalType { get; set; } = string.Empty; // BUY/SELL
    public string Strategy { get; set; } = string.Empty; // Breakout, TrendLine, RSI, EMA, MACD
    public DateTime Timestamp { get; set; }

    public decimal? Rsi { get; set; }
    public decimal? Ema { get; set; }
    public decimal? Macd { get; set; }
}