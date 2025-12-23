using ApplicationLayer.Dto.FinancialPeriod;
using DomainLayer.Common.Enums;
using FluentValidation;

namespace ApplicationLayer.Features.FinancialPeriod.Validations;

public class CreateFinancialPeriodValidator : AbstractValidator<CreateFinancialPeriodDto>
{
    public CreateFinancialPeriodValidator()
    {
        RuleFor(x => x.StartDateUtc).NotEmpty()
            .LessThan(x => x.EndDateUtc).WithMessage("Start date must be before end date.");
        
        RuleFor(x => x.EndDateUtc).NotEmpty();
        
        RuleFor(x => x.PeriodType).Must(FinancialPeriodType.IsValid)
            .WithMessage("Invalid period type.");
    }
}
