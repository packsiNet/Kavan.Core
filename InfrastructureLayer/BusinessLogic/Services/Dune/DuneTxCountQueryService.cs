using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.Dto.BitcoinETF;
using ApplicationLayer.Interfaces;
using ApplicationLayer.Interfaces.Services.Dune;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureLayer.BusinessLogic.Services.Dune;

[InjectAsScoped]
public class DuneTxCountQueryService : IDuneTxCountQueryService
{
    private readonly IRepository<DuneDailyTxCountSnapshot> _repo;

    public DuneTxCountQueryService(IRepository<DuneDailyTxCountSnapshot> repo)
    {
        _repo = repo;
    }

    public async Task<Result<BitcoinEtfTxCountDto>> GetLatestAsync(CancellationToken cancellationToken)
    {
        var entity = await _repo.Query()
            .Where(x => x.IsActive)
            .OrderByDescending(x => x.Time)
            .FirstOrDefaultAsync(cancellationToken);

        if (entity == null)
            return Result<BitcoinEtfTxCountDto>.NotFound("No active tx count snapshot found");

        var dto = new BitcoinEtfTxCountDto
        {
            Time = entity.Time,
            TxCount = entity.TxCount,
            TxCountMovingAverage = entity.TxCountMovingAverage
        };

        return Result<BitcoinEtfTxCountDto>.Success(dto);
    }
}
