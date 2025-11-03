using MediatR;

namespace ApplicationLayer.Features.Plans.Query;

public record GetPlansQuery() : IRequest<HandlerResult>;