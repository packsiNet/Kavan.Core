using MediatR;

namespace ApplicationLayer.Features.Portfolio.Commands;

public record DeletePortfolioSymbolCommand(string Symbol) : IRequest<HandlerResult>;

