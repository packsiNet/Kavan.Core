using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.Dto.BitcoinETF;

namespace ApplicationLayer.Interfaces.Services.Dune;

public interface IDuneEtfIssuerFlowQueryService
{
    Task<Result<List<BitcoinEtfIssuerFlowDto>>> GetLatestAsync(CancellationToken cancellationToken);
}
