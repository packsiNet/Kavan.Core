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

    public async Task<Result<List<BitcoinEtfIssuerFlowDto>>> GetLatestAsync(
        CancellationToken cancellationToken)
    {
        var fromDate = DateTime.UtcNow.Date.AddYears(-1);

        var rows = await _repo.Query()
            .Where(x => x.IsActive && x.Time >= fromDate)
            .OrderBy(x => x.Time)
            .ThenBy(x => x.EtfTicker)
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

        if (!rows.Any())
            return Result<List<BitcoinEtfIssuerFlowDto>>
                .NotFound("No ETF issuer flows found for last year");

        return Result<List<BitcoinEtfIssuerFlowDto>>.Success(rows);
    }
}
