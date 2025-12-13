using System.Text.Json;
using ApplicationLayer.Interfaces;
using ApplicationLayer.Interfaces.External;
using ApplicationLayer.Interfaces.Services.External;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InfrastructureLayer.BusinessLogic.Services.External;

[InjectAsScoped]
public class DuneGasPriceSyncService : IDuneGasPriceSyncService
{
    private readonly IUnitOfWork _uow;
    private readonly IRepository<DuneGasPriceSnapshot> _repo;
    private readonly IDuneClient _client;
    private readonly ILogger<DuneGasPriceSyncService> _logger;

    public DuneGasPriceSyncService(IUnitOfWork uow, IRepository<DuneGasPriceSnapshot> repo, IDuneClient client, ILogger<DuneGasPriceSyncService> logger)
    {
        _uow = uow;
        _repo = repo;
        _client = client;
        _logger = logger;
    }

    public async Task<int> SyncAsync(CancellationToken cancellationToken)
    {
        var queryId = 6349332;
        var dto = await _client.GetQueryResultsAsync<Dictionary<string, JsonElement>>(queryId, 10, cancellationToken);
        if (dto == null || dto.result.rows.Count == 0)
        {
            _logger.LogWarning("Dune gas price results empty");
            return 0;
        }

        static DateTime ParseUtc(string s)
        {
            if (DateTime.TryParse(s, out var dt)) return DateTime.SpecifyKind(dt, DateTimeKind.Utc);
            var formats = new[] { "yyyy-MM-dd HH:mm:ss.fff 'UTC'", "yyyy-MM-dd HH:mm:ss 'UTC'", "yyyy-MM-dd'T'HH:mm:ss'Z'" };
            if (DateTime.TryParseExact(s, formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal, out var dte))
                return DateTime.SpecifyKind(dte, DateTimeKind.Utc);
            return DateTime.UtcNow;
        }

        int insertedOrUpdated = 0;

        foreach (var row in dto.result.rows)
        {
            string TryGetString(Dictionary<string, JsonElement> r, params string[] keys)
            {
                foreach (var k in keys)
                {
                    if (r.TryGetValue(k, out var v) && v.ValueKind == JsonValueKind.String) return v.GetString();
                }
                return null;
            }

            decimal TryGetDecimal(Dictionary<string, JsonElement> r, params string[] keys)
            {
                foreach (var k in keys)
                {
                    if (r.TryGetValue(k, out var v))
                    {
                        if (v.ValueKind == JsonValueKind.Number)
                        {
                            try { return v.GetDecimal(); } catch { try { return (decimal)v.GetDouble(); } catch { } }
                        }
                        if (v.ValueKind == JsonValueKind.String)
                        {
                            var s = v.GetString();
                            if (decimal.TryParse(s, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var ds)) return ds;
                        }
                    }
                }
                return 0m;
            }

            decimal TryGetDecimalByPattern(Dictionary<string, JsonElement> r, params string[] patterns)
            {
                foreach (var kv in r)
                {
                    var key = kv.Key.ToLowerInvariant();
                    if (patterns.Any(p => key.Contains(p)))
                    {
                        var v = kv.Value;
                        if (v.ValueKind == JsonValueKind.Number)
                        {
                            try { return v.GetDecimal(); } catch { try { return (decimal)v.GetDouble(); } catch { } }
                        }
                        if (v.ValueKind == JsonValueKind.String)
                        {
                            var s = v.GetString();
                            if (decimal.TryParse(s, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var ds)) return ds;
                        }
                    }
                }
                return 0m;
            }

            var timeStr = TryGetString(row, "time", "timestamp", "date");
            if (string.IsNullOrWhiteSpace(timeStr)) continue;
            var time = ParseUtc(timeStr);

            var medianGwei = TryGetDecimal(row, "median_gas_price_gwei", "median_gas_price", "median_gwei");
            if (medianGwei == 0m)
                medianGwei = TryGetDecimalByPattern(row, "median", "gwei", "gas_price");

            var transferUsd = TryGetDecimal(row, "eth_transfer_price_usd", "eth_transfer_cost_usd", "eth_transfer_gas_cost_usd");
            if (transferUsd == 0m)
                transferUsd = TryGetDecimalByPattern(row, "transfer", "usd", "gas_cost", "price_usd");

            if (medianGwei == 0m && transferUsd == 0m)
            {
                _logger.LogWarning("DuneGasPrice: failed to parse decimals for time={Time}. Keys={Keys}", time, string.Join(',', row.Keys));
            }

            var existing = await _repo.GetDbSet()
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(x => x.Time == time, cancellationToken);

            if (existing != null)
            {
                var unchanged =
                    existing.MedianGasPriceGwei == medianGwei &&
                    existing.EthTransferPriceUsd == transferUsd &&
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
                existing.MedianGasPriceGwei = medianGwei;
                existing.EthTransferPriceUsd = transferUsd;
                await _repo.UpdateAsync(existing);
                await _uow.SaveChangesAsync(cancellationToken);
                insertedOrUpdated++;
                continue;
            }

            var entity = new DuneGasPriceSnapshot
            {
                ExecutionId = dto.execution_id,
                QueryId = dto.query_id,
                SubmittedAt = dto.submitted_at,
                ExpiresAt = dto.expires_at,
                ExecutionStartedAt = dto.execution_started_at,
                ExecutionEndedAt = dto.execution_ended_at,
                RowCount = dto.result.metadata.row_count,
                Time = time,
                MedianGasPriceGwei = medianGwei,
                EthTransferPriceUsd = transferUsd
            };

            await _repo.AddAsync(entity);
            await _uow.SaveChangesAsync(cancellationToken);
            insertedOrUpdated++;
        }

        return insertedOrUpdated;
    }
}
