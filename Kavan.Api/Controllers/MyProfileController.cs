using ApplicationLayer.Common.Enums;
using ApplicationLayer.Common.Extensions;
using ApplicationLayer.DTOs.Profiles.Organizations;
using ApplicationLayer.Features.Profiles.Organizations.Commands;
using ApplicationLayer.Features.Profiles.Organizations.Query;
using ApplicationLayer.DTOs.User;
using ApplicationLayer.Features.Profiles.Users.Query;
using ApplicationLayer.Features.Profiles.Users.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kavan.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = "Trader")]
[Authorize(Roles = "User")]
public class MyProfileController(IMediator mediator) : ControllerBase
{
    [HttpGet("organization")]
    public async Task<IActionResult> GetMineAsync()
        => await ResultHelper.GetResultAsync(mediator, new GetMyOrganizationProfileQuery());

    [HttpPut("organization")]
    [RequestSizeLimit(12_000_000)]
    public async Task<IActionResult> UpsertAsync([FromForm] UpdateOrganizationProfileDto model)
        => await ResultHelper.GetResultAsync(mediator, new UpsertOrganizationProfileCommand(model));

    [HttpPost("organization/logo", Name = "UploadLogo")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [RequestSizeLimit(6_000_000)]
    public async Task<IActionResult> UploadLogoAsync(IFormFile file)
        => await ResultHelper.GetResultAsync(mediator, new UploadLogoCommand(file));

    [HttpPost("organization/banner", Name = "UploadBanner")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [RequestSizeLimit(6_000_000)]
    public async Task<IActionResult> UploadBannerAsync(IFormFile file)
        => await ResultHelper.GetResultAsync(mediator, new UploadBannerCommand(file));

    [HttpGet("personal")]
    public async Task<IActionResult> GetMyPersonalAsync()
        => await ResultHelper.GetResultAsync(mediator, new GetMyUserProfileQuery());

    [HttpPut("personal")]
    public async Task<IActionResult> UpdateMyPersonalAsync([FromBody] UpdateUserProfileDto model)
        => await ResultHelper.GetResultAsync(mediator, new UpdateMyUserProfileCommand(model));
}
