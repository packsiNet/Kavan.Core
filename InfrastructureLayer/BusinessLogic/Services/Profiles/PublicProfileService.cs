using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.DTOs.Profiles.Public;
using ApplicationLayer.DTOs.Profiles.Organizations;
using ApplicationLayer.Interfaces;
using ApplicationLayer.Interfaces.Profiles;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureLayer.BusinessLogic.Services.Profiles;

[InjectAsScoped]
public class PublicProfileService(IRepository<UserAccount> _users,
                                  IRepository<UserProfile> _profiles,
                                  IRepository<OrganizationProfile> _orgProfiles) : IPublicProfileService
{
    public async Task<Result<PublicProfileDto>> GetByUserIdAsync(int userId)
    {
        var user = await _users.Query().FirstOrDefaultAsync(x => x.Id == userId);
        if (user == null) return Result<PublicProfileDto>.NotFound("کاربر یافت نشد");

        var personal = await _profiles.Query().FirstOrDefaultAsync(x => x.UserAccountId == userId);
        var org = await _orgProfiles.GetDbSet()
            .Include(x => x.Activities)
            .Include(x => x.Websites)
            .Include(x => x.SocialLinks)
            .Include(x => x.Licenses)
            .Include(x => x.Exchanges)
            .Include(x => x.InvestmentPanels)
            .FirstOrDefaultAsync(x => x.UserAccountId == userId && x.IsPublic);

        var dto = new PublicProfileDto
        {
            Personal = new PersonalPublicProfileDto
            {
                UserAccountId = user.Id,
                UserName = user.UserName,
                DisplayName = personal?.DisplayName,
                FirstName = personal?.FirstName,
                LastName = personal?.LastName,
                AvatarUrl = user.Avatar
            },
            Organization = org == null ? null : ToOrgDto(org)
        };

        return Result<PublicProfileDto>.Success(dto);
    }

    private static OrganizationProfileDto ToOrgDto(OrganizationProfile e)
    {
        return new OrganizationProfileDto
        {
            Id = e.Id,
            UserAccountId = e.UserAccountId,
            IsPublic = e.IsPublic,
            OrganizationName = e.OrganizationName,
            LegalName = e.LegalName,
            Description = e.Description,
            FoundedYear = e.FoundedYear,
            RegistrationNumber = e.RegistrationNumber,
            Country = e.Country,
            City = e.City,
            Address = e.Address,
            ContactEmailPublic = e.ContactEmailPublic,
            ContactPhonePublic = e.ContactPhonePublic,
            LogoUrl = e.LogoUrl,
            BannerUrl = e.BannerUrl,
            HasExchange = e.HasExchange,
            HasInvestmentPanel = e.HasInvestmentPanel,
            Activities = e.Activities.Select(a => new OrganizationActivityDto { Title = a.Title, Description = a.Description }).ToList(),
            Websites = e.Websites.Select(w => new OrganizationWebsiteDto { Url = w.Url, Type = w.Type }).ToList(),
            SocialLinks = e.SocialLinks.Select(s => new OrganizationSocialLinkDto { Platform = s.Platform, Url = s.Url }).ToList(),
            Licenses = e.Licenses.Select(l => new OrganizationLicenseDto { RegulatorName = l.RegulatorName, LicenseNumber = l.LicenseNumber, Country = l.Country }).ToList(),
            Exchanges = e.Exchanges.Select(x => new OrganizationExchangeDto { Name = x.Name, Country = x.Country, Url = x.Url }).ToList(),
            InvestmentPanels = e.InvestmentPanels.Select(p => new OrganizationInvestmentPanelDto { Name = p.Name, Url = p.Url, MinimumInvestment = p.MinimumInvestment, ProfitShareModel = p.ProfitShareModel }).ToList()
        };
    }
}
