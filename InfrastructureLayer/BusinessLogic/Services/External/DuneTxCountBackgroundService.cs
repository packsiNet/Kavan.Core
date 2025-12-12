using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace InfrastructureLayer.BusinessLogic.Services.External;

public class DuneTxCountBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<DuneTxCountBackgroundService> _logger;

    public DuneTxCountBackgroundService(IServiceScopeFactory scopeFactory, ILogger<DuneTxCountBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using (var scope = _scopeFactory.CreateScope())
        {
            var service = scope.ServiceProvider.GetRequiredService<DuneTxCountSyncService>();
            await service.SyncLatestAsync(stoppingToken);
        }

        var timer = new PeriodicTimer(TimeSpan.FromHours(24));
        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var service = scope.ServiceProvider.GetRequiredService<DuneTxCountSyncService>();
                    var inserted = await service.SyncLatestAsync(stoppingToken);
                    _logger.LogInformation("Dune tx count sync done, inserted={Inserted}", inserted);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Dune tx count sync error");
                }
            }
        }
        catch (OperationCanceledException)
        {
        }
    }
}
