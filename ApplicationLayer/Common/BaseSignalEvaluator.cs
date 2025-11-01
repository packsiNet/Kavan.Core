using ApplicationLayer.Interfaces.Services;
using DomainLayer.Entities;

namespace ApplicationLayer.Common;

public abstract class BaseSignalEvaluator : ISignalEvaluator
{
    public abstract string IndicatorCategory { get; }
    public abstract string IndicatorName { get; }
    public virtual IEnumerable<string> SupportedTimeFrames => new[] { "1m", "5m", "1h", "4h", "1d" };

    public abstract Task<IEnumerable<TechnicalSignal>> EvaluateAsync(
        string symbol, 
        string timeFrame, 
        IEnumerable<CandleBase> candles,
        CancellationToken cancellationToken = default);

    protected TechnicalSignal CreateSignal(
        string symbol,
        string timeFrame,
        string conditionTitle,
        SignalType signalType,
        DetailedSignalType detailedSignalType,
        decimal? value = null,
        string additionalData = null)
    {
        return new TechnicalSignal
        {
            Symbol = symbol,
            IndicatorCategory = IndicatorCategory,
            IndicatorName = IndicatorName,
            ConditionTitle = conditionTitle,
            SignalType = signalType,
            DetailedSignalType = detailedSignalType,
            TimeFrame = timeFrame,
            CreatedAt = DateTime.UtcNow,
            Value = value,
            AdditionalData = additionalData
        };
    }

    protected bool HasSufficientData(IEnumerable<CandleBase> candles, int requiredCount)
    {
        return candles.Count() >= requiredCount;
    }

    protected IEnumerable<CandleBase> GetOrderedCandles(IEnumerable<CandleBase> candles)
    {
        return candles.OrderBy(c => c.OpenTime);
    }
}