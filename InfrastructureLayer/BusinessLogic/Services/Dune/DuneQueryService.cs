using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.Dto.BitcoinETF;
using ApplicationLayer.Interfaces;
using ApplicationLayer.Interfaces.Services.Dune;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureLayer.BusinessLogic.Services.Dune;

[InjectAsScoped]
public class DuneQueryService : IDuneQueryService
{
    private readonly IRepository<DuneMetricsSnapshot> _repo;

    public DuneQueryService(IRepository<DuneMetricsSnapshot> repo)
    {
        _repo = repo;
    }

    public async Task<Result<BitcoinEtfMetricsDto>> GetLatestAsync(CancellationToken cancellationToken)
    {
        var entity = await _repo.Query()
            .Where(x => x.IsActive)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (entity == null)
            return Result<BitcoinEtfMetricsDto>.NotFound("No active ETF metrics found");

        var dto = new BitcoinEtfMetricsDto
        {
            ExecutionId = entity.ExecutionId,
            QueryId = entity.QueryId,
            SubmittedAt = entity.SubmittedAt,
            ExpiresAt = entity.ExpiresAt,
            ExecutionStartedAt = entity.ExecutionStartedAt,
            ExecutionEndedAt = entity.ExecutionEndedAt,
            RowCount = entity.RowCount,
            TvlInThousands = entity.TvlInThousands,
            UsdTvlInBillions = entity.UsdTvlInBillions,
            PastWeekFlows = entity.PastWeekFlows,
            FlowsUsdSinceApprovalInThousands = entity.FlowsUsdSinceApprovalInThousands,
            PastWeekFlowsUsdInThousands = entity.PastWeekFlowsUsdInThousands,
            PercentageOfBtc = entity.PercentageOfBtc,
            BtcSupply = entity.BtcSupply,
            SixMonthsAnnualisedImpactOnSupply = entity.SixMonthsAnnualisedImpactOnSupply,
            ThreeMonthsAnnualisedImpactOnSupply = entity.ThreeMonthsAnnualisedImpactOnSupply,
            MonthlyAnnualisedImpactOnSupply = entity.MonthlyAnnualisedImpactOnSupply,
            ByWeeklyAnnualisedImpactOnSupply = entity.ByWeeklyAnnualisedImpactOnSupply,
            WeekAnnualisedImpactOnSupply = entity.WeekAnnualisedImpactOnSupply
        };

        return Result<BitcoinEtfMetricsDto>.Success(dto);
    }
}
