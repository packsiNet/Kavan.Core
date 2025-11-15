using ApplicationLayer.Interfaces.Services.Signals;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace InfrastructureLayer.BusinessLogic.Services.Signals;

/// <summary>
/// Background service that runs every minute to detect signals across all timeframes and cryptocurrencies.
/// </summary>
public class SignalsBackgroundService(IServiceScopeFactory scopeFactory, ILogger<SignalsBackgroundService> logger) : BackgroundService
{
    private readonly SemaphoreSlim _gate = new(1, 1);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("SignalsBackgroundService started.");
        var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));
        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                if (!await _gate.WaitAsync(0, stoppingToken))
                {
                    logger.LogInformation("Previous signal detection still running; skipping this tick.");
                    continue;
                }

                try
                {
                    using var scope = scopeFactory.CreateScope();
                    var breakouts = scope.ServiceProvider.GetRequiredService<ISignalService>();
                    var ichimokuUp = scope.ServiceProvider.GetRequiredService<IIchimokuAboveCloudService>();
                    var ichimokuDown = scope.ServiceProvider.GetRequiredService<IIchimokuBelowCloudService>();
                    var sma = scope.ServiceProvider.GetRequiredService<ISmaSignalService>();
                    var ema = scope.ServiceProvider.GetRequiredService<IEmasignalService>();
                    var candles = scope.ServiceProvider.GetRequiredService<ICandlestickPatternService>();
                    var boll = scope.ServiceProvider.GetRequiredService<IBollingerSignalService>();
                    var rsi = scope.ServiceProvider.GetRequiredService<IRsiSignalService>();
                    var adx = scope.ServiceProvider.GetRequiredService<IAdxSignalService>();
                    var cci = scope.ServiceProvider.GetRequiredService<ICciSignalService>();
                    var chikouAbove = scope.ServiceProvider.GetRequiredService<IIchimokuChikouAbovePriceService>();
                    var chikouBelow = scope.ServiceProvider.GetRequiredService<IIchimokuChikouBelowPriceService>();
                    var ichimokuGreen = scope.ServiceProvider.GetRequiredService<IIchimokuGreenKumoService>();
                    var ichimokuRed = scope.ServiceProvider.GetRequiredService<IIchimokuRedKumoService>();
                    var tkBull = scope.ServiceProvider.GetRequiredService<ITenkanKijunBullishCrossService>();
                    var tkBear = scope.ServiceProvider.GetRequiredService<ITenkanKijunBearishCrossService>();

                    logger.LogInformation("Running breakout detection across all markets/timeframes...");
                    await breakouts.DetectBreakoutsAsync(symbols: null, timeframes: null, lookbackPeriod: 0);

                    logger.LogInformation("Running Ichimoku above-cloud detection...");
                    await ichimokuUp.DetectPriceAboveCloudAsync(symbols: null, timeframes: null, lookbackPeriod: 0);

                    logger.LogInformation("Running Ichimoku below-cloud detection...");
                    await ichimokuDown.DetectPriceBelowCloudAsync(symbols: null, timeframes: null, lookbackPeriod: 0);

                    await sma.DetectPriceAboveSma10Async(symbols: null, timeframes: null, lookbackPeriod: 0);
                    await sma.DetectPriceBelowSma10Async(symbols: null, timeframes: null, lookbackPeriod: 0);
                    await sma.DetectPriceAboveSma50Async(symbols: null, timeframes: null, lookbackPeriod: 0);
                    await sma.DetectPriceBelowSma50Async(symbols: null, timeframes: null, lookbackPeriod: 0);
                    await sma.DetectPriceAboveSma200Async(symbols: null, timeframes: null, lookbackPeriod: 0);
                    await sma.DetectPriceBelowSma200Async(symbols: null, timeframes: null, lookbackPeriod: 0);
                    await sma.DetectSma20BreakoutUpAsync(symbols: null, timeframes: null, lookbackPeriod: 0);
                    await sma.DetectSma20BreakoutDownAsync(symbols: null, timeframes: null, lookbackPeriod: 0);
                    await sma.DetectSma100BreakoutUpAsync(symbols: null, timeframes: null, lookbackPeriod: 0);
                    await sma.DetectSma100BreakoutDownAsync(symbols: null, timeframes: null, lookbackPeriod: 0);

                    await ema.DetectPriceAboveEma10Async(symbols: null, timeframes: null, lookbackPeriod: 0);
                    await ema.DetectPriceBelowEma10Async(symbols: null, timeframes: null, lookbackPeriod: 0);
                    await ema.DetectPriceAboveEma50Async(symbols: null, timeframes: null, lookbackPeriod: 0);
                    await ema.DetectPriceBelowEma50Async(symbols: null, timeframes: null, lookbackPeriod: 0);
                    await ema.DetectPriceAboveEma200Async(symbols: null, timeframes: null, lookbackPeriod: 0);
                    await ema.DetectPriceBelowEma200Async(symbols: null, timeframes: null, lookbackPeriod: 0);
                    await ema.DetectEma20BreakoutUpAsync(symbols: null, timeframes: null, lookbackPeriod: 0);
                    await ema.DetectEma20BreakoutDownAsync(symbols: null, timeframes: null, lookbackPeriod: 0);
                    await ema.DetectEma100BreakoutUpAsync(symbols: null, timeframes: null, lookbackPeriod: 0);
                    await ema.DetectEma100BreakoutDownAsync(symbols: null, timeframes: null, lookbackPeriod: 0);
                    await sma.DetectSma50_200BullishCrossAsync(symbols: null, timeframes: null, lookbackPeriod: 0);
                    await sma.DetectSma50_200BearishCrossAsync(symbols: null, timeframes: null, lookbackPeriod: 0);
                    await ema.DetectEma50_200BullishCrossAsync(symbols: null, timeframes: null, lookbackPeriod: 0);
                    await ema.DetectEma50_200BearishCrossAsync(symbols: null, timeframes: null, lookbackPeriod: 0);
                    await candles.DetectBullishHammerAsync(symbols: null, timeframes: null, lookbackPeriod: 0);
                    await candles.DetectBearishShootingStarAsync(symbols: null, timeframes: null, lookbackPeriod: 0);
                    await candles.DetectBullishEngulfingAsync(symbols: null, timeframes: null, lookbackPeriod: 0);
                    await candles.DetectBearishEngulfingAsync(symbols: null, timeframes: null, lookbackPeriod: 0);
                    await candles.DetectThreeWhiteSoldiersAsync(symbols: null, timeframes: null, lookbackPeriod: 0);
                    await candles.DetectThreeBlackCrowsAsync(symbols: null, timeframes: null, lookbackPeriod: 0);
                    await candles.DetectMorningStarAsync(symbols: null, timeframes: null, lookbackPeriod: 0);
                    await candles.DetectEveningStarAsync(symbols: null, timeframes: null, lookbackPeriod: 0);
                    await candles.DetectHangingManAsync(symbols: null, timeframes: null, lookbackPeriod: 0);
                    await candles.DetectDojiAsync(symbols: null, timeframes: null, lookbackPeriod: 0);
                    await boll.DetectPriceAboveUpperBandAsync(symbols: null, timeframes: null, lookbackPeriod: 0);
                    await boll.DetectPriceBelowLowerBandAsync(symbols: null, timeframes: null, lookbackPeriod: 0);
                    await boll.DetectUpperBandBreakoutAsync(symbols: null, timeframes: null, lookbackPeriod: 0);
                    await boll.DetectLowerBandBreakoutAsync(symbols: null, timeframes: null, lookbackPeriod: 0);
                    await boll.DetectBandSqueezeAsync(symbols: null, timeframes: null, lookbackPeriod: 0);
                    await rsi.DetectRsiOverboughtAsync(symbols: null, timeframes: null, lookbackPeriod: 0, period: 14);
                    await rsi.DetectRsiOversoldAsync(symbols: null, timeframes: null, lookbackPeriod: 0, period: 14);
                    await rsi.DetectRsiCrossAbove50Async(symbols: null, timeframes: null, lookbackPeriod: 0, period: 14);
                    await rsi.DetectRsiCrossBelow50Async(symbols: null, timeframes: null, lookbackPeriod: 0, period: 14);
                    await rsi.DetectRsiBullishDivergenceAsync(symbols: null, timeframes: null, lookbackPeriod: 0, period: 14);
                    await rsi.DetectRsiBearishDivergenceAsync(symbols: null, timeframes: null, lookbackPeriod: 0, period: 14);
                    await adx.DetectAdxAboveAsync(symbols: null, timeframes: null, lookbackPeriod: 0, period: 14, threshold: 25m);
                    await adx.DetectAdxBelowAsync(symbols: null, timeframes: null, lookbackPeriod: 0, period: 14, threshold: 20m);
                    await adx.DetectDiPlusAboveDiMinusAsync(symbols: null, timeframes: null, lookbackPeriod: 0, period: 14);
                    await adx.DetectDiMinusAboveDiPlusAsync(symbols: null, timeframes: null, lookbackPeriod: 0, period: 14);
                    await cci.DetectCciAbove100Async(symbols: null, timeframes: null, lookbackPeriod: 0, period: 20);
                    await cci.DetectCciBelowMinus100Async(symbols: null, timeframes: null, lookbackPeriod: 0, period: 20);
                    await cci.DetectCciCrossAboveZeroAsync(symbols: null, timeframes: null, lookbackPeriod: 0, period: 20);
                    await cci.DetectCciCrossBelowZeroAsync(symbols: null, timeframes: null, lookbackPeriod: 0, period: 20);

                    logger.LogInformation("Running Ichimoku Chikou above-price detection...");
                    await chikouAbove.DetectChikouAbovePriceAsync(symbols: null, timeframes: null, lookbackPeriod: 0);

                    logger.LogInformation("Running Ichimoku Chikou below-price detection...");
                    await chikouBelow.DetectChikouBelowPriceAsync(symbols: null, timeframes: null, lookbackPeriod: 0);

                    logger.LogInformation("Running Ichimoku green Kumo detection...");
                    await ichimokuGreen.DetectGreenKumoAsync(symbols: null, timeframes: null, lookbackPeriod: 0);

                    logger.LogInformation("Running Ichimoku red Kumo detection...");
                    await ichimokuRed.DetectRedKumoAsync(symbols: null, timeframes: null, lookbackPeriod: 0);

                    logger.LogInformation("Running Ichimoku Tenkan/Kijun bullish cross detection...");
                    await tkBull.DetectBullishCrossAsync(symbols: null, timeframes: null, lookbackPeriod: 0);

                    logger.LogInformation("Running Ichimoku Tenkan/Kijun bearish cross detection...");
                    await tkBear.DetectBearishCrossAsync(symbols: null, timeframes: null, lookbackPeriod: 0);

                    logger.LogInformation("Signal detection cycle completed.");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error during signal detection cycle");
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
            logger.LogInformation("SignalsBackgroundService stopped.");
        }
    }
}