using ApplicationLayer.DTOs.Identity;
using FluentValidation;

namespace ApplicationLayer.Features.Identity.Validations;

public class SignUpValidator : AbstractValidator<SignUpDto>
{
    public SignUpValidator()
    {
        RuleFor(x => x.DisplayName).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.PhonePrefix).NotEmpty();
        RuleFor(x => x.PhoneNumber).NotEmpty();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
    }
}

public class SignInValidator : AbstractValidator<SignInDto>
{
    public SignInValidator()
    {
        RuleFor(x => x.ValidationMethod).NotEmpty();
        RuleFor(x => x.UserName).NotEmpty();

        When(x => x.ValidationMethod == 1, () =>
        {
            RuleFor(x => x.Password).NotEmpty();
        });

        When(x => x.ValidationMethod == 2 || x.ValidationMethod == 3, () =>
        {
            RuleFor(x => x.SecurityCode).NotEmpty();
        });
    }
}