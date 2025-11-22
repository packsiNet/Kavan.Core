using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities;

public class OrganizationWebsite : BaseEntityModel, IAuditableEntity
{
    public int OrganizationProfileId { get; set; }

    public OrganizationProfile OrganizationProfile { get; set; }

    public string Url { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;
}