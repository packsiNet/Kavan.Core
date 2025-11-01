using DomainLayer.Entities;

namespace ApplicationLayer.Dto.TechnicalSignals;

public class TechnicalSignalDto
{
    public int Id { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public string IndicatorCategory { get; set; } = string.Empty;
    public string IndicatorName { get; set; } = string.Empty;
    public string ConditionTitle { get; set; } = string.Empty;
    public SignalType SignalType { get; set; }
    public string SignalTypeText { get; set; } = string.Empty;
    public DetailedSignalType DetailedSignalType { get; set; }
    public string DetailedSignalTypeText { get; set; } = string.Empty;
    public string TimeFrame { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public decimal? Value { get; set; }
    public string? AdditionalData { get; set; }
}

public class TechnicalSignalFilterDto
{
    public string? Symbol { get; set; }
    public string? IndicatorCategory { get; set; }
    public string? IndicatorName { get; set; }
    public SignalType? SignalType { get; set; }
    public DetailedSignalType? DetailedSignalType { get; set; }
    public string? TimeFrame { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}

public class TechnicalSignalSummaryDto
{
    public string IndicatorCategory { get; set; } = string.Empty;
    public string IndicatorName { get; set; } = string.Empty;
    public int BuySignals { get; set; }
    public int SellSignals { get; set; }
    public int NeutralSignals { get; set; }
    public int TotalSignals { get; set; }
}