using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureLayer.Configuration;

public class NewsPostConfiguration : IEntityTypeConfiguration<NewsPost>
{
    public void Configure(EntityTypeBuilder<NewsPost> builder)
    {
        builder.HasIndex(x => x.ExternalId).IsUnique();

        builder.Property(x => x.Slug).HasMaxLength(512);
        builder.Property(x => x.Title).HasMaxLength(1024);
        builder.Property(x => x.Description).HasMaxLength(2048);
        builder.Property(x => x.Kind).HasMaxLength(64);
        builder.Property(x => x.OriginalUrl).HasMaxLength(1024);
        builder.Property(x => x.Url).HasMaxLength(1024);
        builder.Property(x => x.Image).HasMaxLength(1024);
        builder.Property(x => x.SourceTitle).HasMaxLength(256);
        builder.Property(x => x.SourceRegion).HasMaxLength(32);
        builder.Property(x => x.SourceDomain).HasMaxLength(256);
        builder.Property(x => x.SourceType).HasMaxLength(64);
        builder.Property(x => x.Author).HasMaxLength(256);

        builder.HasMany(x => x.Instruments)
               .WithOne(i => i.NewsPost)
               .HasForeignKey(i => i.NewsPostId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
