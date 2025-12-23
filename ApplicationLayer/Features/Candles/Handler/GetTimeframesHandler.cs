using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.Extensions;
using ApplicationLayer.Features.Candles.Query;
using MediatR;

namespace ApplicationLayer.Features.Candles.Handler;

public class GetTimeframesHandler : IRequestHandler<GetTimeframesQuery, HandlerResult>
{
    public Task<HandlerResult> Handle(GetTimeframesQuery request, CancellationToken cancellationToken)
    {
        var timeframes = new List<string> { "1m", "5m", "15m", "1h", "4h", "1d", "1w" };
        return Task.FromResult(Result<List<string>>.Success(timeframes).ToHandlerResult());
    }
}
