using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.Dto.MarketAnalysis;
using MediatR;

namespace ApplicationLayer.Features.MarketAnalysis.Query;

public record GetActiveSignalsQuery(
    string Symbol = null,
    string Timeframe = null,
    string SignalType = null,
    int PageNumber = 1,
    int PageSize = 50
) : IRequest<HandlerResult>;