using ApplicationLayer.Common;
using ApplicationLayer.Interfaces.Services;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Skender.Stock.Indicators;

namespace InfrastructureLayer.BusinessLogic.Services.TechnicalSignals;

[InjectAsScoped]
public class CandlestickPatternSignalEvaluator : BaseSignalEvaluator
{
    public override string IndicatorCategory => "Candlestick Patterns";
    public override string IndicatorName => "Candlestick Pattern Recognition";

    public override async Task<IEnumerable<TechnicalSignal>> EvaluateAsync(
        string symbol,
        string timeFrame,
        IEnumerable<CandleBase> candles,
        CancellationToken cancellationToken = default)
    {
        var signals = new List<TechnicalSignal>();
        
        if (!HasSufficientData(candles, 10)) // Need at least 10 periods for pattern recognition
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

        if (quotes.Count < 3)
            return signals;

        var current = quotes.Last();
        var previous = quotes[quotes.Count - 2];
        var twoBefore = quotes.Count >= 3 ? quotes[quotes.Count - 3] : null;

        // 1. Doji Pattern
        var dojiSignal = DetectDoji(current, symbol, timeFrame);
        if (dojiSignal != null) signals.Add(dojiSignal);

        // 2. Hammer Pattern
        var hammerSignal = DetectHammer(current, symbol, timeFrame);
        if (hammerSignal != null) signals.Add(hammerSignal);

        // 3. Shooting Star Pattern
        var shootingStarSignal = DetectShootingStar(current, symbol, timeFrame);
        if (shootingStarSignal != null) signals.Add(shootingStarSignal);

        // 4. Engulfing Pattern
        var engulfingSignal = DetectEngulfing(current, previous, symbol, timeFrame);
        if (engulfingSignal != null) signals.Add(engulfingSignal);

        // 5. Harami Pattern
        var haramiSignal = DetectHarami(current, previous, symbol, timeFrame);
        if (haramiSignal != null) signals.Add(haramiSignal);

        // 6. Morning Star Pattern
        if (twoBefore != null)
        {
            var morningStarSignal = DetectMorningStar(current, previous, twoBefore, symbol, timeFrame);
            if (morningStarSignal != null) signals.Add(morningStarSignal);
        }

        // 7. Evening Star Pattern
        if (twoBefore != null)
        {
            var eveningStarSignal = DetectEveningStar(current, previous, twoBefore, symbol, timeFrame);
            if (eveningStarSignal != null) signals.Add(eveningStarSignal);
        }

        // 8. Three White Soldiers Pattern
        if (quotes.Count >= 3)
        {
            var threeWhiteSoldiersSignal = DetectThreeWhiteSoldiers(current, previous, twoBefore, symbol, timeFrame);
            if (threeWhiteSoldiersSignal != null) signals.Add(threeWhiteSoldiersSignal);
        }

        // 9. Three Black Crows Pattern
        if (quotes.Count >= 3)
        {
            var threeBlackCrowsSignal = DetectThreeBlackCrows(current, previous, twoBefore, symbol, timeFrame);
            if (threeBlackCrowsSignal != null) signals.Add(threeBlackCrowsSignal);
        }

        return signals;
    }

    private TechnicalSignal? DetectDoji(Quote candle, string symbol, string timeFrame)
    {
        var bodySize = Math.Abs(candle.Close - candle.Open);
        var totalRange = candle.High - candle.Low;
        
        // Doji: body is very small compared to total range (less than 10% of range)
        if (totalRange > 0 && bodySize / totalRange < 0.1m)
        {
            return CreateSignal(symbol, timeFrame, "Doji Pattern", SignalType.Neutral, DetailedSignalType.CandlestickDoji, candle.Close,
                $"{{\"Open\":{candle.Open:F4},\"High\":{candle.High:F4},\"Low\":{candle.Low:F4},\"Close\":{candle.Close:F4},\"BodyRatio\":{(bodySize / totalRange * 100):F2}}}");
        }
        
        return null;
    }

    private TechnicalSignal? DetectHammer(Quote candle, string symbol, string timeFrame)
    {
        var bodySize = Math.Abs(candle.Close - candle.Open);
        var lowerShadow = Math.Min(candle.Open, candle.Close) - candle.Low;
        var upperShadow = candle.High - Math.Max(candle.Open, candle.Close);
        var totalRange = candle.High - candle.Low;

        // Hammer: long lower shadow (at least 2x body), small upper shadow, small body
        if (totalRange > 0 && lowerShadow >= bodySize * 2 && upperShadow <= bodySize * 0.5m && bodySize / totalRange < 0.3m)
        {
            return CreateSignal(symbol, timeFrame, "Hammer Pattern", SignalType.Buy, DetailedSignalType.CandlestickHammer, candle.Close,
                $"{{\"Open\":{candle.Open:F4},\"High\":{candle.High:F4},\"Low\":{candle.Low:F4},\"Close\":{candle.Close:F4},\"LowerShadowRatio\":{(lowerShadow / bodySize):F2}}}");
        }
        
        return null;
    }

