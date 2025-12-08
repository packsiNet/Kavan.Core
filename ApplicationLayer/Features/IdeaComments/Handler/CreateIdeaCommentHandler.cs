using ApplicationLayer.Extensions;
using ApplicationLayer.Features.IdeaComments.Commands;
using ApplicationLayer.Interfaces.IdeaComments;
using MediatR;

namespace ApplicationLayer.Features.IdeaComments.Handler;

public class CreateIdeaCommentHandler(IIdeaCommentService _service) : IRequestHandler<CreateIdeaCommentCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(CreateIdeaCommentCommand request, CancellationToken cancellationToken)
    {
        var result = await _service.AddCommentAsync(request.Model);
        return result.ToHandlerResult();
    }
}
