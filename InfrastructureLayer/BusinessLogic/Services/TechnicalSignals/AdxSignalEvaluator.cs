using ApplicationLayer.Common;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Skender.Stock.Indicators;

namespace InfrastructureLayer.BusinessLogic.Services.TechnicalSignals;

[InjectAsScoped]
public class AdxSignalEvaluator : BaseSignalEvaluator
{
    public override string IndicatorCategory => "ADX";
    public override string IndicatorName => "Average Directional Index";

    public override async Task<IEnumerable<TechnicalSignal>> EvaluateAsync(
        string symbol,
        string timeFrame,
        IEnumerable<CandleBase> candles,
        CancellationToken cancellationToken = default)
    {
        var signals = new List<TechnicalSignal>();

        if (!HasSufficientData(candles, 30)) // Need at least 30 candles for ADX calculations
            return signals;

        var orderedCandles = GetOrderedCandles(candles).ToList();

        var quotes = orderedCandles.Select(c => new Quote
        {
            Date = c.OpenTime,
            Open = c.Open,
            High = c.High,
            Low = c.Low,
            Close = c.Close,
            Volume = c.Volume
        }).ToList();

        var adx = quotes.GetAdx(14).ToList(); // 14-period ADX

        if (adx.Count < 3) return signals;

        // 1. ADX Trend Strength Signal
        var trendStrengthSignal = EvaluateAdxTrendStrengthSignal(adx, symbol, timeFrame);
        if (trendStrengthSignal != null) signals.Add(trendStrengthSignal);

        // 2. ADX Rising Signal
        var adxRisingSignal = EvaluateAdxRisingSignal(adx, symbol, timeFrame);
        if (adxRisingSignal != null) signals.Add(adxRisingSignal);

        // 3. ADX Falling Signal
        var adxFallingSignal = EvaluateAdxFallingSignal(adx, symbol, timeFrame);
        if (adxFallingSignal != null) signals.Add(adxFallingSignal);

        // 4. DI+ DI- Crossover Signal
        var diCrossoverSignal = EvaluateDiCrossoverSignal(adx, symbol, timeFrame);
        if (diCrossoverSignal != null) signals.Add(diCrossoverSignal);

        return signals;
    }

    private TechnicalSignal? EvaluateAdxTrendStrengthSignal(List<AdxResult> adx, string symbol, string timeFrame)
    {
        var current = adx.Last();
        
        if (!current.Adx.HasValue) return null;

        var adxValue = (decimal)current.Adx.Value;

        // Strong trend: ADX > 25
        if (adxValue > 25)
        {
            var trendStrength = adxValue switch
            {
                > 50 => "Very Strong Trend",
                > 40 => "Strong Trend",
                > 25 => "Moderate Trend",
                _ => "Weak Trend"
            };

            return CreateSignal(
                symbol: symbol,
                timeFrame: timeFrame,
                conditionTitle: $"ADX {trendStrength}",
                signalType: SignalType.Neutral,
                detailedSignalType: DetailedSignalType.AdxTrendStrength,
                value: adxValue,
                additionalData: $"{{\"ADX\":{adxValue:F2},\"TrendStrength\":\"{trendStrength}\"}}"
            );
        }

        // Weak trend: ADX < 20
        if (adxValue < 20)
        {
            return CreateSignal(
                symbol: symbol,
                timeFrame: timeFrame,
                conditionTitle: "ADX Weak Trend",
                signalType: SignalType.Neutral,
                detailedSignalType: DetailedSignalType.AdxWeakTrend,
                value: adxValue,
                additionalData: $"{{\"ADX\":{adxValue:F2},\"TrendStrength\":\"Weak Trend\"}}"
            );
        }

        return null;
    }

