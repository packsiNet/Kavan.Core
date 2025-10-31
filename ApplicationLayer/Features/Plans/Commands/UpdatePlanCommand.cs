using ApplicationLayer.DTOs.Plans;
using MediatR;

namespace ApplicationLayer.Features.Plans.Commands;

public record UpdatePlanCommand(int Id, UpdatePlanDto Model) : IRequest<HandlerResult>;