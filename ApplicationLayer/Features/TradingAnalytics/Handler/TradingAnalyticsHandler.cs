using ApplicationLayer.Dto.Analytics;
using ApplicationLayer.Extensions;
using ApplicationLayer.Features.TradingAnalytics.Query;
using ApplicationLayer.Interfaces.Services;
using MediatR;

namespace ApplicationLayer.Features.TradingAnalytics.Handler;

public class TradingAnalyticsHandler(ITradingAnalyticsService _service) :
    IRequestHandler<GetPeriodSummaryQuery, HandlerResult>,
    IRequestHandler<GetPeriodBehaviorQuery, HandlerResult>,
    IRequestHandler<GetPeriodInsightsQuery, HandlerResult>
{
    public async Task<HandlerResult> Handle(GetPeriodSummaryQuery request, CancellationToken cancellationToken)
    {
        var result = await _service.GetPeriodSummaryAsync(request.PeriodId);
        return result.ToHandlerResult<PeriodSummaryDto>();
    }

    public async Task<HandlerResult> Handle(GetPeriodBehaviorQuery request, CancellationToken cancellationToken)
    {
        var result = await _service.GetPeriodBehaviorAsync(request.PeriodId);
        return result.ToHandlerResult<PeriodBehaviorDto>();
    }

    public async Task<HandlerResult> Handle(GetPeriodInsightsQuery request, CancellationToken cancellationToken)
    {
        var result = await _service.GetPeriodInsightsAsync(request.PeriodId);
        return result.ToHandlerResult<PeriodInsightsDto>();
    }
}