    private TechnicalSignal? EvaluateAdxRisingSignal(List<AdxResult> adx, string symbol, string timeFrame)
    {
        if (adx.Count < 3) return null;

        var current = adx.Last();
        var previous = adx[adx.Count - 2];
        var twoBefore = adx[adx.Count - 3];

        if (!current.Adx.HasValue || !previous.Adx.HasValue || !twoBefore.Adx.HasValue) 
            return null;

        var currentAdx = (decimal)current.Adx.Value;
        var previousAdx = (decimal)previous.Adx.Value;
        var twoBeforeAdx = (decimal)twoBefore.Adx.Value;

        // ADX is rising for at least 2 periods and above 20
        if (currentAdx > previousAdx && previousAdx > twoBeforeAdx && currentAdx > 20)
        {
            return CreateSignal(
                symbol: symbol,
                timeFrame: timeFrame,
                conditionTitle: "ADX Rising",
                signalType: SignalType.Neutral,
                detailedSignalType: DetailedSignalType.AdxRising,
                value: currentAdx,
                additionalData: $"{{\"ADX\":{currentAdx:F2},\"PreviousADX\":{previousAdx:F2},\"Trend\":\"Rising\"}}"
            );
        }

        return null;
    }

    private TechnicalSignal? EvaluateAdxFallingSignal(List<AdxResult> adx, string symbol, string timeFrame)
    {
        if (adx.Count < 3) return null;

        var current = adx.Last();
        var previous = adx[adx.Count - 2];
        var twoBefore = adx[adx.Count - 3];

        if (!current.Adx.HasValue || !previous.Adx.HasValue || !twoBefore.Adx.HasValue) 
            return null;

        var currentAdx = (decimal)current.Adx.Value;
        var previousAdx = (decimal)previous.Adx.Value;
        var twoBeforeAdx = (decimal)twoBefore.Adx.Value;

        // ADX is falling for at least 2 periods from a high level (above 30)
        if (currentAdx < previousAdx && previousAdx < twoBeforeAdx && twoBeforeAdx > 30)
        {
            return CreateSignal(
                symbol: symbol,
                timeFrame: timeFrame,
                conditionTitle: "ADX Falling",
                signalType: SignalType.Neutral,
                detailedSignalType: DetailedSignalType.AdxFalling,
                value: currentAdx,
                additionalData: $"{{\"ADX\":{currentAdx:F2},\"PreviousADX\":{previousAdx:F2},\"Trend\":\"Falling\"}}"
            );
        }

        return null;
    }

    private TechnicalSignal? EvaluateDiCrossoverSignal(List<AdxResult> adx, string symbol, string timeFrame)
    {
        if (adx.Count < 2) return null;

        var current = adx.Last();
        var previous = adx[adx.Count - 2];

        if (!current.Pdi.HasValue || !current.Mdi.HasValue || 
            !previous.Pdi.HasValue || !previous.Mdi.HasValue) 
            return null;

        var currentPdi = (decimal)current.Pdi.Value;
        var currentMdi = (decimal)current.Mdi.Value;
        var previousPdi = (decimal)previous.Pdi.Value;
        var previousMdi = (decimal)previous.Mdi.Value;

        // DI+ crosses above DI- (bullish signal)
        if (previousPdi <= previousMdi && currentPdi > currentMdi)
        {
            return CreateSignal(
                symbol: symbol,
                timeFrame: timeFrame,
                conditionTitle: "DI+ Crosses Above DI-",
                signalType: SignalType.Buy,
                detailedSignalType: DetailedSignalType.AdxDiPlusCrossAbove,
                value: currentPdi,
                additionalData: $"{{\"DI+\":{currentPdi:F2},\"DI-\":{currentMdi:F2},\"ADX\":{current.Adx:F2}}}"
            );
        }

        // DI- crosses above DI+ (bearish signal)
        if (previousMdi <= previousPdi && currentMdi > currentPdi)
        {
            return CreateSignal(
                symbol: symbol,
                timeFrame: timeFrame,
                conditionTitle: "DI- Crosses Above DI+",
                signalType: SignalType.Sell,
                detailedSignalType: DetailedSignalType.AdxDiMinusCrossAbove,
                value: currentMdi,
                additionalData: $"{{\"DI+\":{currentPdi:F2},\"DI-\":{currentMdi:F2},\"ADX\":{current.Adx:F2}}}"
            );
        }

        return null;
    }
}