using ApplicationLayer.Interfaces.Binance;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ApplicationLayer.Services;

public class CandleUpdaterHostedService(
    IServiceScopeFactory _scopeFactory,
    ILogger<CandleUpdaterHostedService> _logger
) : BackgroundService
{
    private DateTime _last5mUpdate = DateTime.MinValue;
    private DateTime _last1hUpdate = DateTime.MinValue;
    private DateTime _last4hUpdate = DateTime.MinValue;
    private DateTime _last1dUpdate = DateTime.MinValue;
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("🚀 CandleUpdaterHostedService started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // ✅ Scope جدید برای هر iteration
                using var scope = _scopeFactory.CreateScope();

                var symbolSyncService = scope.ServiceProvider.GetRequiredService<IBinanceSymbolSyncService>();
                var candleUpdater = scope.ServiceProvider.GetRequiredService<ICandleUpdaterService>();
                var candleAggregator = scope.ServiceProvider.GetRequiredService<ICandleAggregatorService>();

                var symbols = await symbolSyncService.GetActiveSymbolsAsync();
                _logger.LogInformation("🔍 Found {Count} active symbols to update.", symbols.Count);

                // --- مرحله 1: دریافت کندل‌های 1 دقیقه‌ای
                foreach (var symbol in symbols)
                {
                    _logger.LogInformation("⏱ Updating {Symbol} (1m)...", symbol);
                    await candleUpdater.UpdateCandlesAsync(symbol, "1m", stoppingToken);
                }

                // --- مرحله 2: ساخت تایم‌فریم‌های بالاتر بر اساس زمان
                var now = DateTime.UtcNow;

                if ((now - _last5mUpdate).TotalMinutes >= 5)
                {
                    _logger.LogInformation("⏱ Aggregating 5m candles...");
                    await candleAggregator.AggregateCandlesAsync("5m");
                    _last5mUpdate = now;
                }

                if ((now - _last1hUpdate).TotalHours >= 1)
                {
                    _logger.LogInformation("⏱ Aggregating 1h candles...");
                    await candleAggregator.AggregateCandlesAsync("1h");
                    _last1hUpdate = now;
                }

                if ((now - _last4hUpdate).TotalHours >= 4)
                {
                    _logger.LogInformation("⏱ Aggregating 4h candles...");
                    await candleAggregator.AggregateCandlesAsync("4h");
                    _last4hUpdate = now;
                }

                if ((now - _last1dUpdate).TotalDays >= 1)
                {
                    _logger.LogInformation("⏱ Aggregating 1d candles...");
                    await candleAggregator.AggregateCandlesAsync("1d");
                    _last1dUpdate = now;
                }

                _logger.LogInformation("✅ Candle update and aggregation completed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error in CandleUpdaterHostedService loop.");
            }
            finally
            {
                try
                {
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("CandleUpdaterHostedService stopping...");
                }
            }
        }
    }
}