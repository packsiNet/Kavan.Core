using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities;

public class OrganizationExchange : BaseEntityModel, IAuditableEntity
{
    public int OrganizationProfileId { get; set; }

    public OrganizationProfile OrganizationProfile { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    public string Url { get; set; } = string.Empty;
}