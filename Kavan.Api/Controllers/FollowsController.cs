using ApplicationLayer.Common.Extensions;
using ApplicationLayer.Features.Follows.Commands;
using ApplicationLayer.Features.Follows.Query;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kavan.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = "Trader")]
[Authorize(Roles = "User")]
public class FollowsController(IMediator mediator) : ControllerBase
{
    [HttpPost("{targetUserId:int}")]
    public async Task<IActionResult> FollowAsync(int targetUserId)
        => await ResultHelper.GetResultAsync(mediator, new FollowUserCommand(targetUserId));

    [HttpDelete("{targetUserId:int}")]
    public async Task<IActionResult> UnfollowAsync(int targetUserId)
        => await ResultHelper.GetResultAsync(mediator, new UnfollowUserCommand(targetUserId));

    [HttpGet("followers/{userId:int}")]
    public async Task<IActionResult> GetFollowersAsync(int userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        => await ResultHelper.GetResultAsync(mediator, new GetFollowersQuery(userId, page, pageSize));

    [HttpGet("following/{userId:int}")]
    public async Task<IActionResult> GetFollowingAsync(int userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        => await ResultHelper.GetResultAsync(mediator, new GetFollowingQuery(userId, page, pageSize));

    [HttpGet("status/{targetUserId:int}")]
    public async Task<IActionResult> GetStatusAsync(int targetUserId)
        => await ResultHelper.GetResultAsync(mediator, new IsFollowingQuery(targetUserId));
}

