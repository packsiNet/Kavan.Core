using MediatR;

namespace ApplicationLayer.Features.Education.Commands;

public record DeleteCourseCommand(int Id) : IRequest<HandlerResult>;

