using ApplicationLayer.Common.Enums;
using ApplicationLayer.Common.Extensions;
using ApplicationLayer.Dto.Trade;
using ApplicationLayer.Features.Trade.Commands;
using ApplicationLayer.Features.Trade.Query;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kavan.Api.Controllers;

[Route("api/trades")]
[ApiController]
[Authorize]
[ApiExplorerSettings(GroupName = nameof(ApiDefinitions.Public))]
public class TradeController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateTradeDto dto)
        => await ResultHelper.GetResultAsync(mediator, new CreateTradeCommand(dto));

    [HttpGet("period/{periodId}")]
    public async Task<IActionResult> GetByPeriodAsync(int periodId)
        => await ResultHelper.GetResultAsync(mediator, new GetTradesByPeriodQuery(periodId));

    [HttpGet("calendar")]
    public async Task<IActionResult> GetCalendarAsync([FromQuery] int? periodId)
        => await ResultHelper.GetResultAsync(mediator, new GetTradeCalendarQuery(periodId));

    [HttpPost("{id}/close")]
    public async Task<IActionResult> CloseAsync(int id)
        => await ResultHelper.GetResultAsync(mediator, new CloseTradeCommand(id));

    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> CancelAsync(int id, [FromBody] CancelTradeDto dto)
        => await ResultHelper.GetResultAsync(mediator, new CancelTradeCommand(id, dto));

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] UpdateTradeDto dto)
    {
        if (id != dto.TradeId) return BadRequest("Mismatched Trade ID");
        return await ResultHelper.GetResultAsync(mediator, new UpdateTradeCommand(dto));
    }
}
