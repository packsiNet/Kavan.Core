using ApplicationLayer.Interfaces.Services.External;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace InfrastructureLayer.BusinessLogic.Services.External;

public class DuneSyncBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<DuneSyncBackgroundService> _logger;

    public DuneSyncBackgroundService(IServiceScopeFactory scopeFactory, ILogger<DuneSyncBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IDuneSyncService>();
                var inserted = await service.SyncAsync(stoppingToken);
                _logger.LogInformation("Dune sync cycle complete, inserted={Inserted}", inserted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Dune sync error");
            }

            await Task.Delay(TimeSpan.FromHours(6), stoppingToken);
        }
    }
}
