using MediatR;

namespace ApplicationLayer.Features.Education.Query;

public record GetCreatedCoursesQuery() : IRequest<HandlerResult>;
