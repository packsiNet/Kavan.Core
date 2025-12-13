using ApplicationLayer.Interfaces.Services.External;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace InfrastructureLayer.BusinessLogic.Services.External;

public class DuneGasPriceBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<DuneGasPriceBackgroundService> _logger;

    public DuneGasPriceBackgroundService(IServiceScopeFactory scopeFactory, ILogger<DuneGasPriceBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using (var scope = _scopeFactory.CreateScope())
        {
            var service = scope.ServiceProvider.GetRequiredService<IDuneGasPriceSyncService>();
            await service.SyncAsync(stoppingToken);
        }

        var timer = new PeriodicTimer(TimeSpan.FromMinutes(5));
        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var service = scope.ServiceProvider.GetRequiredService<IDuneGasPriceSyncService>();
                    var count = await service.SyncAsync(stoppingToken);
                    _logger.LogInformation("Dune gas price sync cycle inserted/updated={Count}", count);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Dune gas price sync error");
                }
            }
        }
        catch (OperationCanceledException)
        {
        }
    }
}
