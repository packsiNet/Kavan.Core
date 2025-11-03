using FluentValidation;
using ApplicationLayer.Features.Candles.Commands;

namespace ApplicationLayer.Features.Candles.Validations
{
    public class SeedEthMtfFvgValidator : AbstractValidator<SeedEthMtfFvgCommand>
    {
        public SeedEthMtfFvgValidator()
        {
            RuleFor(x => x.Days).GreaterThanOrEqualTo(3);
            RuleFor(x => x.M5Bars).GreaterThanOrEqualTo(10);
            RuleFor(x => x.M1Bars).GreaterThanOrEqualTo(10);
        }
    }
}