using ApplicationLayer.Interfaces;
using ApplicationLayer.Interfaces.External;
using ApplicationLayer.Interfaces.Services.External;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InfrastructureLayer.BusinessLogic.Services.External;

[InjectAsScoped]
public class DuneSyncService : IDuneSyncService
{
    private readonly IUnitOfWork _uow;
    private readonly IRepository<DuneMetricsSnapshot> _repo;
    private readonly IDuneClient _client;
    private readonly ILogger<DuneSyncService> _logger;

    public DuneSyncService(IUnitOfWork uow, IRepository<DuneMetricsSnapshot> repo, IDuneClient client, ILogger<DuneSyncService> logger)
    {
        _uow = uow;
        _repo = repo;
        _client = client;
        _logger = logger;
    }

    public async Task<int> SyncAsync(CancellationToken cancellationToken)
    {
        var queryId = 6344827;

        var dto = await _client.GetQueryResultsAsync<ApplicationLayer.Dto.External.Dune.DuneMetricsRow>(queryId, 1, cancellationToken);
        if (dto == null || dto.result.rows.Count == 0)
        {
            _logger.LogWarning("Dune results empty");
            return 0;
        }

        var row = dto.result.rows[0];

        var latest = await _repo.Query()
            .Where(x => x.QueryId == queryId)
            .OrderByDescending(x => x.SubmittedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (latest != null &&
            latest.ExecutionId == dto.execution_id &&
            latest.QueryId == dto.query_id &&
            latest.SubmittedAt == dto.submitted_at &&
            latest.ExpiresAt == dto.expires_at &&
            latest.ExecutionStartedAt == dto.execution_started_at &&
            latest.ExecutionEndedAt == dto.execution_ended_at &&
            latest.RowCount == dto.result.metadata.row_count &&
            latest.TvlInThousands == row.tvl_in_thousands &&
            latest.UsdTvlInBillions == row.usd_tvl_in_billions &&
            latest.PastWeekFlows == row.past_week_flows &&
            latest.FlowsUsdSinceApprovalInThousands == row.flows_usd_since_approval_in_thousands &&
            latest.PastWeekFlowsUsdInThousands == row.past_week_flows_usd_in_thousands &&
            latest.PercentageOfBtc == row.percentage_of_btc &&
            latest.BtcSupply == row.btc_supply &&
            latest.SixMonthsAnnualisedImpactOnSupply == row.sixmonths_annualised_impact_on_supply &&
            latest.ThreeMonthsAnnualisedImpactOnSupply == row.threemonths_annualised_impact_on_supply &&
            latest.MonthlyAnnualisedImpactOnSupply == row.monthly_annualised_impact_on_supply &&
            latest.ByWeeklyAnnualisedImpactOnSupply == row.byweekly_annualised_impact_on_supply &&
            latest.WeekAnnualisedImpactOnSupply == row.week_annualised_impact_on_supply)
        {
            _logger.LogInformation("Dune metrics unchanged, skip insert");
            return 0;
        }

        var entity = new DuneMetricsSnapshot
        {
            ExecutionId = dto.execution_id,
            QueryId = dto.query_id,
            SubmittedAt = dto.submitted_at,
            ExpiresAt = dto.expires_at,
            ExecutionStartedAt = dto.execution_started_at,
            ExecutionEndedAt = dto.execution_ended_at,
            RowCount = dto.result.metadata.row_count,

            TvlInThousands = row.tvl_in_thousands,
            UsdTvlInBillions = row.usd_tvl_in_billions,
            PastWeekFlows = row.past_week_flows,
            FlowsUsdSinceApprovalInThousands = row.flows_usd_since_approval_in_thousands,
            PastWeekFlowsUsdInThousands = row.past_week_flows_usd_in_thousands,
            PercentageOfBtc = row.percentage_of_btc,
            BtcSupply = row.btc_supply,
            SixMonthsAnnualisedImpactOnSupply = row.sixmonths_annualised_impact_on_supply,
            ThreeMonthsAnnualisedImpactOnSupply = row.threemonths_annualised_impact_on_supply,
            MonthlyAnnualisedImpactOnSupply = row.monthly_annualised_impact_on_supply,
            ByWeeklyAnnualisedImpactOnSupply = row.byweekly_annualised_impact_on_supply,
            WeekAnnualisedImpactOnSupply = row.week_annualised_impact_on_supply,
        };

        await _uow.BeginTransactionAsync();
        try
        {
            await _repo.AddAsync(entity);
            await _uow.SaveChangesAsync(cancellationToken);

            var previousActives = await _repo.Query()
                .Where(x => x.QueryId == queryId && x.IsActive && x.Id != entity.Id)
                .ToListAsync(cancellationToken);
            if (previousActives.Count > 0)
            {
                foreach (var p in previousActives) p.IsActive = false;
                await _repo.UpdateRangeAsync(previousActives);
                await _uow.SaveChangesAsync(cancellationToken);
            }

            await _uow.CommitAsync();
            _logger.LogInformation("Dune metrics inserted: ExecutionId={ExecutionId}", entity.ExecutionId);
            return 1;
        }
        catch
        {
            await _uow.RollbackAsync();
            throw;
        }
    }
}
