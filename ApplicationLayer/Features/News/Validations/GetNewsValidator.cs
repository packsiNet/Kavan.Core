using ApplicationLayer.Dto.News;
using FluentValidation;

namespace ApplicationLayer.Features.News.Validations;

public class GetNewsValidator : AbstractValidator<GetNewsRequestDto>
{
    public GetNewsValidator()
    {
        RuleFor(x => x.Pagination.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.Pagination.PageSize).InclusiveBetween(1, 200);
    }
}
