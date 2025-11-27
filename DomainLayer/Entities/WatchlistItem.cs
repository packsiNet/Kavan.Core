using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities;

public class WatchlistItem : BaseEntityModel, IAuditableEntity
{
    public int WatchlistId { get; set; }
    public string Symbol { get; set; } = string.Empty;
}