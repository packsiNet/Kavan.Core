using DomainLayer.Entities;

namespace ApplicationLayer.Interfaces.Services.Dune;

public interface IDuneGasPriceQueryService
{
    Task<List<DuneGasPriceSnapshot>> GetLatestAsync(CancellationToken cancellationToken);
}
