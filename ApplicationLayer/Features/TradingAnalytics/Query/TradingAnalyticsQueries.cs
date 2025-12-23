using ApplicationLayer.Dto.Analytics;
using ApplicationLayer.Dto.BaseDtos;
using MediatR;

namespace ApplicationLayer.Features.TradingAnalytics.Query;

public record GetPeriodSummaryQuery(int PeriodId) : IRequest<HandlerResult>;
public record GetPeriodBehaviorQuery(int PeriodId) : IRequest<HandlerResult>;
public record GetPeriodInsightsQuery(int PeriodId) : IRequest<HandlerResult>;
