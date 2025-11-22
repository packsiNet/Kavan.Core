using ApplicationLayer.DTOs.Ideas;
using ApplicationLayer.Extensions;
using ApplicationLayer.Features.Ideas.Commands;
using ApplicationLayer.Interfaces.Ideas;
using MediatR;

namespace ApplicationLayer.Features.Ideas.Handler;

public class UpdateIdeaHandler(IIdeaService _service) : IRequestHandler<UpdateIdeaCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(UpdateIdeaCommand request, CancellationToken cancellationToken)
    {
        var result = await _service.UpdateAsync(request.Id, request.Model);
        return result.ToHandlerResult();
    }
}