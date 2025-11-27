using ApplicationLayer.DTOs.Portfolio;
using MediatR;

namespace ApplicationLayer.Features.Portfolio.Query;

public record GetPortfolioEntriesBySymbolQuery(string Symbol, GetPortfolioEntriesRequestDto Model) : IRequest<HandlerResult>;