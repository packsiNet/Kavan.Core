using ApplicationLayer.Extensions;
using ApplicationLayer.Features.MarketDataProxy.Query;
using ApplicationLayer.Interfaces.External;
using MediatR;

namespace ApplicationLayer.Features.MarketDataProxy.Handler;

public class GetCoinbaseCandlesHandler(IMarketDataProxyService _proxy) : IRequestHandler<GetCoinbaseCandlesQuery, HandlerResult>
{
    public async Task<HandlerResult> Handle(GetCoinbaseCandlesQuery request, CancellationToken cancellationToken)
    {
        var result = await ResultExtensions.ExecuteAsync(() => _proxy.GetCoinbaseCandles(request.Symbol, request.Granularity));
        return result.ToHandlerResult();
    }
}

