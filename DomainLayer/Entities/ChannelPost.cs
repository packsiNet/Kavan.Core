using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities;

public class ChannelPost : BaseEntityModel, IAuditableEntity
{
    public int ChannelId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
}