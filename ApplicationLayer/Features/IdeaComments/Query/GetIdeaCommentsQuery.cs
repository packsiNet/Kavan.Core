using MediatR;

namespace ApplicationLayer.Features.IdeaComments.Query;

public record GetIdeaCommentsQuery(int IdeaId) : IRequest<HandlerResult>;
