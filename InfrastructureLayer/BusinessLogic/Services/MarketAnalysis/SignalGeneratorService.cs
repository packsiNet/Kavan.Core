using ApplicationLayer.Dto.MarketAnalysis;
using ApplicationLayer.Interfaces;
using ApplicationLayer.Interfaces.Services;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;

namespace InfrastructureLayer.BusinessLogic.Services.MarketAnalysis;

[InjectAsScoped]
public class SignalGeneratorService(
    IUnitOfWork unitOfWork,
    ITechnicalIndicatorService technicalIndicatorService,
    IRepository<Cryptocurrency> cryptocurrencyRepository
) : ISignalGeneratorService
{
    public async Task<List<TradingSignal>> GenerateSignalsAsync(
        List<string> symbols,
        List<AnalysisConditionDto> conditions,
        AnalysisFiltersDto filters,
        AnalysisPreferencesDto preferences,
        CancellationToken cancellationToken = default)
    {
        var allSignals = new List<TradingSignal>();

        foreach (var symbol in symbols)
        {
            foreach (var timeframe in GetUniqueTimeframes(conditions))
            {
                var signal = await EvaluateConditionsAsync(symbol, timeframe, conditions, preferences, cancellationToken);
                if (signal != null)
                {
                    allSignals.Add(signal);
                }
            }
        }

        return allSignals;
    }

    public async Task<TradingSignal?> EvaluateConditionsAsync(
        string symbol,
        string timeframe,
        List<AnalysisConditionDto> conditions,
        AnalysisPreferencesDto preferences,
        CancellationToken cancellationToken = default)
    {
        var matchedConditions = new List<SignalReason>();
        var confirmedIndicators = new List<ConfirmedIndicator>();

        foreach (var condition in conditions.Where(c => c.Timeframe == timeframe))
        {
            var conditionResult = await EvaluateConditionAsync(symbol, timeframe, condition, cancellationToken);
            
            if (conditionResult.IsMatched)
            {
                matchedConditions.Add(new SignalReason
                {
                    ConditionType = condition.Type,
                    Indicator = condition.Indicator,
                    Description = condition.Description,
                    IsPrimary = true,
                    Weight = CalculateConditionWeight(condition)
                });

                confirmedIndicators.AddRange(conditionResult.ConfirmedIndicators);
            }
        }

        // Check if we have enough matched conditions based on logical operators
        if (!HasSufficientMatches(conditions, matchedConditions))
        {
            return null;
        }

        // Generate signal
        var signal = new TradingSignal
        {
            Symbol = symbol,
            Timeframe = timeframe,
            SignalType = DetermineSignalType(matchedConditions),
            Price = await GetCurrentPriceAsync(symbol, cancellationToken),
            Reasons = matchedConditions,
            ConfirmedIndicators = confirmedIndicators,
            GeneratedAt = DateTime.UtcNow,
            IsActive = true
        };

        // Calculate confidence and other properties
        signal.Confidence = await CalculateSignalConfidenceAsync(signal, conditions, cancellationToken);
        signal.Strength = DetermineSignalStrength(signal.Confidence);
        signal.RiskLevel = DetermineRiskLevel(signal, preferences);
        signal.Targets = await CalculateTargetsAsync(signal, cancellationToken);

        return signal;
    }

    public async Task<bool> ValidateSignalConfirmationAsync(
        TradingSignal signal,
        List<ConditionConfirmationDto> confirmations,
        CancellationToken cancellationToken = default)
    {
        foreach (var confirmation in confirmations.Where(c => c.Required))
        {
            var isConfirmed = await ValidateConfirmationAsync(signal, confirmation, cancellationToken);
            if (!isConfirmed)
            {
                return false;
            }
        }

        return true;
    }

    public async Task<decimal> CalculateSignalConfidenceAsync(
        TradingSignal signal,
        List<AnalysisConditionDto> conditions,
        CancellationToken cancellationToken = default)
    {
        decimal totalWeight = 0;
        decimal matchedWeight = 0;

        foreach (var condition in conditions)
        {
            var weight = CalculateConditionWeight(condition);
            totalWeight += weight;

            if (signal.Reasons.Any(r => r.Indicator == condition.Indicator))
            {
                matchedWeight += weight;
            }
        }

        var baseConfidence = totalWeight > 0 ? (matchedWeight / totalWeight) * 100 : 0;

        // Apply additional confidence factors
        var confirmationBonus = signal.ConfirmedIndicators.Count(ci => ci.Status == "confirmed") * 5;
        var volumeBonus = await HasVolumeConfirmationAsync(signal, cancellationToken) ? 10 : 0;

        return Math.Min(100, baseConfidence + confirmationBonus + volumeBonus);
    }

    private async Task<ConditionEvaluationResult> EvaluateConditionAsync(
        string symbol,
        string timeframe,
        AnalysisConditionDto condition,
        CancellationToken cancellationToken)
    {
        var result = new ConditionEvaluationResult();

        switch (condition.Type.ToLower())
        {
            case "breakout":
                result = await EvaluateBreakoutConditionAsync(symbol, timeframe, condition, cancellationToken);
                break;
            case "pattern":
                result = await EvaluatePatternConditionAsync(symbol, timeframe, condition, cancellationToken);
                break;
            default:
                result.IsMatched = false;
                break;
        }

        return result;
    }

    private async Task<ConditionEvaluationResult> EvaluateBreakoutConditionAsync(
        string symbol,
        string timeframe,
        AnalysisConditionDto condition,
        CancellationToken cancellationToken)
    {
        var result = new ConditionEvaluationResult();

        switch (condition.Indicator.ToLower())
        {
            case "structure_break":
            case "mss_break":
                var mssShifts = await technicalIndicatorService.DetectMarketStructureShiftsAsync(symbol, timeframe, cancellationToken);
                var recentMss = mssShifts.Where(m => m.DetectedAt >= DateTime.UtcNow.AddHours(-24)).ToList();

                if (recentMss.Any())
                {
                    result.IsMatched = true;
                    result.ConfirmedIndicators.Add(new ConfirmedIndicator
                    {
                        Name = "Market Structure Shift",
                        Type = "MSS",
                        Status = "confirmed",
                        IsRequired = true,
                        Values = new Dictionary<string, object>
                        {
                            ["direction"] = recentMss.First().Direction,
                            ["strength"] = recentMss.First().BreakStrength
                        }
                    });
                }
                break;
        }

        return result;
    }

    private async Task<ConditionEvaluationResult> EvaluatePatternConditionAsync(
        string symbol,
        string timeframe,
        AnalysisConditionDto condition,
        CancellationToken cancellationToken)
    {
        var result = new ConditionEvaluationResult();

        switch (condition.Indicator.ToLower())
        {
            case "fvg_entry":
            case "fvg_retest":
                var currentPrice = await GetCurrentPriceAsync(symbol, cancellationToken);
                var isInFvg = await technicalIndicatorService.IsPriceInFVGZoneAsync(symbol, currentPrice, cancellationToken);

                if (isInFvg)
                {
                    result.IsMatched = true;
                    result.ConfirmedIndicators.Add(new ConfirmedIndicator
                    {
                        Name = "Fair Value Gap",
                        Type = "FVG",
                        Status = "confirmed",
                        IsRequired = true,
                        Values = new Dictionary<string, object>
                        {
                            ["current_price"] = currentPrice,
                            ["in_fvg_zone"] = true
                        }
                    });
                }
                break;

            case "support_level":
            case "resistance_level":
                var levels = await technicalIndicatorService.DetectSupportResistanceLevelsAsync(symbol, timeframe, cancellationToken);
                var relevantLevels = levels.Where(l => l.LevelType == condition.Indicator.Replace("_level", "")).ToList();

                if (relevantLevels.Any())
                {
                    result.IsMatched = true;
                    result.ConfirmedIndicators.Add(new ConfirmedIndicator
                    {
                        Name = condition.Indicator.Replace("_", " ").ToTitleCase(),
                        Type = condition.Indicator.ToUpper(),
                        Status = "confirmed",
                        IsRequired = true,
                        Values = new Dictionary<string, object>
                        {
                            ["level_count"] = relevantLevels.Count,
                            ["strongest_level"] = relevantLevels.OrderByDescending(l => l.TouchCount).First().Price
                        }
                    });
                }
                break;
        }

        return result;
    }

    private List<string> GetUniqueTimeframes(List<AnalysisConditionDto> conditions)
    {
        return conditions.Select(c => c.Timeframe).Distinct().ToList();
    }

    private decimal CalculateConditionWeight(AnalysisConditionDto condition)
    {
        return condition.Type.ToLower() switch
        {
            "breakout" => 30,
            "pattern" => 25,
            "volume" => 20,
            "price_action" => 25,
            _ => 10
        };
    }

    private bool HasSufficientMatches(List<AnalysisConditionDto> conditions, List<SignalReason> matches)
    {
        // Simple logic: require at least 50% of conditions to match
        var requiredMatches = Math.Ceiling(conditions.Count * 0.5);
        return matches.Count >= requiredMatches;
    }

    private string DetermineSignalType(List<SignalReason> reasons)
    {
        var bullishIndicators = new[] { "structure_break", "mss_break", "support_level", "fvg_entry" };
        var bearishIndicators = new[] { "resistance_level" };

        var bullishCount = reasons.Count(r => bullishIndicators.Contains(r.Indicator.ToLower()));
        var bearishCount = reasons.Count(r => bearishIndicators.Contains(r.Indicator.ToLower()));

        if (bullishCount > bearishCount)
            return "BUY";
        else if (bearishCount > bullishCount)
            return "SELL";
        else
            return "HOLD";
    }

    private async Task<decimal> GetCurrentPriceAsync(string symbol, CancellationToken cancellationToken)
    {
        // This would typically get the latest price from the most recent candle
        // For now, return a placeholder value
        return 50000; // Placeholder
    }

    private string DetermineSignalStrength(decimal confidence)
    {
        return confidence switch
        {
            >= 80 => "strong",
            >= 60 => "medium",
            _ => "weak"
        };
    }

    private string DetermineRiskLevel(TradingSignal signal, AnalysisPreferencesDto preferences)
    {
        // Base risk on signal strength and user preferences
        return signal.Strength switch
        {
            "strong" => "low",
            "medium" => "medium",
            _ => "high"
        };
    }

    private async Task<SignalTargets> CalculateTargetsAsync(TradingSignal signal, CancellationToken cancellationToken)
    {
        // Calculate basic targets based on signal type and price
        var entryPrice = signal.Price;
        var riskPercent = 0.02m; // 2% risk

        return new SignalTargets
        {
            EntryPrice = entryPrice,
            StopLoss = signal.SignalType == "BUY" ? entryPrice * (1 - riskPercent) : entryPrice * (1 + riskPercent),
            TakeProfits = signal.SignalType == "BUY" 
                ? new List<decimal> { entryPrice * 1.02m, entryPrice * 1.04m, entryPrice * 1.06m }
                : new List<decimal> { entryPrice * 0.98m, entryPrice * 0.96m, entryPrice * 0.94m },
            RiskRewardRatio = 3.0m
        };
    }

    private async Task<bool> ValidateConfirmationAsync(TradingSignal signal, ConditionConfirmationDto confirmation, CancellationToken cancellationToken)
    {
        // Implement specific confirmation logic based on confirmation type
        return confirmation.Type.ToLower() switch
        {
            "structure_break" => signal.ConfirmedIndicators.Any(ci => ci.Type == "MSS" && ci.Status == "confirmed"),
            "volume" => await HasVolumeConfirmationAsync(signal, cancellationToken),
            "price_action" => signal.ConfirmedIndicators.Any(ci => ci.Type == "FVG" && ci.Status == "confirmed"),
            _ => true
        };
    }

    private async Task<bool> HasVolumeConfirmationAsync(TradingSignal signal, CancellationToken cancellationToken)
    {
        // Placeholder for volume confirmation logic
        return true;
    }

    private class ConditionEvaluationResult
    {
        public bool IsMatched { get; set; }
        public List<ConfirmedIndicator> ConfirmedIndicators { get; set; } = new();
    }
}

public static class StringExtensions
{
    public static string ToTitleCase(this string input)
    {
        return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input.ToLower());
    }
}