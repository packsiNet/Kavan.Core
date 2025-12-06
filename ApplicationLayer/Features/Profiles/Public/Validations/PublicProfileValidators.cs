using FluentValidation;
using ApplicationLayer.Features.Profiles.Public.Query;

namespace ApplicationLayer.Features.Profiles.Public.Validations;

public class GetUserPublicProfileValidator : AbstractValidator<GetUserPublicProfileQuery>
{
    public GetUserPublicProfileValidator()
    {
        RuleFor(x => x.UserId).GreaterThan(0);
    }
}
