using ApplicationLayer.Interfaces.Binance;
using ApplicationLayer.Interfaces.Signals;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace InfrastructureLayer.HostedServices;

public class SignalUpdaterHostedService(
    IServiceScopeFactory _scopeFactory,
    ILogger<SignalUpdaterHostedService> _logger
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("🚀 SignalUpdaterHostedService started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();

                var symbolSyncService = scope.ServiceProvider.GetRequiredService<IBinanceSymbolSyncService>();
                var signalService = scope.ServiceProvider.GetRequiredService<ISignalService>();

                var symbols = await symbolSyncService.GetActiveSymbolsAsync();
                _logger.LogInformation("🔍 Found {Count} active symbols for signal generation.", symbols.Count);

                foreach (var symbol in symbols)
                {
                    try
                    {
                        var results = await signalService.GenerateSignalsAsync(symbol, stoppingToken);
                        _logger.LogInformation("✅ Generated {Count} signals for {Symbol}.", results.Count, symbol);
                    }
                    catch (OperationCanceledException)
                    {
                        throw; // احترام به توقف برنامه
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "❌ Error generating signals for {Symbol}.", symbol);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("SignalUpdaterHostedService stopping...");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error in SignalUpdaterHostedService loop.");
            }
            finally
            {
                try
                {
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("SignalUpdaterHostedService canceled during delay.");
                }
            }
        }
    }
}