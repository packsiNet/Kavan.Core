using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities;

public class DuneMetricsSnapshot : BaseEntityModel, IAuditableEntity
{
    public string ExecutionId { get; set; } = string.Empty;
    public int QueryId { get; set; }

    public DateTime SubmittedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime ExecutionStartedAt { get; set; }
    public DateTime ExecutionEndedAt { get; set; }

    public int RowCount { get; set; }

    public decimal TvlInThousands { get; set; }
    public decimal UsdTvlInBillions { get; set; }
    public decimal PastWeekFlows { get; set; }
    public decimal FlowsUsdSinceApprovalInThousands { get; set; }
    public decimal PastWeekFlowsUsdInThousands { get; set; }
    public decimal PercentageOfBtc { get; set; }
    public decimal BtcSupply { get; set; }
    public decimal SixMonthsAnnualisedImpactOnSupply { get; set; }
    public decimal ThreeMonthsAnnualisedImpactOnSupply { get; set; }
    public decimal MonthlyAnnualisedImpactOnSupply { get; set; }
    public decimal ByWeeklyAnnualisedImpactOnSupply { get; set; }
    public decimal WeekAnnualisedImpactOnSupply { get; set; }
}
