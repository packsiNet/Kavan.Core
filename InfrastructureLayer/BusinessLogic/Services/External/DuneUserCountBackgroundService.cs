using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace InfrastructureLayer.BusinessLogic.Services.External;

public class DuneUserCountBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<DuneUserCountBackgroundService> _logger;

    public DuneUserCountBackgroundService(IServiceScopeFactory scopeFactory, ILogger<DuneUserCountBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var timer = new PeriodicTimer(TimeSpan.FromHours(24));
        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var service = scope.ServiceProvider.GetRequiredService<DuneUserCountSyncService>();
                    var inserted = await service.SyncLatestAsync(stoppingToken);
                    _logger.LogInformation("Dune user count periodic sync done, changed={Inserted}", inserted);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Dune user count periodic sync error");
                }
            }
        }
        catch (OperationCanceledException)
        {
        }
    }
}