    private TechnicalSignal? DetectShootingStar(Quote candle, string symbol, string timeFrame)
    {
        var bodySize = Math.Abs(candle.Close - candle.Open);
        var lowerShadow = Math.Min(candle.Open, candle.Close) - candle.Low;
        var upperShadow = candle.High - Math.Max(candle.Open, candle.Close);
        var totalRange = candle.High - candle.Low;

        // Shooting Star: long upper shadow (at least 2x body), small lower shadow, small body
        if (totalRange > 0 && upperShadow >= bodySize * 2 && lowerShadow <= bodySize * 0.5m && bodySize / totalRange < 0.3m)
        {
            return CreateSignal(symbol, timeFrame, "Shooting Star Pattern", SignalType.Sell, DetailedSignalType.CandlestickShootingStar, candle.Close,
                $"{{\"Open\":{candle.Open:F4},\"High\":{candle.High:F4},\"Low\":{candle.Low:F4},\"Close\":{candle.Close:F4},\"UpperShadowRatio\":{(upperShadow / bodySize):F2}}}");
        }
        
        return null;
    }

    private TechnicalSignal? DetectEngulfing(Quote current, Quote previous, string symbol, string timeFrame)
    {
        var currentBody = Math.Abs(current.Close - current.Open);
        var previousBody = Math.Abs(previous.Close - previous.Open);

        // Bullish Engulfing: previous bearish, current bullish and engulfs previous
        if (previous.Close < previous.Open && current.Close > current.Open &&
            current.Open < previous.Close && current.Close > previous.Open &&
            currentBody > previousBody)
        {
            return CreateSignal(symbol, timeFrame, "Bullish Engulfing Pattern", SignalType.Buy, DetailedSignalType.CandlestickEngulfingBullish, current.Close,
                $"{{\"PreviousCandle\":{{\"Open\":{previous.Open:F4},\"Close\":{previous.Close:F4}}},\"CurrentCandle\":{{\"Open\":{current.Open:F4},\"Close\":{current.Close:F4}}}}}");
        }

        // Bearish Engulfing: previous bullish, current bearish and engulfs previous
        if (previous.Close > previous.Open && current.Close < current.Open &&
            current.Open > previous.Close && current.Close < previous.Open &&
            currentBody > previousBody)
        {
            return CreateSignal(symbol, timeFrame, "Bearish Engulfing Pattern", SignalType.Sell, DetailedSignalType.CandlestickEngulfingBearish, current.Close,
                $"{{\"PreviousCandle\":{{\"Open\":{previous.Open:F4},\"Close\":{previous.Close:F4}}},\"CurrentCandle\":{{\"Open\":{current.Open:F4},\"Close\":{current.Close:F4}}}}}");
        }

        return null;
    }

    private TechnicalSignal? DetectHarami(Quote current, Quote previous, string symbol, string timeFrame)
    {
        var currentBody = Math.Abs(current.Close - current.Open);
        var previousBody = Math.Abs(previous.Close - previous.Open);

        // Bullish Harami: previous bearish, current small bullish inside previous
        if (previous.Close < previous.Open && current.Close > current.Open &&
            current.Open > previous.Close && current.Close < previous.Open &&
            currentBody < previousBody * 0.7m)
        {
            return CreateSignal(symbol, timeFrame, "Bullish Harami Pattern", SignalType.Buy, DetailedSignalType.CandlestickBullishHarami, current.Close,
                $"{{\"PreviousCandle\":{{\"Open\":{previous.Open:F4},\"Close\":{previous.Close:F4}}},\"CurrentCandle\":{{\"Open\":{current.Open:F4},\"Close\":{current.Close:F4}}}}}");
        }

        // Bearish Harami: previous bullish, current small bearish inside previous
        if (previous.Close > previous.Open && current.Close < current.Open &&
            current.Open < previous.Close && current.Close > previous.Open &&
            currentBody < previousBody * 0.7m)
        {
            return CreateSignal(symbol, timeFrame, "Bearish Harami Pattern", SignalType.Sell, DetailedSignalType.CandlestickBearishHarami, current.Close,
                $"{{\"PreviousCandle\":{{\"Open\":{previous.Open:F4},\"Close\":{previous.Close:F4}}},\"CurrentCandle\":{{\"Open\":{current.Open:F4},\"Close\":{current.Close:F4}}}}}");
        }

        return null;
    }

