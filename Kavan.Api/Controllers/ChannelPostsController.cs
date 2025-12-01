using ApplicationLayer.Common.Enums;
using ApplicationLayer.Common.Extensions;
using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.DTOs.ChannelPosts;
using ApplicationLayer.Features.ChannelPosts.Commands;
using ApplicationLayer.Features.ChannelPosts.Query;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kavan.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = nameof(ApiDefinitions.Public))]
public class ChannelPostsController(IMediator mediator) : ControllerBase
{
    [HttpGet("channel/{channelId:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByChannelAsync(int channelId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var model = new GetPostsRequestDto { Pagination = new PaginationDto { Page = page, PageSize = pageSize } };
        return await ResultHelper.GetResultAsync(mediator, new GetPostsByChannelQuery(channelId, model));
    }

    [HttpPost("signal")]
    [Authorize(Roles = "User")]
    [RequestSizeLimit(6_000_000)]
    public async Task<IActionResult> CreateSignalAsync([FromForm] CreateSignalPostDto model)
        => await ResultHelper.GetResultAsync(mediator, new CreateSignalPostCommand(model));

    [HttpPost("news")]
    [Authorize(Roles = "User")]
    [RequestSizeLimit(6_000_000)]
    public async Task<IActionResult> CreateNewsAsync([FromForm] CreateNewsPostDto model)
        => await ResultHelper.GetResultAsync(mediator, new CreateNewsPostCommand(model));

    [HttpPut("signal/{id:int}")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> UpdateSignalAsync(int id, [FromBody] UpdateSignalPostDto model)
        => await ResultHelper.GetResultAsync(mediator, new UpdateSignalPostCommand(id, model));

    [HttpPut("news/{id:int}")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> UpdateNewsAsync(int id, [FromBody] UpdateNewsPostDto model)
        => await ResultHelper.GetResultAsync(mediator, new UpdateNewsPostCommand(id, model));

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> DeleteAsync(int id)
        => await ResultHelper.GetResultAsync(mediator, new DeletePostCommand(id));

    [HttpPost("{id:int}/react")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> ReactAsync(int id, [FromBody] ReactPostDto model)
        => await ResultHelper.GetResultAsync(mediator, new ReactPostCommand(model));
}