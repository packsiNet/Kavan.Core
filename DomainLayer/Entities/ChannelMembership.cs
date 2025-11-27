using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities;

public class ChannelMembership : BaseEntityModel, IAuditableEntity
{
    public int ChannelId { get; set; }
    public int UserId { get; set; }
    public bool IsActive { get; set; }
}