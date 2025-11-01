using ApplicationLayer.Common;
using ApplicationLayer.Interfaces.Services;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Skender.Stock.Indicators;

namespace InfrastructureLayer.BusinessLogic.Services.TechnicalSignals;

[InjectAsScoped]
public class MovingAverageCrossoverSignalEvaluator : BaseSignalEvaluator
{
    public override string IndicatorCategory => "MA Crossover";
    public override string IndicatorName => "Moving Average Crossover";

    public override async Task<IEnumerable<TechnicalSignal>> EvaluateAsync(
        string symbol,
        string timeFrame,
        IEnumerable<CandleBase> candles,
        CancellationToken cancellationToken = default)
    {
        var signals = new List<TechnicalSignal>();
        
        if (!HasSufficientData(candles, 200)) // Need at least 200 periods for MA200
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

        // Calculate different SMA periods for crossover analysis
        var sma50 = quotes.GetSma(50).ToList();
        var sma200 = quotes.GetSma(200).ToList();
        var sma20 = quotes.GetSma(20).ToList();

        if (sma50.Count < 2 || sma200.Count < 2 || sma20.Count < 2)
            return signals;

        var currentPrice = orderedCandles.Last().Close;
        var previousPrice = orderedCandles[orderedCandles.Count - 2].Close;
        
        var currentSma50 = (decimal?)sma50.Last().Sma;
        var previousSma50 = (decimal?)sma50[sma50.Count - 2].Sma;
        var currentSma200 = (decimal?)sma200.Last().Sma;
        var previousSma200 = (decimal?)sma200[sma200.Count - 2].Sma;
        var currentSma20 = (decimal?)sma20.Last().Sma;
        var previousSma20 = (decimal?)sma20[sma20.Count - 2].Sma;

        // 1. Golden Cross Signal (SMA50 crosses above SMA200)
        if (currentSma50.HasValue && currentSma200.HasValue && 
            previousSma50.HasValue && previousSma200.HasValue)
        {
            if (previousSma50 <= previousSma200 && currentSma50 > currentSma200)
            {
                signals.Add(CreateSignal(symbol, timeFrame, "Golden Cross", SignalType.Buy, DetailedSignalType.MovingAverageGoldenCross, currentSma50,
                    $"{{\"SMA50\":{currentSma50:F4},\"SMA200\":{currentSma200:F4},\"CrossoverStrength\":{((currentSma50 / currentSma200 - 1) * 100):F2}}}"));
            }
        }

        // 2. Death Cross Signal (SMA50 crosses below SMA200)
        if (currentSma50.HasValue && currentSma200.HasValue && 
            previousSma50.HasValue && previousSma200.HasValue)
        {
            if (previousSma50 >= previousSma200 && currentSma50 < currentSma200)
            {
                signals.Add(CreateSignal(symbol, timeFrame, "Death Cross", SignalType.Sell, DetailedSignalType.MovingAverageDeathCross, currentSma50,
                    $"{{\"SMA50\":{currentSma50:F4},\"SMA200\":{currentSma200:F4},\"CrossoverStrength\":{((currentSma50 / currentSma200 - 1) * 100):F2}}}"));
            }
        }

        // 3. MA Support Signal (Price bounces off moving average support)
        if (currentSma50.HasValue && orderedCandles.Count >= 3)
        {
            var twoPeriodsBefore = orderedCandles[orderedCandles.Count - 3].Close;
            var tolerance = currentSma50 * 0.005m; // 0.5% tolerance for support level

            // Price bounces off SMA50 support
            if (previousPrice <= currentSma50 + tolerance && previousPrice >= currentSma50 - tolerance &&
                currentPrice > previousPrice && twoPeriodsBefore > currentSma50 &&
                currentPrice > currentSma50) // Confirming bounce above support
            {
                signals.Add(CreateSignal(symbol, timeFrame, "SMA50 Support Bounce", SignalType.Buy, DetailedSignalType.MovingAverageSupportBounce, currentSma50,
                    $"{{\"SMA50\":{currentSma50:F4},\"SupportPrice\":{previousPrice:F4},\"BounceStrength\":{((currentPrice / previousPrice - 1) * 100):F2}}}"));
            }

            // Price bounces off SMA20 support (shorter-term support)
            if (currentSma20.HasValue)
            {
                var sma20Tolerance = currentSma20 * 0.003m; // 0.3% tolerance for SMA20
                if (previousPrice <= currentSma20 + sma20Tolerance && previousPrice >= currentSma20 - sma20Tolerance &&
                    currentPrice > previousPrice && twoPeriodsBefore > currentSma20 &&
                    currentPrice > currentSma20)
                {
                    signals.Add(CreateSignal(symbol, timeFrame, "SMA20 Support Bounce", SignalType.Buy, DetailedSignalType.MovingAverageSupportBounce, currentSma20,
                        $"{{\"SMA20\":{currentSma20:F4},\"SupportPrice\":{previousPrice:F4},\"BounceStrength\":{((currentPrice / previousPrice - 1) * 100):F2}}}"));
                }
            }
        }

        // 4. MA Resistance Signal (Price rejects from moving average resistance)
        if (currentSma50.HasValue && orderedCandles.Count >= 3)
        {
            var twoPeriodsBefore = orderedCandles[orderedCandles.Count - 3].Close;
            var tolerance = currentSma50 * 0.005m; // 0.5% tolerance for resistance level

            // Price rejects from SMA50 resistance
            if (previousPrice >= currentSma50 - tolerance && previousPrice <= currentSma50 + tolerance &&
                currentPrice < previousPrice && twoPeriodsBefore < currentSma50 &&
                currentPrice < currentSma50) // Confirming rejection below resistance
            {
                signals.Add(CreateSignal(symbol, timeFrame, "SMA50 Resistance Rejection", SignalType.Sell, DetailedSignalType.MovingAverageResistanceRejection, currentSma50,
                    $"{{\"SMA50\":{currentSma50:F4},\"ResistancePrice\":{previousPrice:F4},\"RejectionStrength\":{((previousPrice / currentPrice - 1) * 100):F2}}}"));
            }

            // Price rejects from SMA20 resistance (shorter-term resistance)
            if (currentSma20.HasValue)
            {
                var sma20Tolerance = currentSma20 * 0.003m; // 0.3% tolerance for SMA20
                if (previousPrice >= currentSma20 - sma20Tolerance && previousPrice <= currentSma20 + sma20Tolerance &&
                    currentPrice < previousPrice && twoPeriodsBefore < currentSma20 &&
                    currentPrice < currentSma20)
                {
                    signals.Add(CreateSignal(symbol, timeFrame, "SMA20 Resistance Rejection", SignalType.Sell, DetailedSignalType.MovingAverageResistanceRejection, currentSma20,
                        $"{{\"SMA20\":{currentSma20:F4},\"ResistancePrice\":{previousPrice:F4},\"RejectionStrength\":{((previousPrice / currentPrice - 1) * 100):F2}}}"));
                }
            }
        }

        // Additional: Price crossing above/below key moving averages
        if (currentSma200.HasValue && previousSma200.HasValue)
        {
            // Bullish breakout above SMA200
            if (previousPrice <= previousSma200 && currentPrice > currentSma200)
            {
                signals.Add(CreateSignal(symbol, timeFrame, "SMA200 Bullish Breakout", SignalType.Buy, DetailedSignalType.MovingAverageBullishBreakout, currentSma200,
                    $"{{\"SMA200\":{currentSma200:F4},\"Price\":{currentPrice:F4},\"BreakoutStrength\":{((currentPrice / currentSma200 - 1) * 100):F2}}}"));
            }

            // Bearish breakdown below SMA200
            if (previousPrice >= previousSma200 && currentPrice < currentSma200)
            {
                signals.Add(CreateSignal(symbol, timeFrame, "SMA200 Bearish Breakdown", SignalType.Sell, DetailedSignalType.MovingAverageBearishBreakdown, currentSma200,
                    $"{{\"SMA200\":{currentSma200:F4},\"Price\":{currentPrice:F4},\"BreakdownStrength\":{((currentSma200 / currentPrice - 1) * 100):F2}}}"));
            }
        }

        return signals;
    }
}