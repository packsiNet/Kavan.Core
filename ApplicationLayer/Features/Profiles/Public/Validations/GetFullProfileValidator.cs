using FluentValidation;
using ApplicationLayer.Features.Profiles.Public.Query;

namespace ApplicationLayer.Features.Profiles.Public.Validations;

public class GetFullProfileValidator : AbstractValidator<GetFullProfileQuery>
{
    public GetFullProfileValidator()
    {
        RuleFor(x => x.UserId).Must(id => !id.HasValue || id.Value > 0);
    }
}
