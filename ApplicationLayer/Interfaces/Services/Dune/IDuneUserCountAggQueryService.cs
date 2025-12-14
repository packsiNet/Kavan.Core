using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.Dto.BitcoinETF;

namespace ApplicationLayer.Interfaces.Services.Dune;

public interface IDuneUserCountAggQueryService
{
    Task<Result<BitcoinUserCountsResponseDto>> GetAggregatedAsync(CancellationToken cancellationToken);
}
