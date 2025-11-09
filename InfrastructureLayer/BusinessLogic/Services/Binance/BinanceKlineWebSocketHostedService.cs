using ApplicationLayer.Interfaces;
using ApplicationLayer.Interfaces.Services;
using ApplicationLayer.Interfaces;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace InfrastructureLayer.BusinessLogic.Services.Binance;

public class BinanceKlineWebSocketHostedService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<BinanceKlineWebSocketHostedService> _logger;

    public BinanceKlineWebSocketHostedService(
        IServiceScopeFactory scopeFactory,
        ILogger<BinanceKlineWebSocketHostedService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting Binance Kline WebSocket hosted service");

        using var scope = _scopeFactory.CreateScope();
        var cryptoRepo = scope.ServiceProvider.GetRequiredService<IRepository<Cryptocurrency>>();

        var symbols = await cryptoRepo.GetDbSet()
            .AsNoTracking()
            .Where(c => c.IsActive)
            .ToListAsync(stoppingToken);

        if (symbols.Count == 0)
        {
            _logger.LogWarning("No active cryptocurrencies found. WS service will idle.");
            return;
        }

        _logger.LogInformation("Launching WS streams for {Count} symbols", symbols.Count);

        var tasks = new List<Task>(symbols.Count);
        foreach (var crypto in symbols)
        {
            tasks.Add(Task.Run(async () =>
            {
                using var symbolScope = _scopeFactory.CreateScope();
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
            _logger.LogError(ex, "Error while running WS streams");
        }
    }
}