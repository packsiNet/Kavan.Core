using ApplicationLayer.Extensions;
using ApplicationLayer.Features.BitcoinETF.Query;
using ApplicationLayer.Interfaces.Services.Dune;
using MediatR;

namespace ApplicationLayer.Features.BitcoinETF.Handler;

public class GetLatestBitcoinUserCountHandler(IDuneUserCountQueryService service) : IRequestHandler<GetLatestBitcoinUserCountQuery, HandlerResult>
{
    public async Task<HandlerResult> Handle(GetLatestBitcoinUserCountQuery request, CancellationToken cancellationToken)
    {
        var result = await service.GetLatestAsync(cancellationToken);
        return result.ToHandlerResult();
    }
}
