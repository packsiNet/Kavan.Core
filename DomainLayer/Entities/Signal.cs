using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities;

public class Signal : BaseEntityModel, IAuditableEntity
{
    public int CryptocurrencyId { get; set; }
    public string Symbol { get; set; } = string.Empty;

    public string Timeframe { get; set; } = string.Empty;

    public DateTime SignalTime { get; set; }

    // Signal classification
    public string SignalCategory { get; set; } = string.Empty; // e.g., Breakout
    public string SignalName { get; set; } = string.Empty;     // e.g., ResistanceBreakout or SupportBreakdown
    public int Direction { get; set; } // 1 = Up, -1 = Down, 0 = Neutral

    // Levels
    public decimal BreakoutLevel { get; set; }
    public decimal NearestResistance { get; set; }
    public decimal NearestSupport { get; set; }

    // Pivot points
    public decimal PivotR1 { get; set; }
    public decimal PivotR2 { get; set; }
    public decimal PivotR3 { get; set; }
    public decimal PivotS1 { get; set; }
    public decimal PivotS2 { get; set; }
    public decimal PivotS3 { get; set; }

    // Analytics parameters
    public decimal Atr { get; set; }
    public decimal Tolerance { get; set; }
    public decimal VolumeRatio { get; set; }
    public decimal BodySize { get; set; }

    // Last candle snapshot (optional quick access)
    public DateTime CandleOpenTime { get; set; }
    public DateTime CandleCloseTime { get; set; }
    public decimal CandleOpen { get; set; }
    public decimal CandleHigh { get; set; }
    public decimal CandleLow { get; set; }
    public decimal CandleClose { get; set; }
    public decimal CandleVolume { get; set; }

    public Cryptocurrency Cryptocurrency { get; set; } = null!;
    public ICollection<SignalCandle> Candles { get; set; } = new List<SignalCandle>();
}