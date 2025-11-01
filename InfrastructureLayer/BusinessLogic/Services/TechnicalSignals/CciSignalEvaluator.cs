using ApplicationLayer.Common;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Skender.Stock.Indicators;

namespace InfrastructureLayer.BusinessLogic.Services.TechnicalSignals;

[InjectAsScoped]
public class CciSignalEvaluator : BaseSignalEvaluator
{
    public override string IndicatorCategory => "CCI";
    public override string IndicatorName => "Commodity Channel Index";

    public override async Task<IEnumerable<TechnicalSignal>> EvaluateAsync(
        string symbol,
        string timeFrame,
        IEnumerable<CandleBase> candles,
        CancellationToken cancellationToken = default)
    {
        var signals = new List<TechnicalSignal>();

        if (!HasSufficientData(candles, 25)) // Need at least 25 candles for CCI calculations
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

        var cci = quotes.GetCci(20).ToList(); // 20-period CCI

        if (cci.Count < 3) return signals;

        // 1. CCI Overbought Signal
        var overboughtSignal = EvaluateCciOverboughtSignal(cci, symbol, timeFrame);
        if (overboughtSignal != null) signals.Add(overboughtSignal);

        // 2. CCI Oversold Signal
        var oversoldSignal = EvaluateCciOversoldSignal(cci, symbol, timeFrame);
        if (oversoldSignal != null) signals.Add(oversoldSignal);

        // 3. CCI Zero Line Cross Above
        var zeroCrossAboveSignal = EvaluateCciZeroCrossAboveSignal(cci, symbol, timeFrame);
        if (zeroCrossAboveSignal != null) signals.Add(zeroCrossAboveSignal);

        // 4. CCI Zero Line Cross Below
        var zeroCrossBelowSignal = EvaluateCciZeroCrossBelowSignal(cci, symbol, timeFrame);
        if (zeroCrossBelowSignal != null) signals.Add(zeroCrossBelowSignal);

        // 5. CCI Bullish Divergence
        var bullishDivergenceSignal = EvaluateCciBullishDivergenceSignal(cci, orderedCandles, symbol, timeFrame);
        if (bullishDivergenceSignal != null) signals.Add(bullishDivergenceSignal);

        // 6. CCI Bearish Divergence
        var bearishDivergenceSignal = EvaluateCciBearishDivergenceSignal(cci, orderedCandles, symbol, timeFrame);
        if (bearishDivergenceSignal != null) signals.Add(bearishDivergenceSignal);

        return signals;
    }

    private TechnicalSignal? EvaluateCciOverboughtSignal(List<CciResult> cci, string symbol, string timeFrame)
    {
        var current = cci.Last();
        var previous = cci[cci.Count - 2];
        
        if (!current.Cci.HasValue || !previous.Cci.HasValue) return null;

        var currentCci = (decimal)current.Cci.Value;
        var previousCci = (decimal)previous.Cci.Value;

        // CCI crosses above +100 (overbought)
        if (previousCci <= 100 && currentCci > 100)
        {
            return CreateSignal(
                symbol: symbol,
                timeFrame: timeFrame,
                conditionTitle: "CCI Overbought",
                signalType: SignalType.Sell,
                detailedSignalType: DetailedSignalType.CciOverbought,
                value: currentCci,
                additionalData: $"{{\"CCI\":{currentCci:F2},\"Level\":\"Overbought\",\"Threshold\":100}}"
            );
        }

        return null;
    }

    private TechnicalSignal? EvaluateCciOversoldSignal(List<CciResult> cci, string symbol, string timeFrame)
    {
        var current = cci.Last();
        var previous = cci[cci.Count - 2];
        
        if (!current.Cci.HasValue || !previous.Cci.HasValue) return null;

        var currentCci = (decimal)current.Cci.Value;
        var previousCci = (decimal)previous.Cci.Value;

        // CCI crosses below -100 (oversold)
        if (previousCci >= -100 && currentCci < -100)
        {
            return CreateSignal(
                symbol: symbol,
                timeFrame: timeFrame,
                conditionTitle: "CCI Oversold",
                signalType: SignalType.Buy,
                detailedSignalType: DetailedSignalType.CciOversold,
                value: currentCci,
                additionalData: $"{{\"CCI\":{currentCci:F2},\"Level\":\"Oversold\",\"Threshold\":-100}}"
            );
        }

        return null;
    }

    private TechnicalSignal? EvaluateCciZeroCrossAboveSignal(List<CciResult> cci, string symbol, string timeFrame)
    {
        var current = cci.Last();
        var previous = cci[cci.Count - 2];
        
        if (!current.Cci.HasValue || !previous.Cci.HasValue) return null;

        var currentCci = (decimal)current.Cci.Value;
        var previousCci = (decimal)previous.Cci.Value;

        // CCI crosses above zero line
        if (previousCci <= 0 && currentCci > 0)
        {
            return CreateSignal(
                symbol: symbol,
                timeFrame: timeFrame,
                conditionTitle: "CCI Zero Line Cross Above",
                signalType: SignalType.Buy,
                detailedSignalType: DetailedSignalType.CciZeroCrossAbove,
                value: currentCci,
                additionalData: $"{{\"CCI\":{currentCci:F2},\"CrossDirection\":\"Above\",\"ZeroLine\":0}}"
            );
        }

        return null;
    }

