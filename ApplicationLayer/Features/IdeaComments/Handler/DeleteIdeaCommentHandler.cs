using ApplicationLayer.Extensions;
using ApplicationLayer.Features.IdeaComments.Commands;
using ApplicationLayer.Interfaces.IdeaComments;
using MediatR;

namespace ApplicationLayer.Features.IdeaComments.Handler;

public class DeleteIdeaCommentHandler(IIdeaCommentService _service) : IRequestHandler<DeleteIdeaCommentCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(DeleteIdeaCommentCommand request, CancellationToken cancellationToken)
    {
        var result = await _service.DeleteCommentAsync(request.Id);
        return result.ToHandlerResult();
    }
}
