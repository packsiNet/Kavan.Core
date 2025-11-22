using ApplicationLayer.Extensions;
using ApplicationLayer.Features.Ideas.Query;
using ApplicationLayer.Interfaces.Ideas;
using MediatR;

namespace ApplicationLayer.Features.Ideas.Handler;

public class GetIdeasHandler(IIdeaService _service) : IRequestHandler<GetIdeasQuery, HandlerResult>
{
    public async Task<HandlerResult> Handle(GetIdeasQuery request, CancellationToken cancellationToken)
    {
        var result = await _service.GetPublicAsync(request.Model);
        return result.ToHandlerResult();
    }
}