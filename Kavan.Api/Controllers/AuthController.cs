using ApplicationLayer.Common.Enums;
using ApplicationLayer.Common.Extensions;
using ApplicationLayer.DTOs.Identity;
using ApplicationLayer.DTOs.RefreshTokens;
using ApplicationLayer.Features.Identity.Commands;
using ApplicationLayer.Features.Identity.Query;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kavan.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = "Identity")]
public class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("signup")]
    [AllowAnonymous]
    public async Task<IActionResult> SignUpAsync([FromBody] SignUpDto model)
        => await ResultHelper.GetResultAsync(mediator, new SignUpCommand(model));

    [HttpPost("signin")]
    [AllowAnonymous]
    public async Task<IActionResult> SignInAsync([FromBody] SignInDto model)
        => await ResultHelper.GetResultAsync(mediator, new SignInCommand(model));

    [HttpPost("token/refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshTokenAsync([FromBody] TokenRequestDto model)
        => await ResultHelper.GetResultAsync(mediator, new TokenRequestQuery(model.AccessTokens, model.RefreshToken));

    [HttpPost("token/revoke")]
    [Authorize(Policy = nameof(ApiDefinitions.Public), Roles = "Users")]
    public async Task<IActionResult> RevokeTokensAsync([FromBody] RevokeRefreshTokenDto model)
        => await ResultHelper.GetResultAsync(mediator, new ApplicationLayer.Features.Identity.Commands.RevokeRefreshTokenCommand(model.UserId));
}