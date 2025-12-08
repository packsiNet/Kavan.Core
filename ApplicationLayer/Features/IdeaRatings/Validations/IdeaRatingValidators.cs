using ApplicationLayer.Features.IdeaRatings.Commands;
using FluentValidation;

namespace ApplicationLayer.Features.IdeaRatings.Validations;

public class AddIdeaRatingValidator : AbstractValidator<AddIdeaRatingCommand>
{
    public AddIdeaRatingValidator()
    {
        RuleFor(x => x.Model.IdeaId).GreaterThan(0);
        RuleFor(x => x.Model.Rating).InclusiveBetween(1, 5);
    }
}
