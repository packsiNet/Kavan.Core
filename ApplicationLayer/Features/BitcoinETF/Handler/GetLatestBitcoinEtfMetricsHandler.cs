using ApplicationLayer.Extensions;
using ApplicationLayer.Features.BitcoinETF.Query;
using ApplicationLayer.Interfaces.Services.Dune;
using MediatR;

namespace ApplicationLayer.Features.BitcoinETF.Handler;

public class GetLatestBitcoinEtfMetricsHandler(IDuneQueryService service) : IRequestHandler<GetLatestBitcoinEtfMetricsQuery, HandlerResult>
{
    public async Task<HandlerResult> Handle(GetLatestBitcoinEtfMetricsQuery request, CancellationToken cancellationToken)
    {
        var result = await service.GetLatestAsync(cancellationToken);
        return result.ToHandlerResult();
    }
}
