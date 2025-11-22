using ApplicationLayer.Common.Enums;
using ApplicationLayer.Common.Extensions;
using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.DTOs.Ideas;
using ApplicationLayer.Features.Ideas.Commands;
using ApplicationLayer.Features.Ideas.Query;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kavan.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = nameof(ApiDefinitions.Public))]
public class IdeasController(IMediator mediator) : ControllerBase
{
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

    [HttpPost]
    [Authorize(Policy = nameof(ApiDefinitions.Trader), Roles = "Users")]
    [RequestSizeLimit(6_000_000)]
    public async Task<IActionResult> CreateAsync([FromForm] CreateIdeaDto model)
        => await ResultHelper.GetResultAsync(mediator, new CreateIdeaCommand(model));

    [HttpPut("{id:int}")]
    [Authorize(Policy = nameof(ApiDefinitions.Trader), Roles = "Users")]
    [RequestSizeLimit(6_000_000)]
    public async Task<IActionResult> UpdateAsync(int id, [FromForm] UpdateIdeaDto model)
        => await ResultHelper.GetResultAsync(mediator, new UpdateIdeaCommand(id, model));

    [HttpDelete("{id:int}")]
    [Authorize(Policy = nameof(ApiDefinitions.Trader), Roles = "Users")]
    public async Task<IActionResult> DeleteAsync(int id)
        => await ResultHelper.GetResultAsync(mediator, new DeleteIdeaCommand(id));
}