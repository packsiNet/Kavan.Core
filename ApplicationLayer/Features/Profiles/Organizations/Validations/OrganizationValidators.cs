using ApplicationLayer.Common.Enums;
using ApplicationLayer.DTOs.Profiles.Organizations;
using FluentValidation;

namespace ApplicationLayer.Features.Profiles.Organizations.Validations;

public class UpdateOrganizationProfileValidator : AbstractValidator<UpdateOrganizationProfileDto>
{
    public UpdateOrganizationProfileValidator()
    {
        RuleFor(x => x.OrganizationName).NotEmpty();
        RuleFor(x => x.Country).NotEmpty();
        RuleFor(x => x.Description).MaximumLength(4000);

        RuleForEach(x => x.Websites).ChildRules(w =>
        {
            w.RuleFor(y => y.Url).NotEmpty();
            w.RuleFor(y => y.Type).Must(WebsiteType.IsValid);
        });

        RuleForEach(x => x.SocialLinks).ChildRules(s =>
        {
            s.RuleFor(y => y.Url).NotEmpty();
            s.RuleFor(y => y.Platform).Must(SocialPlatform.IsValid);
        });

        When(x => x.Logo != null, () =>
        {
            RuleFor(x => x.Logo.ContentType).Must(ct => new[] { "image/jpeg", "image/png", "image/webp" }.Contains(ct));
            RuleFor(x => x.Logo.Length).LessThanOrEqualTo(5 * 1024 * 1024);
        });

        When(x => x.Banner != null, () =>
        {
            RuleFor(x => x.Banner.ContentType).Must(ct => new[] { "image/jpeg", "image/png", "image/webp" }.Contains(ct));
            RuleFor(x => x.Banner.Length).LessThanOrEqualTo(5 * 1024 * 1024);
        });
    }
}

public class SearchOrganizationsValidator : AbstractValidator<SearchOrganizationsDto>
{
    public SearchOrganizationsValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}