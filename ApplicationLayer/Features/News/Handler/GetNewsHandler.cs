using ApplicationLayer.Extensions;
using ApplicationLayer.Features.News.Query;
using ApplicationLayer.Interfaces.Services.News;
using MediatR;

namespace ApplicationLayer.Features.News.Handler;

public class GetNewsHandler(INewsQueryService _service) : IRequestHandler<GetNewsQuery, HandlerResult>
{
    public async Task<HandlerResult> Handle(GetNewsQuery request, CancellationToken cancellationToken)
    {
        var result = await _service.GetAsync(request.Model);
        return result.ToHandlerResult();
    }
}
