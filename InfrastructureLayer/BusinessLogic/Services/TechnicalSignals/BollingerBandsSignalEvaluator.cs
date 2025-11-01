using ApplicationLayer.Common;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Skender.Stock.Indicators;

namespace InfrastructureLayer.BusinessLogic.Services.TechnicalSignals;

[InjectAsScoped]
public class BollingerBandsSignalEvaluator : BaseSignalEvaluator
{
    public override string IndicatorCategory => "Bollinger Bands";
    public override string IndicatorName => "BB";

    public override async Task<IEnumerable<TechnicalSignal>> EvaluateAsync(
        string symbol,
        string timeFrame,
        IEnumerable<CandleBase> candles,
        CancellationToken cancellationToken = default)
    {
        var signals = new List<TechnicalSignal>();

        if (!HasSufficientData(candles, 50)) // Need at least 50 candles for BB calculations
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

        var bb = quotes.GetBollingerBands(20, 2).ToList(); // 20-period, 2 standard deviations

        if (bb.Count < 3) return signals;

        // 1. Bollinger Band Squeeze Signal
        var squeezeSignal = EvaluateBBSqueezeSignal(bb, symbol, timeFrame);
        if (squeezeSignal != null) signals.Add(squeezeSignal);

        // 2. Bollinger Band Expansion Signal
        var expansionSignal = EvaluateBBExpansionSignal(bb, symbol, timeFrame);
        if (expansionSignal != null) signals.Add(expansionSignal);

        // 3. Upper Band Touch Signal
        var upperBandSignal = EvaluateUpperBandTouchSignal(bb, quotes, symbol, timeFrame);
        if (upperBandSignal != null) signals.Add(upperBandSignal);

        // 4. Lower Band Touch Signal
        var lowerBandSignal = EvaluateLowerBandTouchSignal(bb, quotes, symbol, timeFrame);
        if (lowerBandSignal != null) signals.Add(lowerBandSignal);

        // 5. Middle Band Cross Signal
        var middleBandSignal = EvaluateMiddleBandCrossSignal(bb, quotes, symbol, timeFrame);
        if (middleBandSignal != null) signals.Add(middleBandSignal);

        return signals;
    }

    private TechnicalSignal? EvaluateBBSqueezeSignal(List<BollingerBandsResult> bb, string symbol, string timeFrame)
    {
        if (bb.Count < 20) return null;

        var current = bb.Last();
        var previous = bb[bb.Count - 2];

        if (current.UpperBand == null || current.LowerBand == null || 
            previous.UpperBand == null || previous.LowerBand == null) return null;

        var currentWidth = (decimal)(current.UpperBand - current.LowerBand);
        var previousWidth = (decimal)(previous.UpperBand - previous.LowerBand);

        // Calculate average width over last 20 periods
        var avgWidth = bb.TakeLast(20)
            .Where(b => b.UpperBand.HasValue && b.LowerBand.HasValue)
            .Average(b => (double)(b.UpperBand!.Value - b.LowerBand!.Value));

        // Squeeze: current width is significantly below average
        if (currentWidth < (decimal)avgWidth * 0.7m)
        {
            return CreateSignal(
                symbol: symbol,
                conditionTitle: "BB Squeeze",
                signalType: SignalType.Neutral,
                detailedSignalType: DetailedSignalType.BollingerBandsSqueeze,
                timeFrame: timeFrame,
                value: currentWidth,
                additionalData: $"Band width: {currentWidth:F4}, Avg width: {avgWidth:F4}"
            );
        }

        return null;
    }

    private TechnicalSignal? EvaluateBBExpansionSignal(List<BollingerBandsResult> bb, string symbol, string timeFrame)
    {
        if (bb.Count < 20) return null;

        var current = bb.Last();
        var previous = bb[bb.Count - 2];

        if (current.UpperBand == null || current.LowerBand == null || 
            previous.UpperBand == null || previous.LowerBand == null) return null;

        var currentWidth = (decimal)(current.UpperBand - current.LowerBand);
        var previousWidth = (decimal)(previous.UpperBand - previous.LowerBand);

        // Calculate average width over last 20 periods
        var avgWidth = bb.TakeLast(20)
            .Where(b => b.UpperBand.HasValue && b.LowerBand.HasValue)
            .Average(b => (double)(b.UpperBand!.Value - b.LowerBand!.Value));

        // Expansion: current width is significantly above average and increasing
        if (currentWidth > (decimal)avgWidth * 1.3m && currentWidth > previousWidth)
        {
            return CreateSignal(
                symbol: symbol,
                conditionTitle: "BB Expansion",
                signalType: SignalType.Neutral,
                detailedSignalType: DetailedSignalType.BollingerBandsExpansion,
                timeFrame: timeFrame,
                value: currentWidth,
                additionalData: $"Band width: {currentWidth:F4}, Avg width: {avgWidth:F4}"
            );
        }

        return null;
    }

