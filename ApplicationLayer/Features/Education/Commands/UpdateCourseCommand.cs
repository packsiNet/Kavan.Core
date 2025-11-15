using ApplicationLayer.DTOs.Education;
using MediatR;

namespace ApplicationLayer.Features.Education.Commands;

public record UpdateCourseCommand(int Id, UpdateCourseDto Model) : IRequest<HandlerResult>;