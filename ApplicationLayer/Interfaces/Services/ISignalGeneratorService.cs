using ApplicationLayer.Dto.MarketAnalysis;
using DomainLayer.Entities;

namespace ApplicationLayer.Interfaces.Services;

public interface ISignalGeneratorService
{
    Task<List<TradingSignal>> GenerateSignalsAsync(
        List<string> symbols, 
        List<AnalysisConditionDto> conditions, 
        AnalysisFiltersDto filters,
        AnalysisPreferencesDto preferences,
        CancellationToken cancellationToken = default);

    Task<TradingSignal?> EvaluateConditionsAsync(
        string symbol, 
        string timeframe,
        List<AnalysisConditionDto> conditions,
        AnalysisPreferencesDto preferences,
        CancellationToken cancellationToken = default);

    Task<bool> ValidateSignalConfirmationAsync(
        TradingSignal signal,
        List<ConditionConfirmationDto> confirmations,
        CancellationToken cancellationToken = default);

    Task<decimal> CalculateSignalConfidenceAsync(
        TradingSignal signal,
        List<AnalysisConditionDto> conditions,
        CancellationToken cancellationToken = default);
}