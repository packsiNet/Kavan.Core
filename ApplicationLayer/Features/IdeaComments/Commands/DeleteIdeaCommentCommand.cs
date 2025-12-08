using MediatR;

namespace ApplicationLayer.Features.IdeaComments.Commands;

public record DeleteIdeaCommentCommand(int Id) : IRequest<HandlerResult>;
