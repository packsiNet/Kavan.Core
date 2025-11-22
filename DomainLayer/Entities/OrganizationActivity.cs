using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities;

public class OrganizationActivity : BaseEntityModel, IAuditableEntity
{
    public int OrganizationProfileId { get; set; }

    public OrganizationProfile OrganizationProfile { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}