using System;
using System.Threading;
using System.Threading.Tasks;
using ApplicationLayer.Interfaces.Services.Signals;
using InfrastructureLayer.Context;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace InfrastructureLayer.BusinessLogic.Services.Signals
{
    /// <summary>
    /// Background service that runs every minute to detect signals across all timeframes and cryptocurrencies.
    /// </summary>
    public class SignalsBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<SignalsBackgroundService> _logger;
        private readonly SemaphoreSlim _gate = new(1, 1);

        public SignalsBackgroundService(IServiceScopeFactory scopeFactory, ILogger<SignalsBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("SignalsBackgroundService started.");
            var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));
            try
            {
                while (await timer.WaitForNextTickAsync(stoppingToken))
                {
                    if (!await _gate.WaitAsync(0, stoppingToken))
                    {
                        _logger.LogInformation("Previous signal detection still running; skipping this tick.");
                        continue;
                    }

                    try
                    {
                        using var scope = _scopeFactory.CreateScope();
                        var breakouts = scope.ServiceProvider.GetRequiredService<ISignalService>();
                        var ichimokuUp = scope.ServiceProvider.GetRequiredService<IIchimokuAboveCloudService>();
                        var ichimokuDown = scope.ServiceProvider.GetRequiredService<IIchimokuBelowCloudService>();

                        _logger.LogInformation("Running breakout detection across all markets/timeframes...");
                        await breakouts.DetectBreakoutsAsync(symbols: null, timeframes: null, lookbackPeriod: 0);

                        _logger.LogInformation("Running Ichimoku above-cloud detection...");
                        await ichimokuUp.DetectPriceAboveCloudAsync(symbols: null, timeframes: null, lookbackPeriod: 0);

                        _logger.LogInformation("Running Ichimoku below-cloud detection...");
                        await ichimokuDown.DetectPriceBelowCloudAsync(symbols: null, timeframes: null, lookbackPeriod: 0);

                        _logger.LogInformation("Signal detection cycle completed.");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error during signal detection cycle");
                    }
                    finally
                    {
                        _gate.Release();
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // graceful shutdown
            }
            finally
            {
                _logger.LogInformation("SignalsBackgroundService stopped.");
            }
        }
    }
}