    private TechnicalSignal? DetectMorningStar(Quote current, Quote middle, Quote first, string symbol, string timeFrame)
    {
        // Morning Star: bearish candle, small body (star), bullish candle
        if (first.Close < first.Open && // First candle bearish
            current.Close > current.Open && // Third candle bullish
            Math.Abs(middle.Close - middle.Open) < Math.Abs(first.Close - first.Open) * 0.5m && // Middle candle small
            current.Close > (first.Open + first.Close) / 2) // Third candle closes above midpoint of first
        {
            return CreateSignal(symbol, timeFrame, "Morning Star Pattern", SignalType.Buy, DetailedSignalType.CandlestickMorningStar, current.Close,
                $"{{\"FirstCandle\":{{\"Open\":{first.Open:F4},\"Close\":{first.Close:F4}}},\"MiddleCandle\":{{\"Open\":{middle.Open:F4},\"Close\":{middle.Close:F4}}},\"CurrentCandle\":{{\"Open\":{current.Open:F4},\"Close\":{current.Close:F4}}}}}");
        }

        return null;
    }

    private TechnicalSignal? DetectEveningStar(Quote current, Quote middle, Quote first, string symbol, string timeFrame)
    {
        // Evening Star: bullish candle, small body (star), bearish candle
        if (first.Close > first.Open && // First candle bullish
            current.Close < current.Open && // Third candle bearish
            Math.Abs(middle.Close - middle.Open) < Math.Abs(first.Close - first.Open) * 0.5m && // Middle candle small
            current.Close < (first.Open + first.Close) / 2) // Third candle closes below midpoint of first
        {
            return CreateSignal(symbol, timeFrame, "Evening Star Pattern", SignalType.Sell, DetailedSignalType.CandlestickEveningStar, current.Close,
                $"{{\"FirstCandle\":{{\"Open\":{first.Open:F4},\"Close\":{first.Close:F4}}},\"MiddleCandle\":{{\"Open\":{middle.Open:F4},\"Close\":{middle.Close:F4}}},\"CurrentCandle\":{{\"Open\":{current.Open:F4},\"Close\":{current.Close:F4}}}}}");
        }

        return null;
    }

    private TechnicalSignal? DetectThreeWhiteSoldiers(Quote current, Quote middle, Quote first, string symbol, string timeFrame)
    {
        // Three White Soldiers: three consecutive bullish candles with higher closes
        if (first.Close > first.Open && middle.Close > middle.Open && current.Close > current.Open &&
            middle.Close > first.Close && current.Close > middle.Close &&
            middle.Open > first.Open && current.Open > middle.Open)
        {
            return CreateSignal(symbol, timeFrame, "Three White Soldiers Pattern", SignalType.Buy, DetailedSignalType.CandlestickThreeWhiteSoldiers, current.Close,
                $"{{\"FirstCandle\":{{\"Open\":{first.Open:F4},\"Close\":{first.Close:F4}}},\"MiddleCandle\":{{\"Open\":{middle.Open:F4},\"Close\":{middle.Close:F4}}},\"CurrentCandle\":{{\"Open\":{current.Open:F4},\"Close\":{current.Close:F4}}}}}");
        }

        return null;
    }

    private TechnicalSignal? DetectThreeBlackCrows(Quote current, Quote middle, Quote first, string symbol, string timeFrame)
    {
        // Three Black Crows: three consecutive bearish candles with lower closes
        if (first.Close < first.Open && middle.Close < middle.Open && current.Close < current.Open &&
            middle.Close < first.Close && current.Close < middle.Close &&
            middle.Open < first.Open && current.Open < middle.Open)
        {
            return CreateSignal(symbol, timeFrame, "Three Black Crows Pattern", SignalType.Sell, DetailedSignalType.CandlestickThreeBlackCrows, current.Close,
                $"{{\"FirstCandle\":{{\"Open\":{first.Open:F4},\"Close\":{first.Close:F4}}},\"MiddleCandle\":{{\"Open\":{middle.Open:F4},\"Close\":{middle.Close:F4}}},\"CurrentCandle\":{{\"Open\":{current.Open:F4},\"Close\":{current.Close:F4}}}}}");
        }

        return null;
    }
}