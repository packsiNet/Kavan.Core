using ApplicationLayer.Common;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Skender.Stock.Indicators;

namespace InfrastructureLayer.BusinessLogic.Services.SignalEvaluators;

[InjectAsScoped]
public class RsiSignalEvaluator : BaseSignalEvaluator
{
    public override string IndicatorCategory => "RSI";
    public override string IndicatorName => "Relative Strength Index";

    public override async Task<IEnumerable<TechnicalSignal>> EvaluateAsync(
        string symbol, 
        string timeFrame, 
        IEnumerable<CandleBase> candles,
        CancellationToken cancellationToken = default)
    {
        var signals = new List<TechnicalSignal>();
        
        if (!HasSufficientData(candles, 50)) // Need at least 50 candles for RSI
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

        var rsiResults = quotes.GetRsi(14).ToList();
        
        if (rsiResults.Count < 2)
            return signals;

        var currentRsi = rsiResults.Last();
        var previousRsi = rsiResults[^2];
        
        if (currentRsi.Rsi == null || previousRsi.Rsi == null)
            return signals;

        var currentRsiValue = (decimal)currentRsi.Rsi.Value;
        var previousRsiValue = (decimal)previousRsi.Rsi.Value;

        // RSI > 70 (اشباع خرید)
        if (currentRsiValue > 70)
        {
            signals.Add(CreateSignal(symbol, timeFrame, "RSI > 70 (اشباع خرید)", SignalType.Sell, DetailedSignalType.RSIOverBought, currentRsiValue));
        }

        // RSI < 30 (اشباع فروش)
        if (currentRsiValue < 30)
        {
            signals.Add(CreateSignal(symbol, timeFrame, "RSI < 30 (اشباع فروش)", SignalType.Buy, DetailedSignalType.RSIOverSold, currentRsiValue));
        }

        // عبور RSI از سطح 50 به بالا
        if (previousRsiValue <= 50 && currentRsiValue > 50)
        {
            signals.Add(CreateSignal(symbol, timeFrame, "عبور RSI از سطح 50 به بالا", SignalType.Buy, DetailedSignalType.RSICenterLineCrossUp, currentRsiValue));
        }

        // عبور RSI از سطح 50 به پایین
        if (previousRsiValue >= 50 && currentRsiValue < 50)
        {
            signals.Add(CreateSignal(symbol, timeFrame, "عبور RSI از سطح 50 به پایین", SignalType.Sell, DetailedSignalType.RSICenterLineCrossDown, currentRsiValue));
        }

        // واگرایی مثبت (Bullish Divergence)
        if (DetectBullishDivergence(orderedCandles, rsiResults))
        {
            signals.Add(CreateSignal(symbol, timeFrame, "واگرایی مثبت RSI", SignalType.Buy, DetailedSignalType.RSIDivergenceBullish, currentRsiValue));
        }

        // واگرایی منفی (Bearish Divergence)
        if (DetectBearishDivergence(orderedCandles, rsiResults))
        {
            signals.Add(CreateSignal(symbol, timeFrame, "واگرایی منفی RSI", SignalType.Sell, DetailedSignalType.RSIDivergenceBearish, currentRsiValue));
        }

        return signals;
    }

    private bool DetectBullishDivergence(List<CandleBase> candles, List<RsiResult> rsiResults)
    {
        if (candles.Count < 10 || rsiResults.Count < 10)
            return false;

        // Simple bullish divergence detection
        // Price makes lower low, but RSI makes higher low
        var recentCandles = candles.TakeLast(10).ToList();
        var recentRsi = rsiResults.TakeLast(10).Where(r => r.Rsi.HasValue).ToList();

        if (recentRsi.Count < 5)
            return false;

        var firstLow = recentCandles.Take(5).Min(c => c.Low);
        var secondLow = recentCandles.Skip(5).Min(c => c.Low);
        
        var firstRsiLow = recentRsi.Take(5).Min(r => r.Rsi!.Value);
        var secondRsiLow = recentRsi.Skip(5).Min(r => r.Rsi!.Value);

        return secondLow < firstLow && secondRsiLow > firstRsiLow;
    }

    private bool DetectBearishDivergence(List<CandleBase> candles, List<RsiResult> rsiResults)
    {
        if (candles.Count < 10 || rsiResults.Count < 10)
            return false;

        // Simple bearish divergence detection
        // Price makes higher high, but RSI makes lower high
        var recentCandles = candles.TakeLast(10).ToList();
        var recentRsi = rsiResults.TakeLast(10).Where(r => r.Rsi.HasValue).ToList();

        if (recentRsi.Count < 5)
            return false;

        var firstHigh = recentCandles.Take(5).Max(c => c.High);
        var secondHigh = recentCandles.Skip(5).Max(c => c.High);
        
        var firstRsiHigh = recentRsi.Take(5).Max(r => r.Rsi!.Value);
        var secondRsiHigh = recentRsi.Skip(5).Max(r => r.Rsi!.Value);

        return secondHigh > firstHigh && secondRsiHigh < firstRsiHigh;
    }
}