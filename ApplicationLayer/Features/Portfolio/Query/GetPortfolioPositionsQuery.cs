using ApplicationLayer.DTOs.Portfolio;
using MediatR;

namespace ApplicationLayer.Features.Portfolio.Query;

public record GetPortfolioPositionsQuery(GetPortfolioRequestDto Model) : IRequest<HandlerResult>;