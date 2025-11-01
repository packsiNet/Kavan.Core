using ApplicationLayer.Common;
using ApplicationLayer.Interfaces.Services;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Skender.Stock.Indicators;

namespace InfrastructureLayer.BusinessLogic.Services.TechnicalSignals;

[InjectAsScoped]
public class EmaSignalEvaluator : BaseSignalEvaluator
{
    public override string IndicatorCategory => "EMA";
    public override string IndicatorName => "Exponential Moving Average";

    public override async Task<IEnumerable<TechnicalSignal>> EvaluateAsync(
        string symbol,
        string timeFrame,
        IEnumerable<CandleBase> candles,
        CancellationToken cancellationToken = default)
    {
        var signals = new List<TechnicalSignal>();
        
        if (!HasSufficientData(candles, 50)) // Need at least 50 periods for reliable EMA signals
            return signals;

        var orderedCandles = GetOrderedCandles(candles).ToList();
        
        // Convert to Skender format
        var quotes = orderedCandles.Select(c => new Quote
        {
            Date = c.OpenTime,
            Open = c.Open,
            High = c.High,
            Low = c.Low,
            Close = c.Close,
            Volume = c.Volume
        }).ToList();

        // Calculate different EMA periods
        var ema12 = quotes.GetEma(12).ToList();
        var ema26 = quotes.GetEma(26).ToList();
        var ema50 = quotes.GetEma(50).ToList();
        var ema200 = quotes.GetEma(200).ToList();

        if (ema12.Count < 2 || ema26.Count < 2)
            return signals;

        var currentPrice = orderedCandles.Last().Close;
        var previousPrice = orderedCandles[orderedCandles.Count - 2].Close;
        
        var currentEma12 = (decimal?)ema12.Last().Ema;
        var previousEma12 = (decimal?)ema12[ema12.Count - 2].Ema;
        var currentEma26 = (decimal?)ema26.Last().Ema;
        var previousEma26 = (decimal?)ema26[ema26.Count - 2].Ema;

        // 1. EMA Cross Above Signal
        if (currentEma12.HasValue && previousEma12.HasValue)
        {
            if (previousPrice <= previousEma12 && currentPrice > currentEma12)
            {
                signals.Add(CreateSignal(symbol, timeFrame, "EMA12 Cross Above", SignalType.Buy, DetailedSignalType.EmaCrossAbove, currentEma12,
                    $"{{\"EMA12\":{currentEma12:F4},\"Price\":{currentPrice:F4}}}"));
            }
        }

        // 2. EMA Cross Below Signal
        if (currentEma12.HasValue && previousEma12.HasValue)
        {
            if (previousPrice >= previousEma12 && currentPrice < currentEma12)
            {
                signals.Add(CreateSignal(symbol, timeFrame, "EMA12 Cross Below", SignalType.Sell, DetailedSignalType.EmaCrossBelow, currentEma12,
                    $"{{\"EMA12\":{currentEma12:F4},\"Price\":{currentPrice:F4}}}"));
            }
        }

        // 3. EMA Golden Cross (EMA12 crosses above EMA26)
        if (currentEma12.HasValue && currentEma26.HasValue && previousEma12.HasValue && previousEma26.HasValue)
        {
            if (previousEma12 <= previousEma26 && currentEma12 > currentEma26)
            {
                signals.Add(CreateSignal(symbol, timeFrame, "EMA Golden Cross", SignalType.Buy, DetailedSignalType.EmaGoldenCross, currentEma12,
                    $"{{\"EMA12\":{currentEma12:F4},\"EMA26\":{currentEma26:F4}}}"));
            }
        }

        // 4. EMA Death Cross (EMA12 crosses below EMA26)
        if (currentEma12.HasValue && currentEma26.HasValue && previousEma12.HasValue && previousEma26.HasValue)
        {
            if (previousEma12 >= previousEma26 && currentEma12 < currentEma26)
            {
                signals.Add(CreateSignal(symbol, timeFrame, "EMA Death Cross", SignalType.Sell, DetailedSignalType.EmaDeathCross, currentEma12,
                    $"{{\"EMA12\":{currentEma12:F4},\"EMA26\":{currentEma26:F4}}}"));
            }
        }

        // 5. EMA Bounce Signal (Price bounces off EMA support/resistance)
        if (currentEma26.HasValue && orderedCandles.Count >= 3)
        {
            var twoPeriodsBefore = orderedCandles[orderedCandles.Count - 3].Close;
            var tolerance = currentEma26 * 0.002m; // 0.2% tolerance

            // Bullish bounce off EMA26 support
            if (previousPrice <= currentEma26 + tolerance && previousPrice >= currentEma26 - tolerance &&
                currentPrice > previousPrice && twoPeriodsBefore > currentEma26)
            {
                signals.Add(CreateSignal(symbol, timeFrame, "EMA26 Bullish Bounce", SignalType.Buy, DetailedSignalType.EmaBullishBounce, currentEma26,
                    $"{{\"EMA26\":{currentEma26:F4},\"BouncePrice\":{previousPrice:F4}}}"));
            }

            // Bearish bounce off EMA26 resistance
            if (previousPrice >= currentEma26 - tolerance && previousPrice <= currentEma26 + tolerance &&
                currentPrice < previousPrice && twoPeriodsBefore < currentEma26)
            {
                signals.Add(CreateSignal(symbol, timeFrame, "EMA26 Bearish Bounce", SignalType.Sell, DetailedSignalType.EmaBearishBounce, currentEma26,
                    $"{{\"EMA26\":{currentEma26:F4},\"BouncePrice\":{previousPrice:F4}}}"));
            }
        }

        // 6. EMA Breakout Signal (Strong move away from EMA)
        if (currentEma26.HasValue)
        {
            var breakoutThreshold = 0.015m; // 1.5% breakout threshold

            if (currentPrice > currentEma26 * (1 + breakoutThreshold))
            {
                signals.Add(CreateSignal(symbol, timeFrame, "EMA26 Bullish Breakout", SignalType.Buy, DetailedSignalType.EmaBullishBreakout, currentEma26,
                    $"{{\"EMA26\":{currentEma26:F4},\"Price\":{currentPrice:F4},\"Breakout\":{((currentPrice / currentEma26 - 1) * 100):F2}}}"));
            }
            else if (currentPrice < currentEma26 * (1 - breakoutThreshold))
            {
                signals.Add(CreateSignal(symbol, timeFrame, "EMA26 Bearish Breakout", SignalType.Sell, DetailedSignalType.EmaBearishBreakout, currentEma26,
                    $"{{\"EMA26\":{currentEma26:F4},\"Price\":{currentPrice:F4},\"Breakout\":{((currentPrice / currentEma26 - 1) * 100):F2}}}"));
            }
        }

        // 7. EMA Convergence Signal (EMAs coming together)
        if (currentEma12.HasValue && currentEma26.HasValue && previousEma12.HasValue && previousEma26.HasValue)
        {
            var currentSpread = Math.Abs(currentEma12.Value - currentEma26.Value);
            var previousSpread = Math.Abs(previousEma12.Value - previousEma26.Value);
            var averageEma = (currentEma12.Value + currentEma26.Value) / 2;
            var convergenceThreshold = averageEma * 0.005m; // 0.5% convergence threshold

            if (currentSpread < convergenceThreshold && previousSpread > currentSpread)
            {
                signals.Add(CreateSignal(symbol, timeFrame, "EMA Convergence", SignalType.Neutral, DetailedSignalType.EmaConvergence, averageEma,
                    $"{{\"EMA12\":{currentEma12:F4},\"EMA26\":{currentEma26:F4},\"Spread\":{currentSpread:F4}}}"));
            }
        }

        // 8. EMA Divergence Signal (EMAs moving apart)
        if (currentEma12.HasValue && currentEma26.HasValue && previousEma12.HasValue && previousEma26.HasValue)
        {
            var currentSpread = Math.Abs(currentEma12.Value - currentEma26.Value);
            var previousSpread = Math.Abs(previousEma12.Value - previousEma26.Value);
            var averageEma = (currentEma12.Value + currentEma26.Value) / 2;
            var divergenceThreshold = averageEma * 0.02m; // 2% divergence threshold

            if (currentSpread > divergenceThreshold && currentSpread > previousSpread)
            {
                var signalType = currentEma12 > currentEma26 ? SignalType.Buy : SignalType.Sell;
                var direction = currentEma12 > currentEma26 ? "Bullish" : "Bearish";
                
                signals.Add(CreateSignal(symbol, timeFrame, $"EMA {direction} Divergence", signalType, DetailedSignalType.EmaDivergence, averageEma,
                    $"{{\"EMA12\":{currentEma12:F4},\"EMA26\":{currentEma26:F4},\"Spread\":{currentSpread:F4}}}"));
            }
        }

        // 9. EMA Trend Confirmation Signal (using EMA50 and EMA200)
        if (ema50.Count >= 2 && ema200.Count >= 2)
        {
            var currentEma50 = (decimal?)ema50.Last().Ema;
            var currentEma200 = (decimal?)ema200.Last().Ema;

            if (currentEma50.HasValue && currentEma200.HasValue && 
                currentEma12.HasValue && currentEma26.HasValue)
            {
                // Strong bullish trend confirmation
                if (currentEma12 > currentEma26 && currentEma26 > currentEma50 && 
                    currentEma50 > currentEma200 && currentPrice > currentEma12)
                {
                    signals.Add(CreateSignal(symbol, timeFrame, "EMA Bullish Trend Confirmation", SignalType.Buy, DetailedSignalType.EmaBullishTrendConfirmation, currentEma12,
                        $"{{\"EMA12\":{currentEma12:F4},\"EMA26\":{currentEma26:F4},\"EMA50\":{currentEma50:F4},\"EMA200\":{currentEma200:F4}}}"));
                }

                // Strong bearish trend confirmation
                if (currentEma12 < currentEma26 && currentEma26 < currentEma50 && 
                    currentEma50 < currentEma200 && currentPrice < currentEma12)
                {
                    signals.Add(CreateSignal(symbol, timeFrame, "EMA Bearish Trend Confirmation", SignalType.Sell, DetailedSignalType.EmaBearishTrendConfirmation, currentEma12,
                        $"{{\"EMA12\":{currentEma12:F4},\"EMA26\":{currentEma26:F4},\"EMA50\":{currentEma50:F4},\"EMA200\":{currentEma200:F4}}}"));
                }
            }
        }

        return signals;
    }
}