using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.Dto.BitcoinETF;
using ApplicationLayer.Interfaces;
using ApplicationLayer.Interfaces.Services.Dune;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureLayer.BusinessLogic.Services.Dune;

[InjectAsScoped]
public class DuneUserCountQueryService : IDuneUserCountQueryService
{
    private readonly IRepository<BitcoinActiveAddress> _repo;

    public DuneUserCountQueryService(IRepository<BitcoinActiveAddress> repo)
    {
        _repo = repo;
    }

    public async Task<Result<BitcoinEtfUserCountDto>> GetLatestAsync(CancellationToken cancellationToken)
    {
        var entity = await _repo.Query()
            .Where(x => x.IsActive)
            .OrderByDescending(x => x.Time)
            .FirstOrDefaultAsync(cancellationToken);

        if (entity == null)
            return Result<BitcoinEtfUserCountDto>.NotFound("No active user count snapshot found");

        var dto = new BitcoinEtfUserCountDto
        {
            Time = entity.Time,
            Users = entity.Users
        };

        return Result<BitcoinEtfUserCountDto>.Success(dto);
    }
}
