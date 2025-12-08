using ApplicationLayer.Common.Enums;
using ApplicationLayer.Common.Extensions;
using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.DTOs.Channels;
using ApplicationLayer.Features.Channels.Commands;
using ApplicationLayer.Features.Channels.Query;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kavan.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = nameof(ApiDefinitions.Public))]
public class ChannelsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllAsync([FromQuery] int? category, [FromQuery] int? accessType, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var model = new GetChannelsRequestDto
        {
            Category = category,
            AccessType = accessType,
            Pagination = new PaginationDto { Page = page, PageSize = pageSize }
        };
        return await ResultHelper.GetResultAsync(mediator, new GetChannelsQuery(model));
    }

    [HttpGet("my")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> GetMineAsync([FromQuery] int? category, [FromQuery] int? accessType, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var model = new GetChannelsRequestDto
        {
            Category = category,
            AccessType = accessType,
            Pagination = new PaginationDto { Page = page, PageSize = pageSize }
        };
        return await ResultHelper.GetResultAsync(mediator, new GetMyChannelsQuery(model));
    }

    [HttpGet("created")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> GetCreatedAsync([FromQuery] int? category, [FromQuery] int? accessType, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var model = new GetChannelsRequestDto
        {
            Category = category,
            AccessType = accessType,
            Pagination = new PaginationDto { Page = page, PageSize = pageSize }
        };
        return await ResultHelper.GetResultAsync(mediator, new GetCreatedChannelsQuery(model));
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByIdAsync(int id)
        => await ResultHelper.GetResultAsync(mediator, new GetChannelByIdQuery(id));

    [HttpPost]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> CreateAsync([FromBody] CreateChannelDto model)
        => await ResultHelper.GetResultAsync(mediator, new CreateChannelCommand(model));

    [HttpPut("{id:int}")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] UpdateChannelDto model)
        => await ResultHelper.GetResultAsync(mediator, new UpdateChannelCommand(id, model));

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> DeleteAsync(int id)
        => await ResultHelper.GetResultAsync(mediator, new DeleteChannelCommand(id));

    [HttpPost("{id:int}/join")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> JoinAsync(int id)
        => await ResultHelper.GetResultAsync(mediator, new JoinChannelCommand(id));

    [HttpPost("{id:int}/leave")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> LeaveAsync(int id)
        => await ResultHelper.GetResultAsync(mediator, new LeaveChannelCommand(id));

    [HttpPost("{id:int}/rate")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> RateAsync(int id, [FromBody] RateChannelDto model)
        => await ResultHelper.GetResultAsync(mediator, new RateChannelCommand(model));

    [HttpPost("{id:int}/mute")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> MuteAsync(int id)
        => await ResultHelper.GetResultAsync(mediator, new MuteChannelCommand(id));
}