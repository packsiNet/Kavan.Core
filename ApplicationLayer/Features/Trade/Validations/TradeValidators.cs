using ApplicationLayer.Dto.Trade;
using DomainLayer.Common.Enums;
using FluentValidation;

namespace ApplicationLayer.Features.Trade.Validations;

public class CreateTradeValidator : AbstractValidator<CreateTradeDto>
{
    public CreateTradeValidator()
    {
        RuleFor(x => x.Symbol).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Side).Must(TradeSide.IsValid).WithMessage("Invalid trade side.");
        RuleFor(x => x.EntryPrice).GreaterThan(0);
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.Leverage).GreaterThan(0);
        
        RuleFor(x => x.StopLoss).GreaterThan(0);
        RuleFor(x => x).Must(HaveValidStopLoss).WithMessage("Invalid StopLoss for the given Side and EntryPrice.");
        
        RuleFor(x => x.ConfidenceLevel).InclusiveBetween(1, 5);
        RuleFor(x => x.EmotionBeforeEntry).MaximumLength(500);
    }

    private bool HaveValidStopLoss(CreateTradeDto dto)
    {
        if (dto.Side == TradeSide.Long)
            return dto.StopLoss < dto.EntryPrice;
        else if (dto.Side == TradeSide.Short)
            return dto.StopLoss > dto.EntryPrice;
        
        return false;
    }
}
