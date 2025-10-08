using ApplicationLayer.Interfaces.Binance;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ApplicationLayer.Services;

public class CandleUpdaterHostedService(IServiceScopeFactory _scopeFactory, ILogger<CandleUpdaterHostedService> _logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Candle updater background service started.");
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var symbolSyncService = scope.ServiceProvider.GetRequiredService<IBinanceSymbolSyncService>();
                var candleUpdater = scope.ServiceProvider.GetRequiredService<ICandleUpdaterService>();

                var symbols = await symbolSyncService.GetActiveSymbolsAsync();
                _logger.LogInformation("Found {Count} active symbols for update.", symbols.Count);

                foreach (var symbol in symbols)
                {
                    _logger.LogInformation("Updating candles for {Symbol} (1m)...", symbol);
                    await candleUpdater.UpdateCandlesAsync(symbol, "1m", stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in CandleUpdaterHostedService loop.");
            }
            finally
            {
                try
                {
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
                catch (OperationCanceledException) { }
            }
        }
    }
}