    private TechnicalSignal? EvaluateCciZeroCrossBelowSignal(List<CciResult> cci, string symbol, string timeFrame)
    {
        var current = cci.Last();
        var previous = cci[cci.Count - 2];
        
        if (!current.Cci.HasValue || !previous.Cci.HasValue) return null;

        var currentCci = (decimal)current.Cci.Value;
        var previousCci = (decimal)previous.Cci.Value;

        // CCI crosses below zero line
        if (previousCci >= 0 && currentCci < 0)
        {
            return CreateSignal(
                symbol: symbol,
                timeFrame: timeFrame,
                conditionTitle: "CCI Zero Line Cross Below",
                signalType: SignalType.Sell,
                detailedSignalType: DetailedSignalType.CciZeroCrossBelow,
                value: currentCci,
                additionalData: $"{{\"CCI\":{currentCci:F2},\"CrossDirection\":\"Below\",\"ZeroLine\":0}}"
            );
        }

        return null;
    }

    private TechnicalSignal? EvaluateCciBullishDivergenceSignal(List<CciResult> cci, List<CandleBase> candles, string symbol, string timeFrame)
    {
        if (cci.Count < 10 || candles.Count < 10) return null;

        // Look for bullish divergence: price makes lower lows while CCI makes higher lows
        var recentCandles = candles.TakeLast(10).ToList();
        var recentCci = cci.TakeLast(10).ToList();

        // Find recent low in price
        var priceMinIndex = 0;
        var priceMin = recentCandles[0].Low;
        for (int i = 1; i < recentCandles.Count; i++)
        {
            if (recentCandles[i].Low < priceMin)
            {
                priceMin = recentCandles[i].Low;
                priceMinIndex = i;
            }
        }

        // Find recent low in CCI
        var cciMinIndex = 0;
        var cciMin = recentCci[0].Cci ?? 0;
        for (int i = 1; i < recentCci.Count; i++)
        {
            if (recentCci[i].Cci.HasValue && recentCci[i].Cci.Value < cciMin)
            {
                cciMin = recentCci[i].Cci.Value;
                cciMinIndex = i;
            }
        }

        // Check for divergence: price low is recent but CCI low is older
        if (priceMinIndex > cciMinIndex + 2 && recentCci.Last().Cci.HasValue)
        {
            var currentCci = (decimal)recentCci.Last().Cci.Value;
            
            return CreateSignal(
                symbol: symbol,
                timeFrame: timeFrame,
                conditionTitle: "CCI Bullish Divergence",
                signalType: SignalType.Buy,
                detailedSignalType: DetailedSignalType.CCIBullishDivergence,
                value: currentCci,
                additionalData: $"{{\"CCI\":{currentCci:F2},\"DivergenceType\":\"Bullish\",\"PriceLow\":{priceMin:F2},\"CCILow\":{cciMin:F2}}}"
            );
        }

        return null;
    }

    private TechnicalSignal? EvaluateCciBearishDivergenceSignal(List<CciResult> cci, List<CandleBase> candles, string symbol, string timeFrame)
    {
        if (cci.Count < 10 || candles.Count < 10) return null;

        // Look for bearish divergence: price makes higher highs while CCI makes lower highs
        var recentCandles = candles.TakeLast(10).ToList();
        var recentCci = cci.TakeLast(10).ToList();

        // Find recent high in price
        var priceMaxIndex = 0;
        var priceMax = recentCandles[0].High;
        for (int i = 1; i < recentCandles.Count; i++)
        {
            if (recentCandles[i].High > priceMax)
            {
                priceMax = recentCandles[i].High;
                priceMaxIndex = i;
            }
        }

        // Find recent high in CCI
        var cciMaxIndex = 0;
        var cciMax = recentCci[0].Cci ?? 0;
        for (int i = 1; i < recentCci.Count; i++)
        {
            if (recentCci[i].Cci.HasValue && recentCci[i].Cci.Value > cciMax)
            {
                cciMax = recentCci[i].Cci.Value;
                cciMaxIndex = i;
            }
        }

        // Check for divergence: price high is recent but CCI high is older
        if (priceMaxIndex > cciMaxIndex + 2 && recentCci.Last().Cci.HasValue)
        {
            var currentCci = (decimal)recentCci.Last().Cci.Value;
            
            return CreateSignal(
                symbol: symbol,
                timeFrame: timeFrame,
                conditionTitle: "CCI Bearish Divergence",
                signalType: SignalType.Sell,
                detailedSignalType: DetailedSignalType.CCIBearishDivergence,
                value: currentCci,
                additionalData: $"{{\"CCI\":{currentCci:F2},\"DivergenceType\":\"Bearish\",\"PriceHigh\":{priceMax:F2},\"CCIHigh\":{cciMax:F2}}}"
            );
        }

        return null;
    }
}