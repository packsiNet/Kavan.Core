using ApplicationLayer.DTOs.User;
using FluentValidation;

namespace ApplicationLayer.Features.Profiles.Users.Validations;

public class UpdateMyUserProfileValidator : AbstractValidator<UpdateUserProfileDto>
{
    public UpdateMyUserProfileValidator()
    {
        RuleFor(x => x.DisplayName).NotEmpty();
        RuleFor(x => x.AboutMe).MaximumLength(4000);
    }
}
