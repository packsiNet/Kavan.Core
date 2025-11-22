using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities;

public class OrganizationLicense : BaseEntityModel, IAuditableEntity
{
    public int OrganizationProfileId { get; set; }

    public OrganizationProfile OrganizationProfile { get; set; }

    public string RegulatorName { get; set; } = string.Empty;

    public string LicenseNumber { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;
}