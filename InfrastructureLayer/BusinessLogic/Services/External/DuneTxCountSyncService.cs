using ApplicationLayer.Interfaces;
using ApplicationLayer.Interfaces.External;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InfrastructureLayer.BusinessLogic.Services.External;

[InjectAsScoped]
public class DuneTxCountSyncService
{
    private readonly IUnitOfWork _uow;
    private readonly IRepository<DuneDailyTxCountSnapshot> _repo;
    private readonly IDuneClient _client;
    private readonly ILogger<DuneTxCountSyncService> _logger;

    public DuneTxCountSyncService(IUnitOfWork uow, IRepository<DuneDailyTxCountSnapshot> repo, IDuneClient client, ILogger<DuneTxCountSyncService> logger)
    {
        _uow = uow;
        _repo = repo;
        _client = client;
        _logger = logger;
    }

        public async Task<int> SyncLatestAsync(CancellationToken cancellationToken)
        {
            var queryId = 6345954;
            var dto = await _client.GetQueryResultsAsync<ApplicationLayer.Dto.External.Dune.DuneTxCountRow>(queryId, 3, cancellationToken);
            if (dto == null || dto.result.rows.Count == 0)
            {
                _logger.LogWarning("Dune tx count results empty");
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

        var latestRow = dto.result.rows.OrderByDescending(r => ParseUtc(r.time)).First();
        var latestTime = ParseUtc(latestRow.time);

        var existingForTime = await _repo.GetDbSet()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Time == latestTime, cancellationToken);

        if (existingForTime != null)
        {
            var unchanged =
                existingForTime.TxCount == latestRow.tx_count &&
                existingForTime.TxCountMovingAverage == latestRow.tx_count_moving_average &&
                existingForTime.ExecutionId == dto.execution_id &&
                existingForTime.QueryId == dto.query_id &&
                existingForTime.SubmittedAt == dto.submitted_at &&
                existingForTime.ExpiresAt == dto.expires_at &&
                existingForTime.ExecutionStartedAt == dto.execution_started_at &&
                existingForTime.ExecutionEndedAt == dto.execution_ended_at &&
                existingForTime.RowCount == dto.result.metadata.row_count;

            if (unchanged)
            {
                _logger.LogInformation("Dune tx count unchanged, skip update");
                return 0;
            }

            existingForTime.ExecutionId = dto.execution_id;
            existingForTime.QueryId = dto.query_id;
            existingForTime.SubmittedAt = dto.submitted_at;
            existingForTime.ExpiresAt = dto.expires_at;
            existingForTime.ExecutionStartedAt = dto.execution_started_at;
            existingForTime.ExecutionEndedAt = dto.execution_ended_at;
            existingForTime.RowCount = dto.result.metadata.row_count;
            existingForTime.TxCount = latestRow.tx_count;
            existingForTime.TxCountMovingAverage = latestRow.tx_count_moving_average;
            existingForTime.IsActive = true;

            var otherActives = await _repo.GetDbSet()
                .IgnoreQueryFilters()
                .Where(x => x.IsActive && x.Id != existingForTime.Id)
                .ToListAsync(cancellationToken);
            if (otherActives.Count > 0)
            {
                foreach (var p in otherActives) p.IsActive = false;
                await _repo.UpdateRangeAsync(otherActives);
            }

            await _repo.UpdateAsync(existingForTime);
            await _uow.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Dune tx count updated: Time={Time}", existingForTime.Time);
            return 1;
        }

        var previousActives = await _repo.Query()
            .Where(x => x.IsActive)
            .ToListAsync(cancellationToken);
        if (previousActives.Count > 0)
        {
            foreach (var p in previousActives) p.IsActive = false;
            await _repo.UpdateRangeAsync(previousActives);
            await _uow.SaveChangesAsync(cancellationToken);
        }

        var entity = new DuneDailyTxCountSnapshot
        {
            ExecutionId = dto.execution_id,
            QueryId = dto.query_id,
            SubmittedAt = dto.submitted_at,
            ExpiresAt = dto.expires_at,
            ExecutionStartedAt = dto.execution_started_at,
            ExecutionEndedAt = dto.execution_ended_at,
            RowCount = dto.result.metadata.row_count,
            Time = latestTime,
            TxCount = latestRow.tx_count,
            TxCountMovingAverage = latestRow.tx_count_moving_average
        };

        await _repo.AddAsync(entity);
        await _uow.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Dune tx count inserted: Time={Time}", entity.Time);
        return 1;
        }
    }
