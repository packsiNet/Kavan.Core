using System.Collections.Generic;
using System.Linq;
using ApplicationLayer.Common;
using ApplicationLayer.Dto.Signals;
using FluentValidation;

namespace ApplicationLayer.Features.Signals.Validations
{
    public class SignalValidators
    {
        public class SignalRequestValidator : AbstractValidator<SignalRequestDto>
        {
            private static readonly HashSet<string> AllowedTimeframes = new(new[] { "1m", "5m", "1h", "4h", "1d" });
            private static readonly HashSet<string> AllowedTypes = new(new[]
            {
                "structure", "fvg_entry", "fvg_retest", "support_level", "resistance_level", "mss_break"
            });

            public SignalRequestValidator()
            {
                RuleFor(x => x.Market)
                    .NotEmpty().WithErrorCode(ValidationErrorCodes.NotNull)
                    .Must(m => m.Equals("crypto", System.StringComparison.OrdinalIgnoreCase))
                    .WithMessage(CommonMessages.IncorrectFormat);

                RuleFor(x => x.Timeframes)
                    .NotNull().WithErrorCode(ValidationErrorCodes.NotNull)
                    .Must(tf => tf.Count > 0).WithErrorCode(ValidationErrorCodes.AtLeastOneRequired)
                    .WithMessage("وارد کردن بازه زمانی الزامی است");

                RuleForEach(x => x.Timeframes)
                    .Must(tf => AllowedTimeframes.Contains(tf))
                    .WithMessage("بازه زمانی نامعتبر است");

                // At least one condition or group is required
                RuleFor(x => x)
                    .Must(x => (x.Conditions?.Any() ?? false) || (x.Groups?.Any() ?? false))
                    .WithErrorCode(ValidationErrorCodes.AtLeastOneRequired)
                    .WithMessage("حداقل یک شرط یا گروه الزامی است");

                // Validate condition nodes
                RuleForEach(x => x.Conditions).ChildRules(cond =>
                {
                    cond.RuleFor(c => c.Type)
                        .NotEmpty().WithErrorCode(ValidationErrorCodes.NotNull)
                        .Must(t => AllowedTypes.Contains(t!)).WithMessage("نوع شرط نامعتبر است");

                    cond.RuleFor(c => c.Timeframe)
                        .Must(tf => string.IsNullOrWhiteSpace(tf) || AllowedTimeframes.Contains(tf))
                        .WithMessage("بازه زمانی شرط نامعتبر است");
                });

                // Filters
                RuleFor(x => x.Filters.Volume_Min)
                    .Must(v => v is null || v >= 0).WithErrorCode(ValidationErrorCodes.MustBeGreaterThanZero)
                    .WithMessage("حداقل حجم باید غیرمنفی باشد")
                    .When(x => x.Filters != null);

                RuleFor(x => x.Filters.Price_Min)
                    .Must(v => v is null || v >= 0).WithErrorCode(ValidationErrorCodes.MustBeGreaterThanZero)
                    .WithMessage("حداقل قیمت باید غیرمنفی باشد")
                    .When(x => x.Filters != null);

                RuleFor(x => x.Filters.Price_Max)
                    .Must(v => v is null || v >= 0).WithErrorCode(ValidationErrorCodes.MustBeGreaterThanZero)
                    .WithMessage("حداکثر قیمت باید غیرمنفی باشد")
                    .When(x => x.Filters != null);

                // Optional: Ensure min <= max when both provided
                RuleFor(x => x.Filters)
                    .Must(f => f == null || f.Price_Min == null || f.Price_Max == null || f.Price_Min <= f.Price_Max)
                    .WithMessage("حداقل قیمت باید کمتر یا مساوی حداکثر قیمت باشد")
                    .When(x => x.Filters != null);
            }
        }
    }
}