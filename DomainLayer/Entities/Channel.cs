using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities;

public class Channel : BaseEntityModel, IAuditableEntity
{
    public int OwnerUserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string AccessType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}