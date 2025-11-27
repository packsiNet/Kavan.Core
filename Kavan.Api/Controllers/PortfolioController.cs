using ApplicationLayer.Common.Enums;
using ApplicationLayer.Common.Extensions;
using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.DTOs.Portfolio;
using ApplicationLayer.Features.Portfolio.Commands;
using ApplicationLayer.Features.Portfolio.Query;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kavan.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = "Trader")]
[Authorize(Policy = nameof(ApiDefinitions.Public), Roles = "User")]
public class PortfolioController(IMediator mediator) : ControllerBase
{
    [HttpGet("positions")]
    public async Task<IActionResult> GetPositionsAsync([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        => await ResultHelper.GetResultAsync(mediator, new GetPortfolioPositionsQuery(new GetPortfolioRequestDto { Page = page, PageSize = pageSize }));

    [HttpGet("entries/{symbol}")]
    public async Task<IActionResult> GetEntriesAsync(string symbol, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        => await ResultHelper.GetResultAsync(mediator, new GetPortfolioEntriesBySymbolQuery(symbol, new GetPortfolioEntriesRequestDto { Page = page, PageSize = pageSize }));

    [HttpPost("entry")]
    public async Task<IActionResult> AddEntryAsync([FromBody] CreatePortfolioEntryDto model)
        => await ResultHelper.GetResultAsync(mediator, new AddPortfolioEntryCommand(model));

    [HttpPut("entry/{id:int}")]
    public async Task<IActionResult> UpdateEntryAsync(int id, [FromBody] UpdatePortfolioEntryDto model)
        => await ResultHelper.GetResultAsync(mediator, new UpdatePortfolioEntryCommand(id, model));

    [HttpDelete("entry/{id:int}")]
    public async Task<IActionResult> DeleteEntryAsync(int id)
        => await ResultHelper.GetResultAsync(mediator, new DeletePortfolioEntryCommand(id));
}