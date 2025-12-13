using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities;

public class DuneGasPriceSnapshot : BaseEntityModel
{
    public string ExecutionId { get; set; } = string.Empty;
    public int QueryId { get; set; }

    public DateTime SubmittedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime ExecutionStartedAt { get; set; }
    public DateTime ExecutionEndedAt { get; set; }

    public int RowCount { get; set; }

    public DateTime Time { get; set; }
    public decimal MedianGasPriceGwei { get; set; }
    public decimal EthTransferPriceUsd { get; set; }
}
