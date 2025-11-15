using ApplicationLayer.DTOs.Education;
using MediatR;

namespace ApplicationLayer.Features.Education.Commands;

public record EnrollCourseCommand(EnrollCourseDto Model) : IRequest<HandlerResult>;