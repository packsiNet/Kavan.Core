using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.Dto.BitcoinETF;

namespace ApplicationLayer.Interfaces.Services.Dune;

public interface IDuneUserCountQueryService
{
    Task<Result<BitcoinEtfUserCountDto>> GetLatestAsync(CancellationToken cancellationToken);
}
