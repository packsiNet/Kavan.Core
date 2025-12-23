namespace ApplicationLayer.Dto.Analytics;

public class PeriodSummaryDto
{
    public int FinancialPeriodId { get; set; }
    public decimal WinRate { get; set; }
    public decimal Expectancy { get; set; }
    public decimal AvgR { get; set; }
    public decimal ProfitFactor { get; set; }
    public decimal MaxDrawdown { get; set; }
    public decimal TotalPnLPercent { get; set; }
    public int TotalTrades { get; set; }
    public int WinningTrades { get; set; }
    public int LosingTrades { get; set; }
    public int BreakEvenTrades { get; set; }
}

public class PeriodBehaviorDto
{
    public int FinancialPeriodId { get; set; }
    // Add behavior metrics like "Avg Holding Time", "Trades per Day", etc.
    public TimeSpan AvgHoldingTime { get; set; }
    public decimal AvgLeverage { get; set; }
    public Dictionary<string, int> TradesBySide { get; set; } = new();
    public Dictionary<string, int> TradesByReason { get; set; } = new();
}

public class PeriodInsightsDto
{
    public int FinancialPeriodId { get; set; }
    public List<InsightItemDto> Insights { get; set; } = new();
}

public class InsightItemDto
{
    public string Type { get; set; } // Warning, Optimization, Positive
    public string Message { get; set; }
    public string RuleName { get; set; }
}
