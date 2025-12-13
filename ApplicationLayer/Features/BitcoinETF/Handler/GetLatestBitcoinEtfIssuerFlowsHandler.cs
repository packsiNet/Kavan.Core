using ApplicationLayer;
using ApplicationLayer.Extensions;
using ApplicationLayer.Features.BitcoinETF.Query;
using ApplicationLayer.Interfaces.Services.Dune;
using MediatR;

namespace ApplicationLayer.Features.BitcoinETF.Handler;

public class GetLatestBitcoinEtfIssuerFlowsHandler : IRequestHandler<GetLatestBitcoinEtfIssuerFlowsQuery, HandlerResult>
{
    private readonly IDuneEtfIssuerFlowQueryService _service;

    public GetLatestBitcoinEtfIssuerFlowsHandler(IDuneEtfIssuerFlowQueryService service)
    {
        _service = service;
    }

    public async Task<HandlerResult> Handle(GetLatestBitcoinEtfIssuerFlowsQuery request, CancellationToken cancellationToken)
    {
        var result = await _service.GetLatestAsync(cancellationToken);
        return result.ToHandlerResult();
    }
}
