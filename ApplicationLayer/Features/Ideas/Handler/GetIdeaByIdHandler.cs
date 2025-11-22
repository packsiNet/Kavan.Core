using ApplicationLayer.Extensions;
using ApplicationLayer.Features.Ideas.Query;
using ApplicationLayer.Interfaces.Ideas;
using MediatR;

namespace ApplicationLayer.Features.Ideas.Handler;

public class GetIdeaByIdHandler(IIdeaService _service) : IRequestHandler<GetIdeaByIdQuery, HandlerResult>
{
    public async Task<HandlerResult> Handle(GetIdeaByIdQuery request, CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(request.Id);
        return result.ToHandlerResult();
    }
}