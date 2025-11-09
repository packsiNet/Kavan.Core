using ApplicationLayer.Interfaces;
using ApplicationLayer.Interfaces.Services;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace InfrastructureLayer.BusinessLogic.Services.Binance;

public class BinanceKlineWebSocketHostedService(
    IServiceScopeFactory scopeFactory,
    ILogger<BinanceKlineWebSocketHostedService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Starting Binance Kline WebSocket hosted service");

        using var scope = scopeFactory.CreateScope();
        var cryptoRepo = scope.ServiceProvider.GetRequiredService<IRepository<Cryptocurrency>>();

        var symbols = await cryptoRepo.GetDbSet()
            .AsNoTracking()
            .Where(c => c.IsActive)
            .ToListAsync(stoppingToken);

        if (symbols.Count == 0)
        {
            logger.LogWarning("No active cryptocurrencies found. WS service will idle.");
            return;
        }

        logger.LogInformation("Launching WS streams for {Count} symbols", symbols.Count);

        var tasks = new List<Task>(symbols.Count);
        foreach (var crypto in symbols)
        {
            tasks.Add(Task.Run(async () =>
            {
                using var symbolScope = scopeFactory.CreateScope();
                var scopedWsService = symbolScope.ServiceProvider.GetRequiredService<IKlineWebSocketService>();
                await scopedWsService.RunStreamsForSymbolAsync(crypto, stoppingToken);
            }, stoppingToken));
        }

        try
        {
            await Task.WhenAll(tasks);
        }
        catch (OperationCanceledException)
        {
            // graceful stop
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while running WS streams");
        }
    }
}