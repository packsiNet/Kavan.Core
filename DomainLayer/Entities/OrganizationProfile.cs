using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities;

public class OrganizationProfile : BaseEntityModel, IAuditableEntity
{
    public int UserAccountId { get; set; }

    public bool IsPublic { get; set; } = true;

    public string OrganizationName { get; set; } = string.Empty;

    public string LegalName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int? FoundedYear { get; set; }

    public string RegistrationNumber { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public string ContactEmailPublic { get; set; } = string.Empty;

    public string ContactPhonePublic { get; set; } = string.Empty;

    public string LogoUrl { get; set; } = string.Empty;

    public string BannerUrl { get; set; } = string.Empty;

    public bool HasExchange { get; set; }

    public bool HasInvestmentPanel { get; set; }

    public UserAccount UserAccount { get; set; }

    public ICollection<OrganizationActivity> Activities { get; set; } = [];

    public ICollection<OrganizationWebsite> Websites { get; set; } = [];

    public ICollection<OrganizationSocialLink> SocialLinks { get; set; } = [];

    public ICollection<OrganizationLicense> Licenses { get; set; } = [];

    public ICollection<OrganizationExchange> Exchanges { get; set; } = [];

    public ICollection<OrganizationInvestmentPanel> InvestmentPanels { get; set; } = [];
}