using DomainLayer.Common.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomainLayer.Entities;

public class TradeResult
{
    public decimal? ExitPrice { get; set; }
    public int? ExitReason { get; set; } // ExitReason Enum
    public decimal? RMultiple { get; set; }
    public decimal? PnLPercent { get; set; }
    public decimal? PnL { get; set; }
    public TimeSpan? HoldingTime { get; set; }

    [NotMapped]
    public ExitReason ExitReasonEnum
    {
        get => ExitReason.HasValue ? DomainLayer.Common.Enums.ExitReason.FromValue(ExitReason.Value) : null;
        set => ExitReason = value?.Value;
    }
}
