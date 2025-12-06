using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities;

public class OrganizationEmail : BaseEntityModel, IAuditableEntity
{
    public int OrganizationProfileId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public OrganizationProfile OrganizationProfile { get; set; }
}
