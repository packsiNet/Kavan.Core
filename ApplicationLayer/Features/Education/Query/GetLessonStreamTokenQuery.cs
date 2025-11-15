using MediatR;

namespace ApplicationLayer.Features.Education.Query;

public record GetLessonStreamTokenQuery(int CourseId, int LessonId) : IRequest<HandlerResult>;