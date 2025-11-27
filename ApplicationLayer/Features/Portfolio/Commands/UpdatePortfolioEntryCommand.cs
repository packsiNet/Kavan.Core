using ApplicationLayer.DTOs.Portfolio;
using MediatR;

namespace ApplicationLayer.Features.Portfolio.Commands;

public record UpdatePortfolioEntryCommand(int Id, UpdatePortfolioEntryDto Model) : IRequest<HandlerResult>;