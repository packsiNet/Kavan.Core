using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities;

public class DuneEtfIssuerFlowSnapshot : BaseEntityModel
{
    public string ExecutionId { get; set; } = string.Empty;
    public int QueryId { get; set; }

    public DateTime SubmittedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime ExecutionStartedAt { get; set; }
    public DateTime ExecutionEndedAt { get; set; }

    public int RowCount { get; set; }

    public DateTime Time { get; set; }
    public string Issuer { get; set; } = string.Empty;
    public string EtfTicker { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal AmountUsd { get; set; }
    public decimal? AmountNetFlow { get; set; }
    public decimal? AmountUsdNetFlow { get; set; }
}
