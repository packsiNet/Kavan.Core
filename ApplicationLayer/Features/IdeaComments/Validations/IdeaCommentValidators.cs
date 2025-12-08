using ApplicationLayer.Features.IdeaComments.Commands;
using FluentValidation;

namespace ApplicationLayer.Features.IdeaComments.Validations;

public class CreateIdeaCommentValidator : AbstractValidator<CreateIdeaCommentCommand>
{
    public CreateIdeaCommentValidator()
    {
        RuleFor(x => x.Model.IdeaId).GreaterThan(0);
        RuleFor(x => x.Model.Comment).NotEmpty().MaximumLength(1000);
    }
}
