using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities;

public class ChannelRating : BaseEntityModel, IAuditableEntity
{
    public int ChannelId { get; set; }
    public int UserId { get; set; }
    public int Stars { get; set; }
}