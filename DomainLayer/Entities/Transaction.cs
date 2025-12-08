using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities;

public class Transaction : BaseEntityModel, IAuditableEntity
{
    public int UserId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty; // IRR, USDT
    public string Type { get; set; } = string.Empty; // Deposit, Withdrawal, Purchase
    public string Status { get; set; } = string.Empty; // Pending, Success, Failed
    public string Description { get; set; } = string.Empty;
    public string ReferenceId { get; set; } = string.Empty; // Gateway Ref
    public int? RelatedEntityId { get; set; } // e.g. ChannelId
    public string RelatedEntityType { get; set; } = string.Empty; // "Channel"
}
