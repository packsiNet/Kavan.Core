using ApplicationLayer.Extensions;
using ApplicationLayer.Features.BitcoinETF.Query;
using ApplicationLayer.Interfaces.Services.Dune;
using MediatR;

namespace ApplicationLayer.Features.BitcoinETF.Handler;

public class GetBitcoinUserCountsHandler(IDuneUserCountAggQueryService service) : IRequestHandler<GetBitcoinUserCountsQuery, HandlerResult>
{
    public async Task<HandlerResult> Handle(GetBitcoinUserCountsQuery request, CancellationToken cancellationToken)
    {
        var result = await service.GetAggregatedAsync(cancellationToken);
        return result.ToHandlerResult();
    }
}
