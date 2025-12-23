using ApplicationLayer.Common.Enums;
using ApplicationLayer.Common.Extensions;
using ApplicationLayer.Features.TradingAnalytics.Query;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kavan.Api.Controllers;

[Route("api/analytics")]
[ApiController]
[Authorize]
[ApiExplorerSettings(GroupName = nameof(ApiDefinitions.Public))]
public class TradingAnalyticsController(IMediator mediator) : ControllerBase
{
    [HttpGet("period/{periodId}/summary")]
    public async Task<IActionResult> GetSummaryAsync(int periodId)
        => await ResultHelper.GetResultAsync(mediator, new GetPeriodSummaryQuery(periodId));

    [HttpGet("period/{periodId}/behavior")]
    public async Task<IActionResult> GetBehaviorAsync(int periodId)
        => await ResultHelper.GetResultAsync(mediator, new GetPeriodBehaviorQuery(periodId));

    [HttpGet("period/{periodId}/insights")]
    public async Task<IActionResult> GetInsightsAsync(int periodId)
        => await ResultHelper.GetResultAsync(mediator, new GetPeriodInsightsQuery(periodId));
}
