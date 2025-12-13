using ApplicationLayer.Interfaces.Services.Dune;
using DomainLayer.Entities;
using MediatR;

namespace ApplicationLayer.Features.DuneGasPrice.Handler;

public class GetLatestGasPriceHandler : IRequestHandler<Query.GetLatestGasPriceQuery, DuneGasPriceSnapshot?>
{
    private readonly IDuneGasPriceQueryService _service;

    public GetLatestGasPriceHandler(IDuneGasPriceQueryService service)
    {
        _service = service;
    }

    public async Task<DuneGasPriceSnapshot> Handle(Query.GetLatestGasPriceQuery request, CancellationToken cancellationToken)
    {
        return await _service.GetLatestAsync(cancellationToken);
    }
}
