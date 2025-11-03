using ApplicationLayer.Common;
using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.DTOs.Plans;
using ApplicationLayer.Features.Plans.Commands;
using FluentValidation;

namespace ApplicationLayer.Features.Plans.Validations;

public class PlanValidators
{
    public class CreatePlanValidator : AbstractValidator<CreatePlanDto>
    {
        public CreatePlanValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithErrorCode(ValidationErrorCodes.NotNull)
                .MaximumLength(100);

            RuleFor(x => x.Code)
                .NotEmpty().WithErrorCode(ValidationErrorCodes.NotNull)
                .MaximumLength(50);

            RuleFor(x => x.Description)
                .MaximumLength(500);

            RuleFor(x => x.PriceMonthly)
                .GreaterThanOrEqualTo(0).WithErrorCode(ValidationErrorCodes.MustBeGreaterThanZero);

            RuleFor(x => x.PriceYearly)
                .GreaterThanOrEqualTo(0).WithErrorCode(ValidationErrorCodes.MustBeGreaterThanZero);

            RuleForEach(x => x.Features).ChildRules(features =>
            {
                features.RuleFor(f => f.Key).NotEmpty().MaximumLength(50);
                features.RuleFor(f => f.Value).MaximumLength(200);
                features.RuleFor(f => f.Unit).MaximumLength(20);
            });
        }
    }

    public class UpdatePlanValidator : AbstractValidator<UpdatePlanDto>
    {
        public UpdatePlanValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithErrorCode(ValidationErrorCodes.NotNull)
                .MaximumLength(100);

            RuleFor(x => x.Description)
                .MaximumLength(500);

            RuleFor(x => x.PriceMonthly)
                .GreaterThanOrEqualTo(0).WithErrorCode(ValidationErrorCodes.MustBeGreaterThanZero);

            RuleFor(x => x.PriceYearly)
                .GreaterThanOrEqualTo(0).WithErrorCode(ValidationErrorCodes.MustBeGreaterThanZero);

            RuleForEach(x => x.Features).ChildRules(features =>
            {
                features.RuleFor(f => f.Key).NotEmpty().MaximumLength(50);
                features.RuleFor(f => f.Value).MaximumLength(200);
                features.RuleFor(f => f.Unit).MaximumLength(20);
            });
        }
    }

    public class DeletePlanValidator : AbstractValidator<DeletePlanCommand>
    {
        public DeletePlanValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithErrorCode(ValidationErrorCodes.NotNull)
                .Must(id => id > 0).WithErrorCode(ValidationErrorCodes.MustBeGreaterThanZero)
                .WithMessage(CommonValidateMessages.MustBeGreaterThanZero("آیدی"));
        }
    }
}