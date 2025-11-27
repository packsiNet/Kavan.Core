using MediatR;

namespace ApplicationLayer.Features.Portfolio.Commands;

public record DeletePortfolioEntryCommand(int Id) : IRequest<HandlerResult>;