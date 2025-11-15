using MediatR;

namespace ApplicationLayer.Features.Education.Query;

public record GetMyCoursesQuery() : IRequest<HandlerResult>;