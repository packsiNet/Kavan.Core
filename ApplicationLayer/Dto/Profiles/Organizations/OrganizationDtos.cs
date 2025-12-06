using Microsoft.AspNetCore.Http;

namespace ApplicationLayer.DTOs.Profiles.Organizations;

public class OrganizationActivityDto
{
    public string Title { get; set; }

    public string Description { get; set; }
}

public class OrganizationWebsiteDto
{
    public string Url { get; set; }

    public string Type { get; set; }
}

public class OrganizationSocialLinkDto
{
    public string Platform { get; set; }

    public string Url { get; set; }
}

public class OrganizationPhoneDto
{
    public string Title { get; set; }
    public string PhoneNumber { get; set; }
}

public class OrganizationEmailDto
{
    public string Title { get; set; }
    public string Email { get; set; }
}

public class OrganizationLicenseDto
{
    public string RegulatorName { get; set; }

    public string LicenseNumber { get; set; }

    public string Country { get; set; }
}

public class OrganizationExchangeDto
{
    public string Name { get; set; }

    public string Country { get; set; }

    public string Url { get; set; }
}

public class OrganizationInvestmentPanelDto
{
    public string Name { get; set; }

    public string Url { get; set; }

    public decimal? MinimumInvestment { get; set; }

    public string ProfitShareModel { get; set; }
}

public class OrganizationProfileDto
{
    public int Id { get; set; }

    public int UserAccountId { get; set; }

    public bool IsPublic { get; set; }

    public string OrganizationName { get; set; }

    public string LegalName { get; set; }

    public string Description { get; set; }

    public string DescriptionDetails { get; set; }

    public int? FoundedYear { get; set; }

    public string RegistrationNumber { get; set; }

    public string Country { get; set; }

    public string City { get; set; }

    public string Address { get; set; }

    public string PostalCode { get; set; }

    public string ContactEmailPublic { get; set; }

    public string ContactPhonePublic { get; set; }

    public string LogoUrl { get; set; }

    public string BannerUrl { get; set; }

    public bool HasExchange { get; set; }

    public bool HasInvestmentPanel { get; set; }

    public List<OrganizationActivityDto> Activities { get; set; } = [];

    public List<OrganizationWebsiteDto> Websites { get; set; } = [];

    public List<OrganizationSocialLinkDto> SocialLinks { get; set; } = [];

    public List<OrganizationLicenseDto> Licenses { get; set; } = [];

    public List<OrganizationExchangeDto> Exchanges { get; set; } = [];

    public List<OrganizationInvestmentPanelDto> InvestmentPanels { get; set; } = [];

    public List<OrganizationPhoneDto> Phones { get; set; } = [];

    public List<OrganizationEmailDto> Emails { get; set; } = [];
}

public class UpdateOrganizationProfileDto
{
    public bool IsPublic { get; set; } = true;

    public string OrganizationName { get; set; }

    public string LegalName { get; set; }

    public string Description { get; set; }

    public string DescriptionDetails { get; set; }

    public int? FoundedYear { get; set; }

    public string RegistrationNumber { get; set; }

    public string Country { get; set; }

    public string City { get; set; }

    public string Address { get; set; }

    public string PostalCode { get; set; }

    public string ContactEmailPublic { get; set; }

    public string ContactPhonePublic { get; set; }

    public bool HasExchange { get; set; }

    public bool HasInvestmentPanel { get; set; }

    public List<OrganizationActivityDto> Activities { get; set; } = [];

    public List<OrganizationWebsiteDto> Websites { get; set; } = [];

    public List<OrganizationSocialLinkDto> SocialLinks { get; set; } = [];

    public List<OrganizationLicenseDto> Licenses { get; set; } = [];

    public List<OrganizationExchangeDto> Exchanges { get; set; } = [];

    public List<OrganizationInvestmentPanelDto> InvestmentPanels { get; set; } = [];

    public List<OrganizationPhoneDto> Phones { get; set; } = [];

    public List<OrganizationEmailDto> Emails { get; set; } = [];

    public IFormFile Logo { get; set; }

    public IFormFile Banner { get; set; }
}

public class SearchOrganizationsDto
{
    public string Country { get; set; }

    public bool? HasExchange { get; set; }

    public bool? HasInvestmentPanel { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 10;
}

public class OrganizationsPageDto
{
    public List<OrganizationProfileDto> Items { get; set; } = [];

    public int Total { get; set; }

    public int Page { get; set; }

    public int PageSize { get; set; }
}
