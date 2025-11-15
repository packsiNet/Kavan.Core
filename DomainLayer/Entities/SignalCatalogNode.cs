using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities;

public class SignalCatalogNode : BaseEntityModel, IAuditableEntity
{
    public int? ParentId { get; set; }
    public SignalCatalogNode? Parent { get; set; }
    public ICollection<SignalCatalogNode> Children { get; set; } = new List<SignalCatalogNode>();
    public string NameFa { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string Kind { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string SignalName { get; set; } = string.Empty;
}