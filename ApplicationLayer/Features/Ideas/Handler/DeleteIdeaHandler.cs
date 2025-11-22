using ApplicationLayer.Extensions;
using ApplicationLayer.Features.Ideas.Commands;
using ApplicationLayer.Interfaces.Ideas;
using MediatR;

namespace ApplicationLayer.Features.Ideas.Handler;

public class DeleteIdeaHandler(IIdeaService _service) : IRequestHandler<DeleteIdeaCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(DeleteIdeaCommand request, CancellationToken cancellationToken)
    {
        var result = await _service.DeleteAsync(request.Id);
        return result.ToHandlerResult();
    }
}