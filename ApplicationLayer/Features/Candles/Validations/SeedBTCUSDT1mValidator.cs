using ApplicationLayer.Dto.Candles;
using FluentValidation;

namespace ApplicationLayer.Features.Candles.Validations;

public class SeedCandlesRequestValidator : AbstractValidator<SeedCandlesRequestDto>
{
    public SeedCandlesRequestValidator()
    {
        RuleFor(x => x.Count)
            .GreaterThan(0)
            .LessThanOrEqualTo(5000)
            .WithMessage("تعداد کندل‌ها باید بین 1 تا 5000 باشد");
    }
}