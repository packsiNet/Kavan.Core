using DomainLayer.Entities;

namespace ApplicationLayer.Interfaces.Services;

public interface ISignalEvaluator
{
    string IndicatorCategory { get; }
    string IndicatorName { get; }
    IEnumerable<string> SupportedTimeFrames { get; }
    
    Task<IEnumerable<TechnicalSignal>> EvaluateAsync(
        string symbol, 
        string timeFrame, 
        IEnumerable<CandleBase> candles,
        CancellationToken cancellationToken = default);
}

public interface ISignalEvaluatorFactory
{
    IEnumerable<ISignalEvaluator> GetAllEvaluators();
    IEnumerable<ISignalEvaluator> GetEvaluatorsByCategory(string category);
    ISignalEvaluator? GetEvaluator(string category, string name);
}