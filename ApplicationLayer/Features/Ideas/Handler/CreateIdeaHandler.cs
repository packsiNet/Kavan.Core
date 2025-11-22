using ApplicationLayer.DTOs.Ideas;
using ApplicationLayer.Extensions;
using ApplicationLayer.Features.Ideas.Commands;
using ApplicationLayer.Interfaces.Ideas;
using MediatR;

namespace ApplicationLayer.Features.Ideas.Handler;

public class CreateIdeaHandler(IIdeaService _service) : IRequestHandler<CreateIdeaCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(CreateIdeaCommand request, CancellationToken cancellationToken)
    {
        var result = await _service.CreateAsync(request.Model);
        return result.ToHandlerResult();
    }
}