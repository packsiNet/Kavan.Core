using ApplicationLayer.Dto.IdeaComments;
using MediatR;

namespace ApplicationLayer.Features.IdeaComments.Commands;

public record CreateIdeaCommentCommand(CreateIdeaCommentDto Model) : IRequest<HandlerResult>;
