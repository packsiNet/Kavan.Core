namespace ApplicationLayer.Dto.MarketAnalysis;

public class MarketAnalysisResponseDto
{
    public string RequestId { get; set; } = string.Empty;
    public List<TradingSignalDto> Signals { get; set; } = new();
    public AnalysisMetadataDto Metadata { get; set; } = new();
    public DateTime AnalyzedAt { get; set; } = DateTime.UtcNow;
    public double ProcessingTimeMs { get; set; }
    public int TotalSymbolsAnalyzed { get; set; }
    public int SignalsGenerated { get; set; }
    public bool Success { get; set; } = true;
    public List<string> Errors { get; set; } = new();
}

public class TradingSignalDto
{
    public string Symbol { get; set; } = string.Empty;
    public string SignalType { get; set; } = string.Empty; // BUY, SELL, HOLD
    public string Timeframe { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal Confidence { get; set; } // 0-100
    public string Strength { get; set; } = string.Empty; // weak, medium, strong
    public string RiskLevel { get; set; } = string.Empty; // low, medium, high
    public List<SignalReasonDto> Reasons { get; set; } = new();
    public List<ConfirmedIndicatorDto> ConfirmedIndicators { get; set; } = new();
    public SignalTargetsDto? Targets { get; set; }
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
}

public class SignalReasonDto
{
    public string ConditionType { get; set; } = string.Empty;
    public string Indicator { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsPrimary { get; set; } = true;
    public decimal Weight { get; set; } // Impact weight on final signal
}

public class ConfirmedIndicatorDto
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty; // confirmed, pending, failed
    public Dictionary<string, object> Values { get; set; } = new();
    public bool IsRequired { get; set; } = true;
}

public class SignalTargetsDto
{
    public decimal? EntryPrice { get; set; }
    public decimal? StopLoss { get; set; }
    public List<decimal> TakeProfits { get; set; } = new();
    public decimal? RiskRewardRatio { get; set; }
}

public class AnalysisMetadataDto
{
    public string AnalysisVersion { get; set; } = "1.0";
    public List<string> AppliedFilters { get; set; } = new();
    public List<string> UsedTimeframes { get; set; } = new();
    public Dictionary<string, int> ConditionMatches { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public List<string> Errors { get; set; } = new();
}