using ApplicationLayer.Interfaces;
using ApplicationLayer.Interfaces.External;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InfrastructureLayer.BusinessLogic.Services.External;

[InjectAsScoped]
public class DuneEtfIssuerFlowSyncService
{
    private readonly IUnitOfWork _uow;
    private readonly IRepository<DuneEtfIssuerFlowSnapshot> _repo;
    private readonly IDuneClient _client;
    private readonly ILogger<DuneEtfIssuerFlowSyncService> _logger;

    public DuneEtfIssuerFlowSyncService(IUnitOfWork uow, IRepository<DuneEtfIssuerFlowSnapshot> repo, IDuneClient client, ILogger<DuneEtfIssuerFlowSyncService> logger)
    {
        _uow = uow;
        _repo = repo;
        _client = client;
        _logger = logger;
    }

    public async Task<int> SyncLatestAsync(CancellationToken cancellationToken)
    {
        var queryId = 6350521;
        var dto = await _client.GetQueryResultsAsync<ApplicationLayer.Dto.External.Dune.DuneEtfIssuerFlowRow>(queryId, 1000, cancellationToken);
        if (dto == null || dto.result.rows.Count == 0)
        {
            _logger.LogWarning("Dune ETF issuer flows results empty");
            return 0;
        }

        static DateTime ParseUtc(string s)
        {
            if (DateTime.TryParse(s, out var dt)) return DateTime.SpecifyKind(dt, DateTimeKind.Utc);
            var formats = new[] { "yyyy-MM-dd HH:mm:ss.fff 'UTC'", "yyyy-MM-dd HH:mm:ss 'UTC'" };
            if (DateTime.TryParseExact(s, formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal, out var dte))
                return DateTime.SpecifyKind(dte, DateTimeKind.Utc);
            return DateTime.UtcNow;
        }

        var inserted = 0;
        foreach (var row in dto.result.rows)
        {
            var time = ParseUtc(row.time);
            var existing = await _repo.GetDbSet()
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(x => x.Time == time && x.EtfTicker == row.etf_ticker, cancellationToken);

            if (existing != null)
            {
                var unchanged =
                    existing.Issuer == row.issuer &&
                    existing.Amount == row.amount &&
                    existing.AmountUsd == row.amount_usd &&
                    existing.AmountNetFlow == row.amount_net_flow &&
                    existing.AmountUsdNetFlow == row.amount_usd_net_flow &&
                    existing.ExecutionId == dto.execution_id &&
                    existing.QueryId == dto.query_id &&
                    existing.SubmittedAt == dto.submitted_at &&
                    existing.ExpiresAt == dto.expires_at &&
                    existing.ExecutionStartedAt == dto.execution_started_at &&
                    existing.ExecutionEndedAt == dto.execution_ended_at &&
                    existing.RowCount == dto.result.metadata.row_count;

                if (unchanged)
                    continue;

                existing.ExecutionId = dto.execution_id;
                existing.QueryId = dto.query_id;
                existing.SubmittedAt = dto.submitted_at;
                existing.ExpiresAt = dto.expires_at;
                existing.ExecutionStartedAt = dto.execution_started_at;
                existing.ExecutionEndedAt = dto.execution_ended_at;
                existing.RowCount = dto.result.metadata.row_count;
                existing.Issuer = row.issuer;
                existing.EtfTicker = row.etf_ticker;
                existing.Amount = row.amount;
                existing.AmountUsd = row.amount_usd;
                existing.AmountNetFlow = row.amount_net_flow;
                existing.AmountUsdNetFlow = row.amount_usd_net_flow;
                existing.IsActive = true;

                await _repo.UpdateAsync(existing);
                await _uow.SaveChangesAsync(cancellationToken);
                inserted++;
                continue;
            }

            var entity = new DuneEtfIssuerFlowSnapshot
            {
                ExecutionId = dto.execution_id,
                QueryId = dto.query_id,
                SubmittedAt = dto.submitted_at,
                ExpiresAt = dto.expires_at,
                ExecutionStartedAt = dto.execution_started_at,
                ExecutionEndedAt = dto.execution_ended_at,
                RowCount = dto.result.metadata.row_count,
                Time = time,
                Issuer = row.issuer,
                EtfTicker = row.etf_ticker,
                Amount = row.amount,
                AmountUsd = row.amount_usd,
                AmountNetFlow = row.amount_net_flow,
                AmountUsdNetFlow = row.amount_usd_net_flow
            };

            await _repo.AddAsync(entity);
            await _uow.SaveChangesAsync(cancellationToken);
            inserted++;
        }

        return inserted;
    }
}
