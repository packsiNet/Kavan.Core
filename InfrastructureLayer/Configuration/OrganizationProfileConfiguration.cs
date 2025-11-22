using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureLayer.Configuration;

public class OrganizationProfileConfiguration : BaseEntityConfiguration<OrganizationProfile>
{
    public override void Configure(EntityTypeBuilder<OrganizationProfile> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.OrganizationName).HasMaxLength(200);
        builder.Property(x => x.LegalName).HasMaxLength(200);
        builder.Property(x => x.Description).HasMaxLength(4000);
        builder.Property(x => x.RegistrationNumber).HasMaxLength(100);
        builder.Property(x => x.Country).HasMaxLength(100);
        builder.Property(x => x.City).HasMaxLength(100);
        builder.Property(x => x.Address).HasMaxLength(500);
        builder.Property(x => x.ContactEmailPublic).HasMaxLength(200);
        builder.Property(x => x.ContactPhonePublic).HasMaxLength(50);
        builder.Property(x => x.LogoUrl).HasMaxLength(500);
        builder.Property(x => x.BannerUrl).HasMaxLength(500);

        builder.HasIndex(x => x.UserAccountId).IsUnique();
        builder.HasIndex(x => x.IsPublic);
        builder.HasIndex(x => x.Country);
    }
}

public class OrganizationActivityConfiguration : BaseEntityConfiguration<OrganizationActivity>
{
    public override void Configure(EntityTypeBuilder<OrganizationActivity> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Title).HasMaxLength(200);
        builder.Property(x => x.Description).HasMaxLength(2000);
        builder.HasIndex(x => x.OrganizationProfileId);
    }
}

public class OrganizationWebsiteConfiguration : BaseEntityConfiguration<OrganizationWebsite>
{
    public override void Configure(EntityTypeBuilder<OrganizationWebsite> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Url).HasMaxLength(500);
        builder.Property(x => x.Type).HasMaxLength(50);
        builder.HasIndex(x => x.OrganizationProfileId);
    }
}

public class OrganizationSocialLinkConfiguration : BaseEntityConfiguration<OrganizationSocialLink>
{
    public override void Configure(EntityTypeBuilder<OrganizationSocialLink> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Platform).HasMaxLength(50);
        builder.Property(x => x.Url).HasMaxLength(500);
        builder.HasIndex(x => x.OrganizationProfileId);
    }
}

public class OrganizationLicenseConfiguration : BaseEntityConfiguration<OrganizationLicense>
{
    public override void Configure(EntityTypeBuilder<OrganizationLicense> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.RegulatorName).HasMaxLength(200);
        builder.Property(x => x.LicenseNumber).HasMaxLength(100);
        builder.Property(x => x.Country).HasMaxLength(100);
        builder.HasIndex(x => x.OrganizationProfileId);
    }
}

public class OrganizationExchangeConfiguration : BaseEntityConfiguration<OrganizationExchange>
{
    public override void Configure(EntityTypeBuilder<OrganizationExchange> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name).HasMaxLength(200);
        builder.Property(x => x.Country).HasMaxLength(100);
        builder.Property(x => x.Url).HasMaxLength(500);
        builder.HasIndex(x => x.OrganizationProfileId);
    }
}

public class OrganizationInvestmentPanelConfiguration : BaseEntityConfiguration<OrganizationInvestmentPanel>
{
    public override void Configure(EntityTypeBuilder<OrganizationInvestmentPanel> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name).HasMaxLength(200);
        builder.Property(x => x.Url).HasMaxLength(500);
        builder.Property(x => x.ProfitShareModel).HasMaxLength(200);
        builder.HasIndex(x => x.OrganizationProfileId);
    }
}