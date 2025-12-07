using ApplicationLayer.DTOs.Portfolio;
using MediatR;

namespace ApplicationLayer.Features.Portfolio.Commands;

public record AddPortfolioSaleCommand(CreatePortfolioSaleDto Model) : IRequest<HandlerResult>;

