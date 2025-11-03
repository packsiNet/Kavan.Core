using MediatR;

namespace ApplicationLayer.Features.Plans.Commands;

public record DeletePlanCommand(int Id) : IRequest<HandlerResult>;