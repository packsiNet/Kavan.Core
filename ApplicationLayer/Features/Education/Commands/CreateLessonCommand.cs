using ApplicationLayer.DTOs.Education;
using MediatR;

namespace ApplicationLayer.Features.Education.Commands;

public record CreateLessonCommand(CreateLessonDto Model) : IRequest<HandlerResult>;