using ApplicationLayer.Common.Enums;
using ApplicationLayer.DTOs.Ideas;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace ApplicationLayer.Features.Ideas.Validations;

public class CreateIdeaValidator : AbstractValidator<CreateIdeaDto>
{
    public CreateIdeaValidator()
    {
        RuleFor(x => x.Symbol).NotEmpty();
        RuleFor(x => x.Timeframe).NotEmpty().Must(TimeframeUnit.IsValid);
        RuleFor(x => x.Trend).Must(IdeaTrend.IsValid);
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.Status).Must(IdeaVisibility.IsValid);

        When(x => x.Image != null, () =>
        {
            RuleFor(x => x.Image.ContentType).Must(ct => new[] { "image/jpeg", "image/png", "image/webp" }.Contains(ct));
            RuleFor(x => x.Image.Length).LessThanOrEqualTo(5 * 1024 * 1024);
        });
    }
}

public class UpdateIdeaValidator : AbstractValidator<UpdateIdeaDto>
{
    public UpdateIdeaValidator()
    {
        RuleFor(x => x.Symbol).NotEmpty();
        RuleFor(x => x.Timeframe).NotEmpty().Must(TimeframeUnit.IsValid);
        RuleFor(x => x.Trend).Must(IdeaTrend.IsValid);
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.Status).Must(IdeaVisibility.IsValid);

        When(x => x.Image != null, () =>
        {
            RuleFor(x => x.Image.ContentType).Must(ct => new[] { "image/jpeg", "image/png", "image/webp" }.Contains(ct));
            RuleFor(x => x.Image.Length).LessThanOrEqualTo(5 * 1024 * 1024);
        });
    }
}

public class GetIdeasRequestValidator : AbstractValidator<GetIdeasRequestDto>
{
    public GetIdeasRequestValidator()
    {
        RuleFor(x => x.Timeframe).Must(x => string.IsNullOrWhiteSpace(x) || TimeframeUnit.IsValid(x));
        RuleFor(x => x.Trend).Must(x => string.IsNullOrWhiteSpace(x) || IdeaTrend.IsValid(x));
    }
}