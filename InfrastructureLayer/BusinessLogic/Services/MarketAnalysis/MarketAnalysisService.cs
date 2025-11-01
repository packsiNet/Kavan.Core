using ApplicationLayer.Dto.MarketAnalysis;
using ApplicationLayer.Interfaces;
using ApplicationLayer.Interfaces.Services;
using AutoMapper;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;

namespace InfrastructureLayer.BusinessLogic.Services.MarketAnalysis;

[InjectAsScoped]
public class MarketAnalysisService(
    IUnitOfWork unitOfWork,
    IRepository<Cryptocurrency> cryptocurrencyRepository,
    ISignalGeneratorService signalGeneratorService,
    ITechnicalIndicatorService technicalIndicatorService,
    IMapper mapper
) : IMarketAnalysisService
{
    public async Task<MarketAnalysisResult> AnalyzeMarketAsync(MarketAnalysisRequestDto request, CancellationToken cancellationToken = default)
    {
        var analysisResult = new MarketAnalysisResult
        {
            RequestId = Guid.NewGuid().ToString(),
            AnalyzedAt = DateTime.UtcNow,
            Metadata = new AnalysisMetadata
            {
                UsedTimeframes = request.Timeframes,
                AppliedFilters = GetAppliedFilters(request.Filters)
            }
        };

        try
        {
            // Get symbols to analyze
            var symbolsToAnalyze = await GetSymbolsToAnalyzeAsync(request.Symbols, cancellationToken);
            analysisResult.TotalSymbolsAnalyzed = symbolsToAnalyze.Count;

            // Generate signals for each symbol
            var allSignals = await signalGeneratorService.GenerateSignalsAsync(
                symbolsToAnalyze,
                request.Conditions,
                request.Filters,
                request.Preferences,
                cancellationToken);

            // Apply filters and preferences
            var filteredSignals = await ApplyFiltersAndPreferencesAsync(allSignals, request.Filters, request.Preferences, cancellationToken);

            analysisResult.Signals = filteredSignals;
            analysisResult.SignalsGenerated = filteredSignals.Count;

            // Update metadata
            analysisResult.Metadata.ConditionMatches = CountConditionMatches(filteredSignals, request.Conditions);

            return analysisResult;
        }
        catch (Exception ex)
        {
            analysisResult.Metadata.Errors.Add($"Analysis failed: {ex.Message}");
            return analysisResult;
        }
    }

    public async Task<List<TradingSignal>> GetActiveSignalsAsync(string? symbol = null, string? timeframe = null, CancellationToken cancellationToken = default)
    {
        // This would typically query from a signals repository/database
        // For now, return empty list as placeholder
        return new List<TradingSignal>();
    }

    public async Task<bool> ValidateAnalysisRequestAsync(MarketAnalysisRequestDto request, CancellationToken cancellationToken = default)
    {
        // Validate timeframes
        var validTimeframes = new[] { "1m", "5m", "1h", "4h", "1d" };
        if (!request.Timeframes.All(tf => validTimeframes.Contains(tf)))
            return false;

        // Validate conditions
        if (!request.Conditions.Any())
            return false;

        // Validate symbols if provided
        if (request.Symbols.Any())
        {
            var existingSymbols = cryptocurrencyRepository.GetAll();
            var existingSymbolNames = existingSymbols.Select(c => c.Symbol).ToList();
            
            if (!request.Symbols.All(s => existingSymbolNames.Contains(s)))
                return false;
        }

        return true;
    }

    private async Task<List<string>> GetSymbolsToAnalyzeAsync(List<string> requestedSymbols, CancellationToken cancellationToken)
    {
        if (requestedSymbols.Any())
        {
            return requestedSymbols;
        }

        // If no symbols specified, get all available cryptocurrencies
        return await Task.Run(() =>
        {
            var allCryptos = cryptocurrencyRepository.GetAll();
            return allCryptos.Select(c => c.Symbol).ToList();
        });
    }

    private async Task<List<TradingSignal>> ApplyFiltersAndPreferencesAsync(
        List<TradingSignal> signals,
        AnalysisFiltersDto filters,
        AnalysisPreferencesDto preferences,
        CancellationToken cancellationToken)
    {
        var filteredSignals = signals.AsQueryable();

        // Apply signal strength filter
        if (!string.IsNullOrEmpty(preferences.SignalStrength))
        {
            filteredSignals = filteredSignals.Where(s => s.Strength == preferences.SignalStrength);
        }

        // Apply risk level filter
        if (!string.IsNullOrEmpty(preferences.RiskLevel))
        {
            filteredSignals = filteredSignals.Where(s => s.RiskLevel == preferences.RiskLevel);
        }

        // Apply price filters if specified
        if (filters.PriceMin.HasValue)
        {
            filteredSignals = filteredSignals.Where(s => s.Price >= filters.PriceMin.Value);
        }

        if (filters.PriceMax.HasValue)
        {
            filteredSignals = filteredSignals.Where(s => s.Price <= filters.PriceMax.Value);
        }

        return filteredSignals.ToList();
    }

    private List<string> GetAppliedFilters(AnalysisFiltersDto filters)
    {
        var appliedFilters = new List<string>();

        if (!string.IsNullOrEmpty(filters.VolumeMin))
            appliedFilters.Add($"VolumeMin: {filters.VolumeMin}");

        if (!string.IsNullOrEmpty(filters.Volatility))
            appliedFilters.Add($"Volatility: {filters.Volatility}");

        if (!string.IsNullOrEmpty(filters.Liquidity))
            appliedFilters.Add($"Liquidity: {filters.Liquidity}");

        if (filters.PriceMin.HasValue)
            appliedFilters.Add($"PriceMin: {filters.PriceMin}");

        if (filters.PriceMax.HasValue)
            appliedFilters.Add($"PriceMax: {filters.PriceMax}");

        return appliedFilters;
    }

    private Dictionary<string, int> CountConditionMatches(List<TradingSignal> signals, List<AnalysisConditionDto> conditions)
    {
        var conditionMatches = new Dictionary<string, int>();

        foreach (var condition in conditions)
        {
            var matchCount = signals.Count(s => s.Reasons.Any(r => r.Indicator == condition.Indicator));
            conditionMatches[condition.Indicator] = matchCount;
        }

        return conditionMatches;
    }
}