using ApplicationLayer.Common.Enums;
using ApplicationLayer.Common.Extensions;
using ApplicationLayer.Dto.FinancialPeriod;
using ApplicationLayer.Features.FinancialPeriod.Commands;
using ApplicationLayer.Features.FinancialPeriod.Query;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kavan.Api.Controllers;

[Route("api/financial-periods")]
[ApiController]
[Authorize]
[ApiExplorerSettings(GroupName = nameof(ApiDefinitions.Public))]
public class FinancialPeriodController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateFinancialPeriodDto dto)
        => await ResultHelper.GetResultAsync(mediator, new CreateFinancialPeriodCommand(dto));

    [HttpGet("active")]
    public async Task<IActionResult> GetActiveAsync()
        => await ResultHelper.GetResultAsync(mediator, new GetActiveFinancialPeriodQuery());

    [HttpPost("{id}/close")]
    public async Task<IActionResult> CloseAsync(int id)
        => await ResultHelper.GetResultAsync(mediator, new CloseFinancialPeriodCommand(id));
}
