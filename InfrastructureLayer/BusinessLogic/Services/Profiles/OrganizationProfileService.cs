using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.DTOs.Profiles.Organizations;
using ApplicationLayer.Interfaces;
using ApplicationLayer.Interfaces.External;
using ApplicationLayer.Interfaces.Profiles;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace InfrastructureLayer.BusinessLogic.Services.Profiles;

[InjectAsScoped]
public class OrganizationProfileService(IUnitOfWork _uow,
                                        IUserContextService _userCtx,
                                        IRepository<OrganizationProfile> _profiles,
                                        IRepository<OrganizationActivity> _activities,
                                        IRepository<OrganizationWebsite> _websites,
                                        IRepository<OrganizationSocialLink> _socials,
                                        IRepository<OrganizationLicense> _licenses,
                                        IRepository<OrganizationExchange> _exchanges,
                                        IRepository<OrganizationInvestmentPanel> _panels,
                                        IRepository<OrganizationPhone> _phones,
                                        IRepository<OrganizationEmail> _emails,
                                        IFileStorageService _files) : IOrganizationProfileService
{
    public async Task<Result<OrganizationProfileDto>> GetPublicByUserIdAsync(int userId)
    {
        var q = _profiles.GetDbSet()
            .Include(x => x.Activities)
            .Include(x => x.Websites)
            .Include(x => x.SocialLinks)
            .Include(x => x.Licenses)
            .Include(x => x.Exchanges)
            .Include(x => x.InvestmentPanels)
            .Include(x => x.Phones)
            .Include(x => x.Emails)
            .FirstOrDefaultAsync(x => x.UserAccountId == userId && x.IsPublic);
        var entity = await q;
        if (entity == null) return Result<OrganizationProfileDto>.NotFound("پروفایل سازمان عمومی یافت نشد");
        return Result<OrganizationProfileDto>.Success(ToDto(entity));
    }

    public async Task<Result<OrganizationProfileDto>> GetMineAsync()
    {
        if (_userCtx.UserId == null) return Result<OrganizationProfileDto>.AuthenticationFailure();
        var uid = _userCtx.UserId.Value;
        var entity = await _profiles.GetDbSet()
            .Include(x => x.Activities)
            .Include(x => x.Websites)
            .Include(x => x.SocialLinks)
            .Include(x => x.Licenses)
            .Include(x => x.Exchanges)
            .Include(x => x.InvestmentPanels)
            .Include(x => x.Phones)
            .Include(x => x.Emails)
            .FirstOrDefaultAsync(x => x.UserAccountId == uid);
        if (entity == null) return Result<OrganizationProfileDto>.NotFound("پروفایل سازمانی یافت نشد");
        return Result<OrganizationProfileDto>.Success(ToDto(entity));
    }

    public async Task<Result<OrganizationProfileDto>> UpsertAsync(UpdateOrganizationProfileDto dto)
    {
        if (_userCtx.UserId == null) return Result<OrganizationProfileDto>.AuthenticationFailure();
        var uid = _userCtx.UserId.Value;
        var entity = await _profiles.GetDbSet()
            .Include(x => x.Activities)
            .Include(x => x.Websites)
            .Include(x => x.SocialLinks)
            .Include(x => x.Licenses)
            .Include(x => x.Exchanges)
            .Include(x => x.InvestmentPanels)
            .FirstOrDefaultAsync(x => x.UserAccountId == uid);

        string logoUrl = null, bannerUrl = null;
        if (dto.Logo != null)
        {
            var r = await _files.SaveProfileImageAsync(dto.Logo, "logo");
            if (r.IsFailure) return Result<OrganizationProfileDto>.Failure(r.Error);
            logoUrl = r.Value;
        }
        if (dto.Banner != null)
        {
            var r = await _files.SaveProfileImageAsync(dto.Banner, "banner");
            if (r.IsFailure) return Result<OrganizationProfileDto>.Failure(r.Error);
            bannerUrl = r.Value;
        }

        await _uow.BeginTransactionAsync();
        var isNew = false;
        if (entity == null)
        {
            entity = new OrganizationProfile
            {
                UserAccountId = uid,
                IsPublic = dto.IsPublic,
                OrganizationName = dto.OrganizationName,
                LegalName = dto.LegalName,
                Description = dto.Description,
                DescriptionDetails = dto.DescriptionDetails,
                FoundedYear = dto.FoundedYear,
                RegistrationNumber = dto.RegistrationNumber,
                Country = dto.Country,
                City = dto.City,
                Address = dto.Address,
                PostalCode = dto.PostalCode,
                ContactEmailPublic = dto.ContactEmailPublic,
                ContactPhonePublic = dto.ContactPhonePublic,
                HasExchange = dto.HasExchange,
                HasInvestmentPanel = dto.HasInvestmentPanel,
                LogoUrl = logoUrl ?? string.Empty,
                BannerUrl = bannerUrl ?? string.Empty
            };
            isNew = true;
            await _profiles.AddAsync(entity);
            await _uow.SaveChangesAsync();
        }
        else
        {
            entity.IsPublic = dto.IsPublic;
            entity.OrganizationName = dto.OrganizationName;
            entity.LegalName = dto.LegalName;
            entity.Description = dto.Description;
            entity.DescriptionDetails = dto.DescriptionDetails;
            entity.FoundedYear = dto.FoundedYear;
            entity.RegistrationNumber = dto.RegistrationNumber;
            entity.Country = dto.Country;
            entity.City = dto.City;
            entity.Address = dto.Address;
            entity.PostalCode = dto.PostalCode;
            entity.ContactEmailPublic = dto.ContactEmailPublic;
            entity.ContactPhonePublic = dto.ContactPhonePublic;
            entity.HasExchange = dto.HasExchange;
            entity.HasInvestmentPanel = dto.HasInvestmentPanel;
            if (logoUrl != null) entity.LogoUrl = logoUrl;
            if (bannerUrl != null) entity.BannerUrl = bannerUrl;
            entity.MarkAsUpdated();

            // Replace child collections
            _activities.RemoveRange(entity.Activities.ToList());
            _websites.RemoveRange(entity.Websites.ToList());
            _socials.RemoveRange(entity.SocialLinks.ToList());
            _licenses.RemoveRange(entity.Licenses.ToList());
            _exchanges.RemoveRange(entity.Exchanges.ToList());
            _panels.RemoveRange(entity.InvestmentPanels.ToList());
            _phones.RemoveRange(entity.Phones.ToList());
            _emails.RemoveRange(entity.Emails.ToList());
        }

        // Re-add child items
        entity.Activities = dto.Activities.Select(a => new OrganizationActivity { OrganizationProfile = entity, Title = a.Title, Description = a.Description ?? string.Empty }).ToList();
        entity.Websites = dto.Websites.Select(w => new OrganizationWebsite { OrganizationProfile = entity, Url = w.Url, Type = w.Type ?? string.Empty }).ToList();
        entity.SocialLinks = dto.SocialLinks.Select(s => new OrganizationSocialLink { OrganizationProfile = entity, Platform = s.Platform ?? string.Empty, Url = s.Url }).ToList();
        entity.Licenses = dto.Licenses.Select(l => new OrganizationLicense { OrganizationProfile = entity, RegulatorName = l.RegulatorName, LicenseNumber = l.LicenseNumber, Country = l.Country }).ToList();
        entity.Exchanges = dto.Exchanges.Select(e => new OrganizationExchange { OrganizationProfile = entity, Name = e.Name, Country = e.Country, Url = e.Url }).ToList();
        entity.InvestmentPanels = dto.InvestmentPanels.Select(p => new OrganizationInvestmentPanel { OrganizationProfile = entity, Name = p.Name, Url = p.Url, MinimumInvestment = p.MinimumInvestment, ProfitShareModel = p.ProfitShareModel }).ToList();
        entity.Phones = dto.Phones.Select(ph => new OrganizationPhone { OrganizationProfile = entity, Title = ph.Title ?? string.Empty, PhoneNumber = ph.PhoneNumber }).ToList();
        entity.Emails = dto.Emails.Select(em => new OrganizationEmail { OrganizationProfile = entity, Title = em.Title ?? string.Empty, Email = em.Email }).ToList();

        if (!isNew)
            await _profiles.UpdateAsync(entity);
        await _uow.SaveChangesAsync();
        await _uow.CommitAsync();

        var updated = await _profiles.GetDbSet()
            .Include(x => x.Activities)
            .Include(x => x.Websites)
            .Include(x => x.SocialLinks)
            .Include(x => x.Licenses)
            .Include(x => x.Exchanges)
            .Include(x => x.InvestmentPanels)
            .Include(x => x.Phones)
            .Include(x => x.Emails)
            .FirstAsync(x => x.Id == entity.Id);
        return Result<OrganizationProfileDto>.Success(ToDto(updated));
    }

    public async Task<Result<string>> UploadLogoAsync(IFormFile file)
    {
        var r = await _files.SaveProfileImageAsync(file, "logo");
        return r;
    }

    public async Task<Result<string>> UploadBannerAsync(IFormFile file)
    {
        var r = await _files.SaveProfileImageAsync(file, "banner");
        return r;
    }

    public async Task<Result<OrganizationsPageDto>> SearchAsync(SearchOrganizationsDto dto)
    {
        var query = _profiles.Query().Where(x => x.IsPublic);
        if (!string.IsNullOrWhiteSpace(dto.Country)) query = query.Where(x => x.Country == dto.Country);
        if (dto.HasExchange.HasValue) query = query.Where(x => x.HasExchange == dto.HasExchange.Value);
        if (dto.HasInvestmentPanel.HasValue) query = query.Where(x => x.HasInvestmentPanel == dto.HasInvestmentPanel.Value);

        var total = await query.CountAsync();
        var items = await query
            .OrderBy(x => x.OrganizationName)
            .Skip((dto.Page - 1) * dto.PageSize)
            .Take(dto.PageSize)
            .Select(x => new OrganizationProfileDto
            {
                Id = x.Id,
                UserAccountId = x.UserAccountId,
                IsPublic = x.IsPublic,
                OrganizationName = x.OrganizationName,
                LegalName = x.LegalName,
                Description = x.Description,
                FoundedYear = x.FoundedYear,
                RegistrationNumber = x.RegistrationNumber,
                Country = x.Country,
                City = x.City,
                Address = x.Address,
                ContactEmailPublic = x.ContactEmailPublic,
                ContactPhonePublic = x.ContactPhonePublic,
                LogoUrl = x.LogoUrl,
                BannerUrl = x.BannerUrl,
                HasExchange = x.HasExchange,
                HasInvestmentPanel = x.HasInvestmentPanel
            }).ToListAsync();

        var page = new OrganizationsPageDto { Items = items, Total = total, Page = dto.Page, PageSize = dto.PageSize };
        return Result<OrganizationsPageDto>.Success(page);
    }

    private static OrganizationProfileDto ToDto(OrganizationProfile e)
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
            InvestmentPanels = e.InvestmentPanels.Select(p => new OrganizationInvestmentPanelDto { Name = p.Name, Url = p.Url, MinimumInvestment = p.MinimumInvestment, ProfitShareModel = p.ProfitShareModel }).ToList(),
            Phones = e.Phones.Select(ph => new OrganizationPhoneDto { Title = ph.Title, PhoneNumber = ph.PhoneNumber }).ToList(),
            Emails = e.Emails.Select(em => new OrganizationEmailDto { Title = em.Title, Email = em.Email }).ToList()
        };
    }
}
