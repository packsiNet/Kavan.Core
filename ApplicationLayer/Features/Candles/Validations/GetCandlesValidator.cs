using ApplicationLayer.Features.Candles.Query;
using FluentValidation;

namespace ApplicationLayer.Features.Candles.Validations;

public class GetCandlesValidator : AbstractValidator<GetCandlesQuery>
{
    public GetCandlesValidator()
    {
        RuleFor(x => x.CryptocurrencyId).GreaterThan(0);
        RuleFor(x => x.Timeframe).NotEmpty().Must(BeValidTimeframe).WithMessage("Invalid timeframe. Use 1m, 5m, 15m, 1h, 4h, 1d, 1w.");
        RuleFor(x => x.Limit).GreaterThan(0).LessThanOrEqualTo(1000);
    }

    private bool BeValidTimeframe(string timeframe)
    {
        var valid = new[] { "1m", "5m", "15m", "1h", "4h", "1d", "1w" };
        return valid.Contains(timeframe);
    }
}
