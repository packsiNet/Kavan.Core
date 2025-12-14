using ApplicationLayer.Interfaces;
using ApplicationLayer.Interfaces.External;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InfrastructureLayer.BusinessLogic.Services.External;

[InjectAsScoped]
public class DuneUserCountSyncService
{
    private readonly IUnitOfWork _uow;
    private readonly IRepository<BitcoinActiveAddress> _repo;
    private readonly IDuneClient _client;
    private readonly ILogger<DuneUserCountSyncService> _logger;

    public DuneUserCountSyncService(IUnitOfWork uow, IRepository<BitcoinActiveAddress> repo, IDuneClient client, ILogger<DuneUserCountSyncService> logger)
    {
        _uow = uow;
        _repo = repo;
        _client = client;
        _logger = logger;
    }

    public async Task<int> SyncLatestAsync(CancellationToken cancellationToken)
    {
        var queryId = 6353378;
        var limit = 2;
        var dto = await _client.GetQueryResultsAsync<ApplicationLayer.Dto.External.Dune.DuneUserCountRow>(queryId, limit, cancellationToken);
        if (dto == null || dto.result.rows.Count == 0)
        {
            _logger.LogWarning("Dune user count results empty");
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

        var parsedRows = dto.result.rows.Select(r => new { Row = r, Time = ParseUtc(r.time) }).ToList();
        if (parsedRows.Count == 0) return 0;
        var latestTime = parsedRows.Max(r => r.Time);

        var times = parsedRows.Select(p => p.Time).ToArray();
        var existing = await _repo.GetDbSet()
            .IgnoreQueryFilters()
            .Where(x => times.Contains(x.Time))
            .ToListAsync(cancellationToken);

        var toUpdate = new List<BitcoinActiveAddress>();
        var toAdd = new List<BitcoinActiveAddress>();
        foreach (var pr in parsedRows)
        {
            var existingForTime = existing.FirstOrDefault(x => x.Time == pr.Time);
            if (existingForTime != null)
            {
                var unchanged =
                    existingForTime.Users == pr.Row.users &&
                    existingForTime.ExecutionId == dto.execution_id &&
                    existingForTime.QueryId == dto.query_id &&
                    existingForTime.SubmittedAt == dto.submitted_at &&
                    existingForTime.ExpiresAt == dto.expires_at &&
                    existingForTime.ExecutionStartedAt == dto.execution_started_at &&
                    existingForTime.ExecutionEndedAt == dto.execution_ended_at &&
                    existingForTime.RowCount == dto.result.metadata.row_count;
                if (unchanged) continue;
                existingForTime.ExecutionId = dto.execution_id;
                existingForTime.QueryId = dto.query_id;
                existingForTime.SubmittedAt = dto.submitted_at;
                existingForTime.ExpiresAt = dto.expires_at;
                existingForTime.ExecutionStartedAt = dto.execution_started_at;
                existingForTime.ExecutionEndedAt = dto.execution_ended_at;
                existingForTime.RowCount = dto.result.metadata.row_count;
                existingForTime.Users = pr.Row.users;
                existingForTime.IsActive = pr.Time == latestTime;
                toUpdate.Add(existingForTime);
            }
            else
            {
                var entity = new BitcoinActiveAddress
                {
                    ExecutionId = dto.execution_id,
                    QueryId = dto.query_id,
                    SubmittedAt = dto.submitted_at,
                    ExpiresAt = dto.expires_at,
                    ExecutionStartedAt = dto.execution_started_at,
                    ExecutionEndedAt = dto.execution_ended_at,
                    RowCount = dto.result.metadata.row_count,
                    Time = pr.Time,
                    Users = pr.Row.users,
                    IsActive = pr.Time == latestTime
                };
                toAdd.Add(entity);
            }
        }

        var otherActives = await _repo.GetDbSet()
            .IgnoreQueryFilters()
            .Where(x => x.IsActive && x.Time != latestTime)
            .ToListAsync(cancellationToken);
        if (otherActives.Count > 0) foreach (var p in otherActives) p.IsActive = false;

        await _uow.BeginTransactionAsync();
        try
        {
            if (toUpdate.Count > 0) await _repo.UpdateRangeAsync(toUpdate);
            if (toAdd.Count > 0) await _repo.AddRangeAsync(toAdd);
            if (otherActives.Count > 0) await _repo.UpdateRangeAsync(otherActives);
            await _uow.SaveChangesAsync(cancellationToken);
            await _uow.CommitAsync();
        }
        catch
        {
            await _uow.RollbackAsync();
            throw;
        }
        var changed = toAdd.Count + toUpdate.Count + otherActives.Count;
        _logger.LogInformation("Dune user count sync applied. Inserted={Inserted} Updated={Updated} Deactivated={Deactivated}", toAdd.Count, toUpdate.Count, otherActives.Count);
        return changed;
    }
}
