using ApplicationLayer.Extensions;
using ApplicationLayer.Features.Ideas.Query;
using ApplicationLayer.Interfaces.Ideas;
using MediatR;

namespace ApplicationLayer.Features.Ideas.Handler;

public class GetMyIdeasHandler(IIdeaService _service) : IRequestHandler<GetMyIdeasQuery, HandlerResult>
{
    public async Task<HandlerResult> Handle(GetMyIdeasQuery request, CancellationToken cancellationToken)
    {
        var result = await _service.GetMineAsync(request.Model);
        return result.ToHandlerResult();
    }
}