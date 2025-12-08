using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities;

public class ChannelPostComment : BaseEntityModel, IAuditableEntity
{
    public int PostId { get; set; }
    public int UserId { get; set; }
    public string Comment { get; set; } = string.Empty;
}
