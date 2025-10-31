using ApplicationLayer.DTOs.Plans;
using MediatR;

namespace ApplicationLayer.Features.Plans.Commands;

public record CreatePlanCommand(CreatePlanDto Model) : IRequest<HandlerResult>;