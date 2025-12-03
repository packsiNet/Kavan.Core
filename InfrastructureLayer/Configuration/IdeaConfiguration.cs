using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureLayer.Configuration;

public class IdeaConfiguration : BaseEntityConfiguration<Idea>
{
    public override void Configure(EntityTypeBuilder<Idea> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Symbol)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.Timeframe)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(x => x.Trend)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.TitleTranslate)
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .HasMaxLength(4000);

        builder.Property(x => x.DescriptionTranslate)
            .HasMaxLength(4000);

        builder.Property(x => x.ImageUrl)
            .HasMaxLength(500);

        builder.Property(x => x.Tags)
            .HasMaxLength(1000);

        builder.HasIndex(x => x.IsPublic);
        builder.HasIndex(x => new { x.Symbol, x.Timeframe });
    }
}
