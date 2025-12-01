using ApplicationLayer.DTOs.Portfolio;
using MediatR;

namespace ApplicationLayer.Features.Portfolio.Commands;

public record AddPortfolioEntryCommand(CreatePortfolioEntryDto Model) : IRequest<HandlerResult>;