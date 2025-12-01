using ApplicationLayer.Extensions;
using ApplicationLayer.Features.Ideas.Query;
using ApplicationLayer.Interfaces.Ideas;
using MediatR;

namespace ApplicationLayer.Features.Ideas.Handler;

public class GetUserIdeasHandler(IIdeaService _service) : IRequestHandler<GetUserIdeasQuery, HandlerResult>
{
    public async Task<HandlerResult> Handle(GetUserIdeasQuery request, CancellationToken cancellationToken)
    {
        var result = await _service.GetForUserAsync(request.UserId, request.Model);
        return result.ToHandlerResult();
    }
}