using ApplicationLayer.Dto.BaseDtos;
using MediatR;

namespace ApplicationLayer.Features.MarketAnalysis.Query;

/// <summary>
/// Query to get all available cryptocurrency symbols from database
/// </summary>
public record GetAvailableSymbolsQuery : IRequest<HandlerResult>;