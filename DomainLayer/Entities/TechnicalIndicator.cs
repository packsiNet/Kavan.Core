using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities;

public class TechnicalIndicator : BaseEntityModel
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // FVG, MSS, Support, Resistance, etc.
    public string Symbol { get; set; } = string.Empty;
    public string Timeframe { get; set; } = string.Empty;
    public DateTime DetectedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
    public Dictionary<string, object> Properties { get; set; } = new();
}

public class FairValueGap : TechnicalIndicator
{
    public decimal HighPrice { get; set; }
    public decimal LowPrice { get; set; }
    public decimal MidPrice { get; set; }
    public string Direction { get; set; } = string.Empty; // bullish, bearish
    public bool IsFilled { get; set; } = false;
    public DateTime? FilledAt { get; set; }
    public int CandleIndex { get; set; }
}

public class MarketStructureShift : TechnicalIndicator
{
    public string Direction { get; set; } = string.Empty; // bullish, bearish
    public decimal BreakPrice { get; set; }
    public decimal PreviousStructurePrice { get; set; }
    public string StructureLevel { get; set; } = string.Empty; // minor, major, key
    public bool IsConfirmed { get; set; } = false;
    public decimal Volume { get; set; }
    public string BreakStrength { get; set; } = string.Empty; // weak, medium, strong
}

public class SupportResistanceLevel : TechnicalIndicator
{
    public decimal Price { get; set; }
    public string LevelType { get; set; } = string.Empty; // support, resistance
    public int TouchCount { get; set; }
    public string Strength { get; set; } = string.Empty; // weak, medium, strong
    public List<DateTime> TouchDates { get; set; } = new();
    public bool IsBroken { get; set; } = false;
    public DateTime? BrokenAt { get; set; }
}

public class CandlestickPattern : TechnicalIndicator
{
    public string PatternName { get; set; } = string.Empty; // Hammer, Doji, Engulfing, etc.
    public string Sentiment { get; set; } = string.Empty; // bullish, bearish, neutral
    public decimal Reliability { get; set; } // 0-100
    public int CandleCount { get; set; } // Number of candles in pattern
    public List<int> CandleIndexes { get; set; } = new();
}

public class VolumeProfile : TechnicalIndicator
{
    public decimal Price { get; set; }
    public decimal Volume { get; set; }
    public string VolumeType { get; set; } = string.Empty; // high_volume_node, low_volume_node, point_of_control
    public decimal VolumePercentage { get; set; } // Percentage of total volume
}