    private TechnicalSignal? EvaluateUpperBandTouchSignal(List<BollingerBandsResult> bb, List<Quote> quotes, string symbol, string timeFrame)
    {
        if (bb.Count < 2 || quotes.Count < 2) return null;

        var currentBB = bb.Last();
        var currentQuote = quotes.Last();
        var previousQuote = quotes[quotes.Count - 2];

        if (currentBB.UpperBand == null) return null;

        // Price touches or exceeds upper band (potential sell signal)
        if (currentQuote.Close >= (decimal)currentBB.UpperBand && 
            previousQuote.Close < (decimal)currentBB.UpperBand)
        {
            return CreateSignal(
                symbol: symbol,
                conditionTitle: "BB Upper Band Touch",
                signalType: SignalType.Sell,
                detailedSignalType: DetailedSignalType.BollingerUpperBandTouch,
                timeFrame: timeFrame,
                value: currentQuote.Close,
                additionalData: $"Close: {currentQuote.Close:F4}, Upper Band: {currentBB.UpperBand:F4}"
            );
        }

        return null;
    }

    private TechnicalSignal? EvaluateLowerBandTouchSignal(List<BollingerBandsResult> bb, List<Quote> quotes, string symbol, string timeFrame)
    {
        if (bb.Count < 2 || quotes.Count < 2) return null;

        var currentBB = bb.Last();
        var currentQuote = quotes.Last();
        var previousQuote = quotes[quotes.Count - 2];

        if (currentBB.LowerBand == null) return null;

        // Price touches or falls below lower band (potential buy signal)
        if (currentQuote.Close <= (decimal)currentBB.LowerBand && 
            previousQuote.Close > (decimal)currentBB.LowerBand)
        {
            return CreateSignal(
                symbol: symbol,
                conditionTitle: "BB Lower Band Touch",
                signalType: SignalType.Buy,
                detailedSignalType: DetailedSignalType.BollingerLowerBandTouch,
                timeFrame: timeFrame,
                value: currentQuote.Close,
                additionalData: $"Close: {currentQuote.Close:F4}, Lower Band: {currentBB.LowerBand:F4}"
            );
        }

        return null;
    }

    private TechnicalSignal? EvaluateMiddleBandCrossSignal(List<BollingerBandsResult> bb, List<Quote> quotes, string symbol, string timeFrame)
    {
        if (bb.Count < 2 || quotes.Count < 2) return null;

        var currentBB = bb.Last();
        var previousBB = bb[bb.Count - 2];
        var currentQuote = quotes.Last();
        var previousQuote = quotes[quotes.Count - 2];

        if (currentBB.Sma == null || previousBB.Sma == null) return null;

        // Price crosses above middle band (SMA) - bullish signal
        if (currentQuote.Close > (decimal)currentBB.Sma && 
            previousQuote.Close <= (decimal)previousBB.Sma)
        {
            return CreateSignal(
                symbol: symbol,
                conditionTitle: "BB Middle Band Cross Above",
                signalType: SignalType.Buy,
                detailedSignalType: DetailedSignalType.BollingerMiddleBandCrossUp,
                timeFrame: timeFrame,
                value: currentQuote.Close,
                additionalData: $"Close: {currentQuote.Close:F4}, Middle Band: {currentBB.Sma:F4}"
            );
        }

        // Price crosses below middle band (SMA) - bearish signal
        if (currentQuote.Close < (decimal)currentBB.Sma && 
            previousQuote.Close >= (decimal)previousBB.Sma)
        {
            return CreateSignal(
                symbol: symbol,
                conditionTitle: "BB Middle Band Cross Below",
                signalType: SignalType.Sell,
                detailedSignalType: DetailedSignalType.BollingerMiddleBandCrossDown,
                timeFrame: timeFrame,
                value: currentQuote.Close,
                additionalData: $"Close: {currentQuote.Close:F4}, Middle Band: {currentBB.Sma:F4}"
            );
        }

        return null;
    }
}