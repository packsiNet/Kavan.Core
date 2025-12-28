using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.Dto.Trade;
using MediatR;

namespace ApplicationLayer.Features.Trade.Query;

public record GetTradesByPeriodQuery(int PeriodId) : IRequest<HandlerResult>;
public record GetTradeCalendarQuery(int? PeriodId) : IRequest<HandlerResult>;
