using ApplicationLayer.Interfaces.Binance;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ApplicationLayer.Services;

public class CandleAggregatorHostedService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<CandleAggregatorHostedService> _logger;

    public CandleAggregatorHostedService(IServiceScopeFactory scopeFactory, ILogger<CandleAggregatorHostedService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Candle Aggregator Service started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var aggregator = scope.ServiceProvider.GetRequiredService<ICandleAggregatorService>();
                // Run a basic cycle covering all intervals, adjust if needed
                await aggregator.AggregateCandlesAsync("5m");
                await aggregator.AggregateCandlesAsync("1h");
                await aggregator.AggregateCandlesAsync("4h");
                await aggregator.AggregateCandlesAsync("1d");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CandleAggregatorHostedService loop.");
            }
            finally
            {
                try
                {
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("CandleAggregatorHostedService stopping...");
                }
            }
        }
    }
}
