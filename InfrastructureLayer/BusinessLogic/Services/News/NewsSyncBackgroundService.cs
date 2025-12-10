using ApplicationLayer.Dto.News;
using ApplicationLayer.Interfaces.Services.News;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace InfrastructureLayer.BusinessLogic.Services.News;

public class NewsSyncBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<NewsSyncBackgroundService> _logger;

    public NewsSyncBackgroundService(IServiceScopeFactory scopeFactory, ILogger<NewsSyncBackgroundService> logger)
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
                var service = scope.ServiceProvider.GetRequiredService<INewsSyncService>();
                var query = new CryptoPanicQuery { Public = true };
                await service.SyncLatestAsync(query, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "News sync error");
            }

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}
