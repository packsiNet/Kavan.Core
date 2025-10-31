using MediatR;

namespace ApplicationLayer.Features.Plans.Query;

public record GetPlanByIdQuery(int Id) : IRequest<HandlerResult>;