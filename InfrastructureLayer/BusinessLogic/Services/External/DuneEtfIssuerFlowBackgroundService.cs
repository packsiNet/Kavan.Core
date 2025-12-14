using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace InfrastructureLayer.BusinessLogic.Services.External;

public class DuneEtfIssuerFlowBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<DuneEtfIssuerFlowBackgroundService> _logger;

    public DuneEtfIssuerFlowBackgroundService(IServiceScopeFactory scopeFactory, ILogger<DuneEtfIssuerFlowBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using (var scope = _scopeFactory.CreateScope())
        {
            var service = scope.ServiceProvider.GetRequiredService<DuneEtfIssuerFlowSyncService>();
            await service.SyncLatestAsync(stoppingToken);
        }

        var timer = new PeriodicTimer(TimeSpan.FromHours(12));
        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var service = scope.ServiceProvider.GetRequiredService<DuneEtfIssuerFlowSyncService>();
                    var inserted = await service.SyncLatestAsync(stoppingToken);
                    _logger.LogInformation("Dune ETF issuer flows sync done, changed_rows={Inserted}", inserted);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Dune ETF issuer flows sync error");
                }
            }
        }
        catch (OperationCanceledException)
        {
        }
    }
}
