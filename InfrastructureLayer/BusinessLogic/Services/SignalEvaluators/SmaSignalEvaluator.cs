using ApplicationLayer.Common;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Skender.Stock.Indicators;

namespace InfrastructureLayer.BusinessLogic.Services.SignalEvaluators;

[InjectAsScoped]
public class SmaSignalEvaluator : BaseSignalEvaluator
{
    public override string IndicatorCategory => "SMA";
    public override string IndicatorName => "Simple Moving Average";

    public override async Task<IEnumerable<TechnicalSignal>> EvaluateAsync(
        string symbol, 
        string timeFrame, 
        IEnumerable<CandleBase> candles,
        CancellationToken cancellationToken = default)
    {
        var signals = new List<TechnicalSignal>();
        
        if (!HasSufficientData(candles, 210)) // Need at least 210 candles for SMA200
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

        var sma10 = quotes.GetSma(10).ToList();
        var sma20 = quotes.GetSma(20).ToList();
        var sma50 = quotes.GetSma(50).ToList();
        var sma100 = quotes.GetSma(100).ToList();
        var sma200 = quotes.GetSma(200).ToList();

        if (sma200.Count < 2)
            return signals;

        var currentPrice = orderedCandles.Last().Close;
        var previousPrice = orderedCandles[^2].Close;

        var currentSma10 = sma10.LastOrDefault()?.Sma;
        var previousSma10 = sma10.Count > 1 ? sma10[^2].Sma : null;
        
        var currentSma20 = sma20.LastOrDefault()?.Sma;
        var previousSma20 = sma20.Count > 1 ? sma20[^2].Sma : null;
        
        var currentSma50 = sma50.LastOrDefault()?.Sma;
        var previousSma50 = sma50.Count > 1 ? sma50[^2].Sma : null;
        
        var currentSma100 = sma100.LastOrDefault()?.Sma;
        var previousSma100 = sma100.Count > 1 ? sma100[^2].Sma : null;
        
        var currentSma200 = sma200.LastOrDefault()?.Sma;
        var previousSma200 = sma200.Count > 1 ? sma200[^2].Sma : null;

        // قیمت بالای SMA10
        if (currentSma10.HasValue && currentPrice > (decimal)currentSma10.Value)
        {
            signals.Add(CreateSignal(symbol, timeFrame, "قیمت بالای SMA10", SignalType.Buy, DetailedSignalType.MovingAverageBullishBreakout, (decimal)currentSma10.Value));
        }

        // قیمت پایین SMA10
        if (currentSma10.HasValue && currentPrice < (decimal)currentSma10.Value)
        {
            signals.Add(CreateSignal(symbol, timeFrame, "قیمت پایین SMA10", SignalType.Sell, DetailedSignalType.MovingAverageBearishBreakdown, (decimal)currentSma10.Value));
        }

        // شکست SMA20 به بالا
        if (currentSma20.HasValue && previousSma20.HasValue && 
            previousPrice <= (decimal)previousSma20.Value && currentPrice > (decimal)currentSma20.Value)
        {
            signals.Add(CreateSignal(symbol, timeFrame, "شکست SMA20 به بالا", SignalType.Buy, DetailedSignalType.MovingAverageBullishBreakout, (decimal)currentSma20.Value));
        }

        // شکست SMA20 به پایین
        if (currentSma20.HasValue && previousSma20.HasValue && 
            previousPrice >= (decimal)previousSma20.Value && currentPrice < (decimal)currentSma20.Value)
        {
            signals.Add(CreateSignal(symbol, timeFrame, "شکست SMA20 به پایین", SignalType.Sell, DetailedSignalType.MovingAverageBearishBreakdown, (decimal)currentSma20.Value));
        }

        // قیمت بالای SMA50
        if (currentSma50.HasValue && currentPrice > (decimal)currentSma50.Value)
        {
            signals.Add(CreateSignal(symbol, timeFrame, "قیمت بالای SMA50", SignalType.Buy, DetailedSignalType.MovingAverageBullishBreakout, (decimal)currentSma50.Value));
        }

        // قیمت پایین SMA50
        if (currentSma50.HasValue && currentPrice < (decimal)currentSma50.Value)
        {
            signals.Add(CreateSignal(symbol, timeFrame, "قیمت پایین SMA50", SignalType.Sell, DetailedSignalType.MovingAverageBearishBreakdown, (decimal)currentSma50.Value));
        }

        // شکست SMA100 به بالا
        if (currentSma100.HasValue && previousSma100.HasValue && 
            previousPrice <= (decimal)previousSma100.Value && currentPrice > (decimal)currentSma100.Value)
        {
            signals.Add(CreateSignal(symbol, timeFrame, "شکست SMA100 به بالا", SignalType.Buy, DetailedSignalType.MovingAverageBullishBreakout, (decimal)currentSma100.Value));
        }

        // شکست SMA100 به پایین
        if (currentSma100.HasValue && previousSma100.HasValue && 
            previousPrice >= (decimal)previousSma100.Value && currentPrice < (decimal)currentSma100.Value)
        {
            signals.Add(CreateSignal(symbol, timeFrame, "شکست SMA100 به پایین", SignalType.Sell, DetailedSignalType.MovingAverageBearishBreakdown, (decimal)currentSma100.Value));
        }

        // قیمت بالای SMA200
        if (currentSma200.HasValue && currentPrice > (decimal)currentSma200.Value)
        {
            signals.Add(CreateSignal(symbol, timeFrame, "قیمت بالای SMA200", SignalType.Buy, DetailedSignalType.MovingAverageSupportBounce, (decimal)currentSma200.Value));
        }

        // قیمت پایین SMA200
        if (currentSma200.HasValue && currentPrice < (decimal)currentSma200.Value)
        {
            signals.Add(CreateSignal(symbol, timeFrame, "قیمت پایین SMA200", SignalType.Sell, DetailedSignalType.MovingAverageResistanceRejection, (decimal)currentSma200.Value));
        }

        return signals;
    }
}