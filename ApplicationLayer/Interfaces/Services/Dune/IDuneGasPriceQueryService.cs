using DomainLayer.Entities;

namespace ApplicationLayer.Interfaces.Services.Dune;

public interface IDuneGasPriceQueryService
{
    Task<DuneGasPriceSnapshot?> GetLatestAsync(CancellationToken cancellationToken);
}
