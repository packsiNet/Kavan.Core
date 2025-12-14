using ApplicationLayer.Extensions;
using ApplicationLayer.Features.BitcoinETF.Query;
using ApplicationLayer.Interfaces.Services.Dune;
using MediatR;

namespace ApplicationLayer.Features.BitcoinETF.Handler;

public class GetLatestBitcoinEtfIssuerFlowsHandler(IDuneEtfIssuerFlowQueryService service) : IRequestHandler<GetLatestBitcoinEtfIssuerFlowsQuery, HandlerResult>
{
    public async Task<HandlerResult> Handle(GetLatestBitcoinEtfIssuerFlowsQuery request, CancellationToken cancellationToken)
    {
        var result = await service.GetLatestAsync(cancellationToken);
        return result.ToHandlerResult();
    }
}
