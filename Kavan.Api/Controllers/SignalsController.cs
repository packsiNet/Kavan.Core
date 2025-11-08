using ApplicationLayer.Common.Enums;
using ApplicationLayer.Common.Extensions;
using ApplicationLayer.Dto.Signals;
using ApplicationLayer.Features.Signals.Query;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kavan.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = nameof(ApiDefinitions.Trader))]
public class SignalsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Analyze signals for given request
    /// </summary>
    [HttpPost("analyze")]
    [AllowAnonymous]
    public async Task<IActionResult> AnalyzeAsync([FromBody] SignalRequestDto model)
        => await ResultHelper.GetResultAsync(mediator, new GetSignalsQuery(model));

    /// <summary>
    /// دریافت دسته‌بندی سیگنال‌ها به‌همراه انواع و تعداد هر نوع
    /// </summary>
    [HttpGet("catalog")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCatalogAsync()
        => await ResultHelper.GetResultAsync(mediator, new GetSignalsGroupedQuery());

    /// <summary>
    /// دریافت فهرست سیگنال‌ها براساس دسته‌بندی و نوع سیگنال
    /// مثال: category=Technical, name=ResistanceBreakout
    /// </summary>
    [HttpGet("list")]
    [AllowAnonymous]
    public async Task<IActionResult> GetListAsync([FromQuery] string category, [FromQuery] string name)
        => await ResultHelper.GetResultAsync(mediator, new GetSignalsByClassificationQuery(category, name));

    /// <summary>
    /// دریافت جزئیات یک سیگنال براساس شناسه
    /// </summary>
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByIdAsync([FromRoute] int id)
        => await ResultHelper.GetResultAsync(mediator, new GetSignalByIdQuery(id));
}