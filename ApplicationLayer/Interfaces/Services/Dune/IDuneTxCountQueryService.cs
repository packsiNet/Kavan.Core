using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.Dto.BitcoinETF;

namespace ApplicationLayer.Interfaces.Services.Dune;

public interface IDuneTxCountQueryService
{
    Task<Result<BitcoinEtfTxCountDto>> GetLatestAsync(CancellationToken cancellationToken);
}
