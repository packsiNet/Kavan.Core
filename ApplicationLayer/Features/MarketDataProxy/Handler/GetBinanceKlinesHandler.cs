using ApplicationLayer.Extensions;
using ApplicationLayer.Features.MarketDataProxy.Query;
using ApplicationLayer.Interfaces.External;
using MediatR;

namespace ApplicationLayer.Features.MarketDataProxy.Handler;

public class GetBinanceKlinesHandler(IMarketDataProxyService _proxy) : IRequestHandler<GetBinanceKlinesQuery, HandlerResult>
{
    public async Task<HandlerResult> Handle(GetBinanceKlinesQuery request, CancellationToken cancellationToken)
    {
        var result = await ResultExtensions.ExecuteAsync(() => _proxy.GetBinanceKlines(request.Symbol, request.Interval));
        return result.ToHandlerResult();
    }
}

