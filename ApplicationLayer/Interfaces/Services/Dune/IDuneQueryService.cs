using ApplicationLayer.Dto.BitcoinETF;
using ApplicationLayer.Dto.BaseDtos;

namespace ApplicationLayer.Interfaces.Services.Dune;

public interface IDuneQueryService
{
    Task<Result<BitcoinEtfMetricsDto>> GetLatestAsync(CancellationToken cancellationToken);
}
