using ApplicationLayer.Dto.Breakouts;
using FluentValidation;

namespace ApplicationLayer.Features.Breakouts.Validations
{
    public class BreakoutScanRequestValidator : AbstractValidator<BreakoutScanRequestDto>
    {
        private static readonly HashSet<string> AllowedTimeframes = new(new[] { "1m", "5m", "1h", "4h", "1d" });

        public BreakoutScanRequestValidator()
        {
            RuleFor(x => x.LookbackPeriod)
                .GreaterThanOrEqualTo(30)
                .LessThanOrEqualTo(5000)
                .WithMessage("تعداد کندل‌های بررسی باید بین 30 تا 5000 باشد")
                .When(x => !x.AutoWindow);

            RuleFor(x => x.Timeframes)
                .Must(tfs => tfs == null || tfs.Count == 0 || tfs.All(tf => AllowedTimeframes.Contains(tf)))
                .WithMessage("بازه زمانی نامعتبر است");
        }
    }
}