using MediatR;

namespace ApplicationLayer.Features.Education.Query;

public record GetCoursesQuery(int? CategoryId, byte? CourseLevelValue, bool? IsFree) : IRequest<HandlerResult>;