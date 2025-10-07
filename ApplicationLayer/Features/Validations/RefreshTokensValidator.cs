using ApplicationLayer.Common;
using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.Features.Identity.Commands;
using FluentValidation;

namespace ApplicationLayer.Features.Validations;

public class RefreshTokensValidator
{
    public class TokenRequestValidator : AbstractValidator<TokenRequestDto>
    {
        public TokenRequestValidator()
        {
            RuleFor(row => row.AccessTokens)
                .NotNull()
                .NotEmpty()
                .WithErrorCode("100").WithMessage("توکن نباید خالی باشد");

            RuleFor(row => row.RefreshToken)
                .NotNull()
                .NotEmpty()
                .WithErrorCode("100").WithMessage("رفرش توکن نباید خالی باشد");
        }
    }

    public class RevokeRefreshTokenValidator : AbstractValidator<RevokeRefreshTokenCommand>
    {
        public RevokeRefreshTokenValidator()
        {
            RuleFor(row => row.UserId)
                .NotNull()
                .NotEmpty()
                .WithErrorCode(ValidationErrorCodes.NotNull)
                .WithMessage(CommonValidateMessages.Required("آیدی کاربر"))
                .Must(id => id > 0)
                .WithErrorCode(ValidationErrorCodes.MustBeGreaterThanZero)
                .WithMessage(CommonValidateMessages.MustBeGreaterThanZero("آیدی"));
        }
    }
}