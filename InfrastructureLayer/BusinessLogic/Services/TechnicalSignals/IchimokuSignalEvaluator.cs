using ApplicationLayer.Common;
using ApplicationLayer.Interfaces.Services;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Skender.Stock.Indicators;

namespace InfrastructureLayer.BusinessLogic.Services.TechnicalSignals;

[InjectAsScoped]
public class IchimokuSignalEvaluator : BaseSignalEvaluator
{
    public override string IndicatorCategory => "Ichimoku";
    public override string IndicatorName => "Ichimoku Kinko Hyo";

    public override async Task<IEnumerable<TechnicalSignal>> EvaluateAsync(
        string symbol,
        string timeFrame,
        IEnumerable<CandleBase> candles,
        CancellationToken cancellationToken = default)
    {
        var signals = new List<TechnicalSignal>();
        
        if (!HasSufficientData(candles, 52)) // Ichimoku needs at least 52 periods
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

        // Calculate Ichimoku with standard parameters (9, 26, 52)
        var ichimokuResults = quotes.GetIchimoku(9, 26, 52).ToList();
        
        if (ichimokuResults.Count < 2)
            return signals;

        var current = ichimokuResults.Last();
        var previous = ichimokuResults[ichimokuResults.Count - 2];
        var currentPrice = orderedCandles.Last().Close;
        var previousPrice = orderedCandles[orderedCandles.Count - 2].Close;

        // 1. Tenkan-Sen Cross Signal
        if (current.TenkanSen.HasValue && previous.TenkanSen.HasValue)
        {
            if (previousPrice <= previous.TenkanSen && currentPrice > current.TenkanSen)
            {
                signals.Add(CreateSignal(symbol, timeFrame, "Tenkan-Sen Bullish Cross", SignalType.Buy, DetailedSignalType.IchimokuTenkanSenBullishCross, current.TenkanSen));
            }
            else if (previousPrice >= previous.TenkanSen && currentPrice < current.TenkanSen)
            {
                signals.Add(CreateSignal(symbol, timeFrame, "Tenkan-Sen Bearish Cross", SignalType.Sell, DetailedSignalType.IchimokuTenkanSenBearishCross, current.TenkanSen));
            }
        }

        // 2. Kijun-Sen Cross Signal
        if (current.KijunSen.HasValue && previous.KijunSen.HasValue)
        {
            if (previousPrice <= previous.KijunSen && currentPrice > current.KijunSen)
            {
                signals.Add(CreateSignal(symbol, timeFrame, "Kijun-Sen Bullish Cross", SignalType.Buy, DetailedSignalType.IchimokuKijunSenBullishCross, current.KijunSen));
            }
            else if (previousPrice >= previous.KijunSen && currentPrice < current.KijunSen)
            {
                signals.Add(CreateSignal(symbol, timeFrame, "Kijun-Sen Bearish Cross", SignalType.Sell, DetailedSignalType.IchimokuKijunSenBearishCross, current.KijunSen));
            }
        }

        // 3. Senkou Span Cross Signal (TK Cross)
        if (current.TenkanSen.HasValue && current.KijunSen.HasValue && 
            previous.TenkanSen.HasValue && previous.KijunSen.HasValue)
        {
            if (previous.TenkanSen <= previous.KijunSen && current.TenkanSen > current.KijunSen)
            {
                signals.Add(CreateSignal(symbol, timeFrame, "Tenkan-Kijun Bullish Cross", SignalType.Buy, DetailedSignalType.IchimokuTenkanKijunBullishCross, current.TenkanSen));
            }
            else if (previous.TenkanSen >= previous.KijunSen && current.TenkanSen < current.KijunSen)
            {
                signals.Add(CreateSignal(symbol, timeFrame, "Tenkan-Kijun Bearish Cross", SignalType.Sell, DetailedSignalType.IchimokuTenkanKijunBearishCross, current.TenkanSen));
            }
        }

        // 4. Kumo (Cloud) Breakout Signal
        if (current.SenkouSpanA.HasValue && current.SenkouSpanB.HasValue)
        {
            var cloudTop = Math.Max(current.SenkouSpanA.Value, current.SenkouSpanB.Value);
            var cloudBottom = Math.Min(current.SenkouSpanA.Value, current.SenkouSpanB.Value);
            
            if (previousPrice <= cloudTop && currentPrice > cloudTop)
            {
                signals.Add(CreateSignal(symbol, timeFrame, "Kumo Bullish Breakout", SignalType.Buy, DetailedSignalType.IchimokuKumoBullishBreakout, cloudTop, 
                    $"{{\"CloudTop\":{cloudTop:F4},\"CloudBottom\":{cloudBottom:F4}}}"));
            }
            else if (previousPrice >= cloudBottom && currentPrice < cloudBottom)
            {
                signals.Add(CreateSignal(symbol, timeFrame, "Kumo Bearish Breakdown", SignalType.Sell, DetailedSignalType.IchimokuKumoBearishBreakdown, cloudBottom,
                    $"{{\"CloudTop\":{cloudTop:F4},\"CloudBottom\":{cloudBottom:F4}}}"));
            }
        }

        // 5. Chikou Span Confirmation Signal
        if (ichimokuResults.Count >= 26) // Need at least 26 periods for Chikou Span comparison
        {
            var chikouIndex = ichimokuResults.Count - 26;
            if (chikouIndex >= 0 && chikouIndex < orderedCandles.Count)
            {
                var chikouPrice = orderedCandles[chikouIndex].Close;
                if (currentPrice > chikouPrice)
                {
                    signals.Add(CreateSignal(symbol, timeFrame, "Chikou Span Bullish Confirmation", SignalType.Buy, DetailedSignalType.IchimokuChikouSpanBullishConfirmation, currentPrice,
                         $"{{\"ChikouPrice\":{chikouPrice:F4}}}"));
                }
                else if (currentPrice < chikouPrice)
                {
                    signals.Add(CreateSignal(symbol, timeFrame, "Chikou Span Bearish Confirmation", SignalType.Sell, DetailedSignalType.IchimokuChikouSpanBearishConfirmation, currentPrice,
                         $"{{\"ChikouPrice\":{chikouPrice:F4}}}"));
                }
            }
        }

        // 6. Bullish Cloud Signal
        if (current.SenkouSpanA.HasValue && current.SenkouSpanB.HasValue)
        {
            if (current.SenkouSpanA > current.SenkouSpanB)
            {
                signals.Add(CreateSignal(symbol, timeFrame, "Bullish Cloud Formation", SignalType.Buy, DetailedSignalType.IchimokuBullishCloudFormation, current.SenkouSpanA,
                    $"{{\"SenkouSpanA\":{current.SenkouSpanA:F4},\"SenkouSpanB\":{current.SenkouSpanB:F4}}}"));
            }
        }

        // 7. Bearish Cloud Signal
        if (current.SenkouSpanA.HasValue && current.SenkouSpanB.HasValue)
        {
            if (current.SenkouSpanA < current.SenkouSpanB)
            {
                signals.Add(CreateSignal(symbol, timeFrame, "Bearish Cloud Formation", SignalType.Sell, DetailedSignalType.IchimokuBearishCloudFormation, current.SenkouSpanB,
                    $"{{\"SenkouSpanA\":{current.SenkouSpanA:F4},\"SenkouSpanB\":{current.SenkouSpanB:F4}}}"));
            }
        }

        // 8. Flat Cloud Signal (Neutral/Consolidation)
        if (current.SenkouSpanA.HasValue && current.SenkouSpanB.HasValue)
        {
            var spanDifference = Math.Abs(current.SenkouSpanA.Value - current.SenkouSpanB.Value);
            var averageSpan = (current.SenkouSpanA.Value + current.SenkouSpanB.Value) / 2;
            var percentageDifference = (spanDifference / averageSpan) * 100;

            // If the difference between spans is less than 0.5%, consider it flat
            if (percentageDifference < 0.5m)
            {
                signals.Add(CreateSignal(symbol, timeFrame, "Flat Cloud Formation", SignalType.Neutral, DetailedSignalType.IchimokuFlatCloudFormation, averageSpan,
                    $"{{\"SenkouSpanA\":{current.SenkouSpanA:F4},\"SenkouSpanB\":{current.SenkouSpanB:F4},\"Difference\":{percentageDifference:F2}}}"));
            }
        }

        return signals;
    }
}