using ApplicationLayer.Features.MarketAnalysis.Commands;
using FluentValidation;

namespace ApplicationLayer.Features.MarketAnalysis.Validations;

public class AnalyzeMarketValidator : AbstractValidator<AnalyzeMarketCommand>
{
    public AnalyzeMarketValidator()
    {
        RuleFor(x => x.Request)
            .NotNull()
            .WithMessage("Analysis request cannot be null");

        RuleFor(x => x.Request.Market)
            .NotEmpty()
            .WithMessage("Market is required")
            .Must(market => market.ToLower() == "crypto")
            .WithMessage("Currently only 'crypto' market is supported");

        RuleFor(x => x.Request.Timeframes)
            .NotEmpty()
            .WithMessage("At least one timeframe is required")
            .Must(timeframes => timeframes.All(tf => IsValidTimeframe(tf)))
            .WithMessage("Invalid timeframe. Supported: 1m, 5m, 1h, 4h, 1d");

        RuleFor(x => x.Request.Conditions)
            .NotEmpty()
            .WithMessage("At least one condition is required");

        RuleForEach(x => x.Request.Conditions)
            .SetValidator(new AnalysisConditionValidator());

        RuleFor(x => x.Request.Preferences.RiskLevel)
            .Must(risk => IsValidRiskLevel(risk))
            .WithMessage("Risk level must be: low, medium, or high");

        RuleFor(x => x.Request.Preferences.StrategyType)
            .Must(strategy => IsValidStrategyType(strategy))
            .WithMessage("Strategy type must be: price_action, technical, or fundamental");

        RuleFor(x => x.Request.Preferences.SignalStrength)
            .Must(strength => IsValidSignalStrength(strength))
            .WithMessage("Signal strength must be: weak, medium, or strong");
    }

    private static bool IsValidTimeframe(string timeframe)
    {
        var validTimeframes = new[] { "1m", "5m", "1h", "4h", "1d" };
        return validTimeframes.Contains(timeframe.ToLower());
    }

    private static bool IsValidRiskLevel(string riskLevel)
    {
        var validLevels = new[] { "low", "medium", "high" };
        return validLevels.Contains(riskLevel.ToLower());
    }

    private static bool IsValidStrategyType(string strategyType)
    {
        var validTypes = new[] { "price_action", "technical", "fundamental" };
        return validTypes.Contains(strategyType.ToLower());
    }

    private static bool IsValidSignalStrength(string signalStrength)
    {
        var validStrengths = new[] { "weak", "medium", "strong" };
        return validStrengths.Contains(signalStrength.ToLower());
    }
}

public class AnalysisConditionValidator : AbstractValidator<ApplicationLayer.Dto.MarketAnalysis.AnalysisConditionDto>
{
    public AnalysisConditionValidator()
    {
        RuleFor(x => x.Type)
            .NotEmpty()
            .WithMessage("Condition type is required")
            .Must(type => IsValidConditionType(type))
            .WithMessage("Invalid condition type. Supported: breakout, pattern");

        RuleFor(x => x.Indicator)
            .NotEmpty()
            .WithMessage("Indicator is required");

        RuleFor(x => x.Timeframe)
            .NotEmpty()
            .WithMessage("Timeframe is required for condition");

        RuleFor(x => x.LogicalOperator)
            .Must(op => IsValidLogicalOperator(op))
            .WithMessage("Logical operator must be: AND, OR, or NOT");
    }

    private static bool IsValidConditionType(string type)
    {
        var validTypes = new[] { "breakout", "pattern", "volume", "price_action" };
        return validTypes.Contains(type.ToLower());
    }

    private static bool IsValidLogicalOperator(string op)
    {
        var validOperators = new[] { "AND", "OR", "NOT" };
        return validOperators.Contains(op.ToUpper());
    }
}