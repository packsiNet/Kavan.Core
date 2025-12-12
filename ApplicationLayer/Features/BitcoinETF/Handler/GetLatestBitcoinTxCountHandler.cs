using ApplicationLayer.Extensions;
using ApplicationLayer.Features.BitcoinETF.Query;
using ApplicationLayer.Interfaces.Services.Dune;
using MediatR;

namespace ApplicationLayer.Features.BitcoinETF.Handler;

public class GetLatestBitcoinTxCountHandler(IDuneTxCountQueryService service) : IRequestHandler<GetLatestBitcoinTxCountQuery, HandlerResult>
{
    public async Task<HandlerResult> Handle(GetLatestBitcoinTxCountQuery request, CancellationToken cancellationToken)
    {
        var result = await service.GetLatestAsync(cancellationToken);
        return result.ToHandlerResult();
    }
}
