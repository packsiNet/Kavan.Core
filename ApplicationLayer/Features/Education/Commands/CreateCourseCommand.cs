using ApplicationLayer.DTOs.Education;
using MediatR;

namespace ApplicationLayer.Features.Education.Commands;

public record CreateCourseCommand(CreateCourseDto Model) : IRequest<HandlerResult>;