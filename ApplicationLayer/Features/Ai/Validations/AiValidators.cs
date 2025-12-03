using ApplicationLayer.Dto.AI;
using FluentValidation;

namespace ApplicationLayer.Features.Validations;

public class AiTranslateRequestValidator : AbstractValidator<AiTranslateRequestDto>
{
    public AiTranslateRequestValidator()
    {
        RuleFor(x => x.Text).NotEmpty().MinimumLength(1);
    }
}

