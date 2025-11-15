using ApplicationLayer.DTOs.Education;
using MediatR;

namespace ApplicationLayer.Features.Education.Commands;

public record ScheduleLessonCommand(int LessonId, ScheduleLessonDto Model) : IRequest<HandlerResult>;