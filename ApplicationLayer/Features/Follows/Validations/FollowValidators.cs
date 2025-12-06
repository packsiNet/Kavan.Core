using ApplicationLayer.Features.Follows.Commands;
using FluentValidation;

namespace ApplicationLayer.Features.Follows.Validations;

public class FollowUserValidator : AbstractValidator<FollowUserCommand>
{
    public FollowUserValidator()
    {
        RuleFor(x => x.TargetUserId)
            .GreaterThan(0)
            .WithMessage("شناسه کاربر باید بزرگ‌تر از صفر باشد");
    }
}

public class UnfollowUserValidator : AbstractValidator<UnfollowUserCommand>
{
    public UnfollowUserValidator()
    {
        RuleFor(x => x.TargetUserId)
            .GreaterThan(0)
            .WithMessage("شناسه کاربر باید بزرگ‌تر از صفر باشد");
    }
}

