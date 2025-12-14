using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.Dto.BitcoinETF;
using ApplicationLayer.Interfaces;
using ApplicationLayer.Interfaces.Services.Dune;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureLayer.BusinessLogic.Services.Dune;

[InjectAsScoped]
public class DuneEtfIssuerFlowQueryService : IDuneEtfIssuerFlowQueryService
{
    private readonly IRepository<DuneEtfIssuerFlowSnapshot> _repo;

    public DuneEtfIssuerFlowQueryService(IRepository<DuneEtfIssuerFlowSnapshot> repo)
    {
        _repo = repo;
    }

    public async Task<Result<List<BitcoinEtfIssuerFlowDto>>> GetLatestAsync(CancellationToken cancellationToken)
    {
        var rows = await _repo.Query()
            .Where(x => x.IsActive)
            .OrderBy(x => x.EtfTicker)
            .Select(x => new BitcoinEtfIssuerFlowDto
            {
                Time = x.Time,
                Issuer = x.Issuer,
                EtfTicker = x.EtfTicker,
                Amount = x.Amount,
                AmountUsd = x.AmountUsd,
                AmountNetFlow = x.AmountNetFlow,
                AmountUsdNetFlow = x.AmountUsdNetFlow
            })
            .ToListAsync(cancellationToken);

        return Result<List<BitcoinEtfIssuerFlowDto>>.Success(rows);
    }
}
