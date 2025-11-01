using ApplicationLayer.Dto.MarketAnalysis;
using DomainLayer.Entities;

namespace ApplicationLayer.Interfaces.Services;

public interface IMarketAnalysisService
{
    Task<MarketAnalysisResult> AnalyzeMarketAsync(MarketAnalysisRequestDto request, CancellationToken cancellationToken = default);
    Task<List<TradingSignal>> GetActiveSignalsAsync(string? symbol = null, string? timeframe = null, CancellationToken cancellationToken = default);
    Task<bool> ValidateAnalysisRequestAsync(MarketAnalysisRequestDto request, CancellationToken cancellationToken = default);
}