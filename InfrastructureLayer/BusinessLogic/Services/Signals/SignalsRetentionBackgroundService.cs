using DomainLayer.Entities;
using InfrastructureLayer.Configuration;
using InfrastructureLayer.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace InfrastructureLayer.BusinessLogic.Services.Signals;

public class SignalsRetentionBackgroundService(IServiceScopeFactory scopeFactory, IOptions<SignalRetentionOptions> options, ILogger<SignalsRetentionBackgroundService> logger) : BackgroundService
{
    private readonly SemaphoreSlim _gate = new(1, 1);
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var cfg = options.Value;
        if (!cfg.Enabled)
        {
            logger.LogInformation("SignalsRetentionBackgroundService disabled by configuration.");
            return;
        }

        logger.LogInformation("SignalsRetentionBackgroundService started. Interval: {IntervalHours}h", cfg.IntervalHours);
        var timer = new PeriodicTimer(TimeSpan.FromHours(Math.Max(cfg.IntervalHours, 1)));
        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                if (!await _gate.WaitAsync(0, stoppingToken))
                {
                    logger.LogInformation("Previous retention run still active; skipping this tick.");
                    continue;
                }
                try
                {
                    using var scope = scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var excludeCats = cfg.ExcludeCategories ?? new List<string>();
                    var excludeNames = cfg.ExcludeSignalNames ?? new List<string>();

                    async Task<int> PurgeAsync(string timeframe, int days)
                    {
                        var cutoff = DateTime.UtcNow.AddDays(-Math.Max(days, 0));
                        var q = db.Set<Signal>().Where(s => s.Timeframe == timeframe && s.SignalTime < cutoff);
                        if (excludeCats.Count > 0) q = q.Where(s => !excludeCats.Contains(s.SignalCategory));
                        if (excludeNames.Count > 0) q = q.Where(s => !excludeNames.Contains(s.SignalName));
                        var count = await q.CountAsync(stoppingToken);
                        if (count == 0) return 0;
                        var deleted = await q.ExecuteDeleteAsync(stoppingToken);
                        return deleted;
                    }

                    var d1m = await PurgeAsync("1m", cfg.RetentionDays_1m);
                    var d5m = await PurgeAsync("5m", cfg.RetentionDays_5m);
                    var d1h = await PurgeAsync("1h", cfg.RetentionDays_1h);
                    var d4h = await PurgeAsync("4h", cfg.RetentionDays_4h);
                    var d1d = await PurgeAsync("1d", cfg.RetentionDays_1d);
                    var total = d1m + d5m + d1h + d4h + d1d;

                    var status = scope.ServiceProvider.GetRequiredService<ApplicationLayer.Interfaces.Services.Signals.ISignalsRetentionStatusService>();
                    status.Update(DateTime.UtcNow, d1m, d5m, d1h, d4h, d1d);

                    logger.LogInformation("Signals retention completed. Deleted total: {Total}", total);
                }
                catch (Exception ex)
                {
                    try
                    {
                        using var scope2 = scopeFactory.CreateScope();
                        var status = scope2.ServiceProvider.GetRequiredService<ApplicationLayer.Interfaces.Services.Signals.ISignalsRetentionStatusService>();
                        status.Update(DateTime.UtcNow, 0, 0, 0, 0, 0, ex.Message);
                    }
                    catch { }
                    logger.LogError(ex, "Error during signals retention run");
                }
                finally
                {
                    _gate.Release();
                }
            }
        }
        catch (OperationCanceledException)
        {
            // graceful shutdown
        }
        finally
        {
            logger.LogInformation("SignalsRetentionBackgroundService stopped.");
        }
    }
}