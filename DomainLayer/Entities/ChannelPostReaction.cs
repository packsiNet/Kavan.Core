using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities;

public class ChannelPostReaction : BaseEntityModel, IAuditableEntity
{
    public int PostId { get; set; }
    public int UserId { get; set; }
    public string Reaction { get; set; } = string.Empty;
}