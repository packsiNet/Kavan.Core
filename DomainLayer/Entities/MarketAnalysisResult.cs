using DomainLayer.Common.BaseEntities;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomainLayer.Entities;

public class MarketAnalysisResult : BaseEntityModel
{
    public string RequestId { get; set; } = string.Empty;
    public List<TradingSignal> Signals { get; set; } = new();
    public AnalysisMetadata Metadata { get; set; } = new();
    public DateTime AnalyzedAt { get; set; } = DateTime.UtcNow;
    public TimeSpan ProcessingTime { get; set; }
    public int TotalSymbolsAnalyzed { get; set; }
    public int SignalsGenerated { get; set; }
}

[NotMapped]
public class TradingSignal
{
    public string Symbol { get; set; } = string.Empty;
    public string SignalType { get; set; } = string.Empty; // BUY, SELL, HOLD
    public string Timeframe { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal Confidence { get; set; } // 0-100
    public string Strength { get; set; } = string.Empty; // weak, medium, strong
    public string RiskLevel { get; set; } = string.Empty; // low, medium, high
    public List<SignalReason> Reasons { get; set; } = new();
    public List<ConfirmedIndicator> ConfirmedIndicators { get; set; } = new();
    public SignalTargets Targets { get; set; }
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
}

[NotMapped]
public class SignalReason
{
    public string ConditionType { get; set; } = string.Empty;
    public string Indicator { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsPrimary { get; set; } = true;
    public decimal Weight { get; set; } // Impact weight on final signal
}

[NotMapped]
public class ConfirmedIndicator
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty; // confirmed, pending, failed
    public Dictionary<string, object> Values { get; set; } = new();
    public bool IsRequired { get; set; } = true;
}

[NotMapped]
public class SignalTargets
{
    public decimal? EntryPrice { get; set; }
    public decimal? StopLoss { get; set; }
    public List<decimal> TakeProfits { get; set; } = new();
    public decimal? RiskRewardRatio { get; set; }
}

[NotMapped]
public class AnalysisMetadata
{
    public string AnalysisVersion { get; set; } = "1.0";
    public List<string> AppliedFilters { get; set; } = new();
    public List<string> UsedTimeframes { get; set; } = new();
    public Dictionary<string, int> ConditionMatches { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public List<string> Errors { get; set; } = new();
}