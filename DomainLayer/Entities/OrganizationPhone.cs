using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities;

public class OrganizationPhone : BaseEntityModel, IAuditableEntity
{
    public int OrganizationProfileId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public OrganizationProfile OrganizationProfile { get; set; }
}
