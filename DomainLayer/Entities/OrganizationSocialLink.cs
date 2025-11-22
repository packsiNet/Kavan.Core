using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities;

public class OrganizationSocialLink : BaseEntityModel, IAuditableEntity
{
    public int OrganizationProfileId { get; set; }

    public OrganizationProfile OrganizationProfile { get; set; }

    public string Platform { get; set; } = string.Empty;

    public string Url { get; set; } = string.Empty;
}