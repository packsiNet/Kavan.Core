using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.DTOs.Profiles.Organizations;
using Microsoft.AspNetCore.Http;

namespace ApplicationLayer.Interfaces.Profiles;

public interface IOrganizationProfileService
{
    Task<Result<OrganizationProfileDto>> GetPublicByUserIdAsync(int userId);
    Task<Result<OrganizationProfileDto>> GetMineAsync();
    Task<Result<OrganizationProfileDto>> UpsertAsync(UpdateOrganizationProfileDto dto);
    Task<Result<string>> UploadLogoAsync(IFormFile file);
    Task<Result<string>> UploadBannerAsync(IFormFile file);
    Task<Result<OrganizationsPageDto>> SearchAsync(SearchOrganizationsDto dto);
}