using MediatR;

namespace ApplicationLayer.Features.Ideas.Commands;

public record DeleteIdeaCommand(int Id) : IRequest<HandlerResult>;