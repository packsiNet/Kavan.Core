using ApplicationLayer.Common.Enums;
using ApplicationLayer.Common.Extensions;
using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.DTOs.Watchlist;
using ApplicationLayer.Features.Watchlist.Commands;
using ApplicationLayer.Features.Watchlist.Query;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kavan.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = "Trader")]
[Authorize(Roles = "User")]
public class WatchlistController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAsync()
        => await ResultHelper.GetResultAsync(mediator, new GetWatchlistsTreeQuery());

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateWatchlistDto model)
        => await ResultHelper.GetResultAsync(mediator, new CreateWatchlistCommand(model));

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] UpdateWatchlistDto model)
        => await ResultHelper.GetResultAsync(mediator, new UpdateWatchlistCommand(id, model));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAsync(int id)
        => await ResultHelper.GetResultAsync(mediator, new DeleteWatchlistCommand(id));

    [HttpPost("{id:int}/items")] 
    public async Task<IActionResult> AddItemAsync(int id, [FromQuery] string symbol)
        => await ResultHelper.GetResultAsync(mediator, new AddWatchlistItemCommand(id, symbol));

    [HttpDelete("items/{itemId:int}")]
    public async Task<IActionResult> RemoveItemAsync(int itemId)
        => await ResultHelper.GetResultAsync(mediator, new RemoveWatchlistItemCommand(itemId));
}