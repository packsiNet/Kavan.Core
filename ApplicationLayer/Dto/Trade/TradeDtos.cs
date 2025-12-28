namespace ApplicationLayer.Dto.Trade;

public class CreateTradeDto
{
    public string Symbol { get; set; } = string.Empty;
    public int Side { get; set; } // TradeSide value
    public decimal EntryPrice { get; set; }
    public decimal StopLoss { get; set; }
    public List<decimal> TakeProfits { get; set; } = new();
    public decimal Quantity { get; set; }
    public int Leverage { get; set; }
    
    // Emotion
    public int ConfidenceLevel { get; set; }
    public string EmotionBeforeEntry { get; set; } = string.Empty;
    public bool PlanCompliance { get; set; }
}

public class CloseTradeDto
{
    // Empty - Close at Market
}

public class CancelTradeDto
{
    public string Reason { get; set; } = string.Empty;
}

public class UpdateTradeDto
{
    public int TradeId { get; set; }
    // Only mutable fields
    public int? ConfidenceLevel { get; set; }
    public string? EmotionBeforeEntry { get; set; }
    public bool? PlanCompliance { get; set; }
    public decimal? StopLoss { get; set; }
    public List<decimal>? TakeProfits { get; set; }
}

public class TradeDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Symbol { get; set; }
    public int Side { get; set; }
    public string SideName { get; set; }
    public decimal EntryPrice { get; set; }
    public decimal StopLoss { get; set; }
    public List<TradeTpDto> TakeProfits { get; set; } = new();
    public decimal Quantity { get; set; }
    public int Leverage { get; set; }
    public int Status { get; set; }
    public string StatusName { get; set; }
    public DateTime OpenedAtUtc { get; set; }
    public DateTime? ClosedAtUtc { get; set; }
    public int FinancialPeriodId { get; set; }
    
    public TradeEmotionDto Emotion { get; set; }
    public TradeResultDto Result { get; set; }
}

public class TradeTpDto
{
    public decimal Price { get; set; }
    public bool IsHit { get; set; }
}

public class TradeEmotionDto
{
    public int ConfidenceLevel { get; set; }
    public string EmotionBeforeEntry { get; set; }
    public bool PlanCompliance { get; set; }
}

public class TradeResultDto
{
    public decimal? ExitPrice { get; set; }
    public int? ExitReason { get; set; }
    public string ExitReasonName { get; set; }
    public decimal? RMultiple { get; set; }
    public decimal? PnLPercent { get; set; }
    public decimal? PnL { get; set; }
    public TimeSpan? HoldingTime { get; set; }
}
