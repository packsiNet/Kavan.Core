using ApplicationLayer.Interfaces;
using ApplicationLayer.Interfaces.Services.Dune;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureLayer.BusinessLogic.Services.Dune;

[InjectAsScoped]
public class DuneGasPriceQueryService : IDuneGasPriceQueryService
{
    private readonly IRepository<DuneGasPriceSnapshot> _repo;

    public DuneGasPriceQueryService(IRepository<DuneGasPriceSnapshot> repo)
    {
        _repo = repo;
    }

    public async Task<List<DuneGasPriceSnapshot>> GetLatestAsync(CancellationToken cancellationToken)
    {
        return await _repo.Query()
            .OrderByDescending(x => x.Time)
            .ToListAsync(cancellationToken);
    }
}
