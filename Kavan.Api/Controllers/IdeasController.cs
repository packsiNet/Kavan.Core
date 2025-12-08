using ApplicationLayer.Common.Enums;
using ApplicationLayer.Common.Extensions;
using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.DTOs.Ideas;
using ApplicationLayer.Features.IdeaComments.Commands;
using ApplicationLayer.Features.IdeaComments.Query;
using ApplicationLayer.Features.IdeaRatings.Commands;
using ApplicationLayer.Features.IdeaRatings.Query;
using ApplicationLayer.Features.Ideas.Commands;
using ApplicationLayer.Features.Ideas.Query;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kavan.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = nameof(ApiDefinitions.Trader))]
public class IdeasController(IMediator mediator) : ControllerBase
{
    [HttpGet("{id:int}/comments")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCommentsAsync(int id)
        => await ResultHelper.GetResultAsync(mediator, new GetIdeaCommentsQuery(id));

    [HttpPost("{id:int}/comments")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> AddCommentAsync(int id, [FromBody] ApplicationLayer.Dto.IdeaComments.CreateIdeaCommentDto model)
    {
        model.IdeaId = id;
        return await ResultHelper.GetResultAsync(mediator, new CreateIdeaCommentCommand(model));
    }

    [HttpDelete("comments/{commentId:int}")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> DeleteCommentAsync(int commentId)
        => await ResultHelper.GetResultAsync(mediator, new DeleteIdeaCommentCommand(commentId));

    [HttpGet("{id:int}/rating")]
    [AllowAnonymous]
    public async Task<IActionResult> GetRatingStatsAsync(int id)
        => await ResultHelper.GetResultAsync(mediator, new GetIdeaRatingStatsQuery(id));

    [HttpPost("{id:int}/rating")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> AddRatingAsync(int id, [FromBody] ApplicationLayer.Dto.IdeaRatings.AddIdeaRatingDto model)
    {
        model.IdeaId = id;
        return await ResultHelper.GetResultAsync(mediator, new AddIdeaRatingCommand(model));
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllAsync([FromQuery] string symbol, [FromQuery] string timeframe, [FromQuery] string trend,
                                                 [FromQuery] string tags, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var model = new GetIdeasRequestDto
        {
            Symbol = symbol,
            Timeframe = timeframe,
            Trend = trend,
            Tags = string.IsNullOrWhiteSpace(tags) ? [] : [.. tags.Split(',').Select(x => x.Trim()).Where(x => x.Length > 0)],
            Pagination = new PaginationDto { Page = page, PageSize = pageSize }
        };
        return await ResultHelper.GetResultAsync(mediator, new GetIdeasQuery(model));
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByIdAsync(int id)
        => await ResultHelper.GetResultAsync(mediator, new GetIdeaByIdQuery(id));

    [HttpGet("user/me")]
    //[Authorize(Policy = nameof(ApiDefinitions.Trader), Roles = "User")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> GetMineAsync([FromQuery] string symbol, [FromQuery] string timeframe, [FromQuery] string trend,
                                                  [FromQuery] string tags, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var model = new GetIdeasRequestDto
        {
            Symbol = symbol,
            Timeframe = timeframe,
            Trend = trend,
            Tags = string.IsNullOrWhiteSpace(tags) ? new List<string>() : tags.Split(',').Select(x => x.Trim()).Where(x => x.Length > 0).ToList(),
            Pagination = new PaginationDto { Page = page, PageSize = pageSize }
        };
        return await ResultHelper.GetResultAsync(mediator, new GetMyIdeasQuery(model));
    }

    [HttpGet("user/{userId:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByUserAsync(int userId, [FromQuery] string symbol, [FromQuery] string timeframe, [FromQuery] string trend,
                                                    [FromQuery] string tags, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var model = new GetIdeasRequestDto
        {
            Symbol = symbol,
            Timeframe = timeframe,
            Trend = trend,
            Tags = string.IsNullOrWhiteSpace(tags) ? [] : [.. tags.Split(',').Select(x => x.Trim()).Where(x => x.Length > 0)],
            Pagination = new PaginationDto { Page = page, PageSize = pageSize }
        };
        return await ResultHelper.GetResultAsync(mediator, new GetUserIdeasQuery(userId, model));
    }

    [HttpPost]
    [Authorize(Roles = "User")]
    [RequestSizeLimit(6_000_000)]
    public async Task<IActionResult> CreateAsync([FromForm] CreateIdeaDto model)
        => await ResultHelper.GetResultAsync(mediator, new CreateIdeaCommand(model));

    [HttpPut("{id:int}")]
    [Authorize(Roles = "User")]
    [RequestSizeLimit(6_000_000)]
    public async Task<IActionResult> UpdateAsync(int id, [FromForm] UpdateIdeaDto model)
        => await ResultHelper.GetResultAsync(mediator, new UpdateIdeaCommand(id, model));

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> DeleteAsync(int id)
        => await ResultHelper.GetResultAsync(mediator, new DeleteIdeaCommand(id));
}