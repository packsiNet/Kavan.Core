using ApplicationLayer.Extensions;
using ApplicationLayer.Features.IdeaComments.Query;
using ApplicationLayer.Interfaces.IdeaComments;
using MediatR;

namespace ApplicationLayer.Features.IdeaComments.Handler;

public class GetIdeaCommentsHandler(IIdeaCommentService _service) : IRequestHandler<GetIdeaCommentsQuery, HandlerResult>
{
    public async Task<HandlerResult> Handle(GetIdeaCommentsQuery request, CancellationToken cancellationToken)
    {
        var result = await _service.GetCommentsAsync(request.IdeaId);
        return result.ToHandlerResult();
    }
}
