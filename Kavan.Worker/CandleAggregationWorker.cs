using ApplicationLayer.Interfaces.Services.Candles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Kavan.Worker;

public class CandleAggregationWorker(
    IServiceScopeFactory scopeFactory,
    ILogger<CandleAggregationWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("CandleAggregationWorker started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = scopeFactory.CreateScope())
                {
                    var service = scope.ServiceProvider.GetRequiredService<ICandleAggregatorService>();
                    await service.ExecuteAggregationAsync(stoppingToken);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in CandleAggregationWorker");
            }

            // Wait for 1 minute before next run
            // Align to the next minute to be cleaner?
            // User says "runs every 1 minute".
            // Ideally we wait until the next minute mark, e.g. 00:00, 01:00.
            // But strict 1m delay is also fine.
            // Let's try to align to the start of the next minute for better precision.
            
            var now = DateTime.UtcNow;
            var nextMinute = now.AddMinutes(1).AddSeconds(-now.Second).AddMilliseconds(-now.Millisecond);
            var delay = nextMinute - DateTime.UtcNow;
            if (delay < TimeSpan.Zero) delay = TimeSpan.FromSeconds(1);
            
            await Task.Delay(delay, stoppingToken);
        }
    }
}
