using ApplicationLayer.Extensions;
using ApplicationLayer.Features.CustomApis.Commands;
using ApplicationLayer.Interfaces.CustomApis;
using MediatR;

namespace ApplicationLayer.Features.CustomApis.Handler;

public class GetCandleHistoryHandler(ICustomApisService _service) : IRequestHandler<GetCandleHistoryCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(GetCandleHistoryCommand request, CancellationToken cancellationToken)
    {
        var result = await _service.IngestAsync(request.StartDateUtc);
        return result.ToHandlerResult();
    }
}