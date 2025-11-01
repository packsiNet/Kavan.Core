namespace ApplicationLayer.Dto.MarketAnalysis;

public class MarketAnalysisRequestDto
{
    public string Market { get; set; } = "crypto";
    public List<string> Symbols { get; set; } = new();
    public List<string> Timeframes { get; set; } = new();
    public List<AnalysisConditionDto> Conditions { get; set; } = new();
    public AnalysisFiltersDto Filters { get; set; } = new();
    public AnalysisPreferencesDto Preferences { get; set; } = new();
}

public class AnalysisConditionDto
{
    public string Type { get; set; } = string.Empty; // breakout, pattern, etc.
    public string Indicator { get; set; } = string.Empty; // structure_break, fvg_entry, etc.
    public string Description { get; set; } = string.Empty;
    public Dictionary<string, object> Parameters { get; set; } = new();
    public string Timeframe { get; set; } = string.Empty;
    public string LogicalOperator { get; set; } = "AND"; // AND, OR, NOT
    public List<ConditionConfirmationDto> Confirmation { get; set; } = new();
}

public class ConditionConfirmationDto
{
    public string Type { get; set; } = string.Empty;
    public string Condition { get; set; } = string.Empty;
    public bool Required { get; set; } = true;
}

public class AnalysisFiltersDto
{
    public string? VolumeMin { get; set; } // low, medium, high
    public string? Volatility { get; set; } // low, medium, high
    public string? Liquidity { get; set; } // low, medium, high
    public decimal? PriceMin { get; set; }
    public decimal? PriceMax { get; set; }
}

public class AnalysisPreferencesDto
{
    public string RiskLevel { get; set; } = "medium"; // low, medium, high
    public string StrategyType { get; set; } = "price_action"; // price_action, technical, fundamental
    public string SignalStrength { get; set; } = "medium"; // weak, medium, strong
}