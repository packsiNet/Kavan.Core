using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities;

public class Watchlist : BaseEntityModel, IAuditableEntity
{
    public int OwnerUserId { get; set; }
    public string Name { get; set; } = string.Empty;
}