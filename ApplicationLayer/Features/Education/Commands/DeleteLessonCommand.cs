using MediatR;

namespace ApplicationLayer.Features.Education.Commands;

public record DeleteLessonCommand(int LessonId) : IRequest<HandlerResult>;