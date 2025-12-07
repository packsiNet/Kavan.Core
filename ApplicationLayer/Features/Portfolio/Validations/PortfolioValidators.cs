using ApplicationLayer.DTOs.Portfolio;
using FluentValidation;

namespace ApplicationLayer.Features.Portfolio.Validations;

public class CreatePortfolioEntryValidator : AbstractValidator<CreatePortfolioEntryDto>
{
    public CreatePortfolioEntryValidator()
    {
        RuleFor(x => x.Symbol).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.BuyPrice).GreaterThan(0);
    }
}

public class UpdatePortfolioEntryValidator : AbstractValidator<UpdatePortfolioEntryDto>
{
    public UpdatePortfolioEntryValidator()
    {
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.BuyPrice).GreaterThan(0);
    }
}

public class GetPortfolioRequestValidator : AbstractValidator<GetPortfolioRequestDto>
{
    public GetPortfolioRequestValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 200);
    }
}

public class GetPortfolioEntriesRequestValidator : AbstractValidator<GetPortfolioEntriesRequestDto>
{
    public GetPortfolioEntriesRequestValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 200);
    }
}

public class CreatePortfolioSaleValidator : AbstractValidator<CreatePortfolioSaleDto>
{
    public CreatePortfolioSaleValidator()
    {
        RuleFor(x => x.Symbol).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.SellPrice).GreaterThan(0);
    }
}
