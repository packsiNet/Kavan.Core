using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureLayer.Configuration;

public class NewsInstrumentConfiguration : IEntityTypeConfiguration<NewsInstrument>
{
    public void Configure(EntityTypeBuilder<NewsInstrument> builder)
    {
        builder.Property(x => x.Code).HasMaxLength(64);
        builder.Property(x => x.Title).HasMaxLength(256);
        builder.Property(x => x.Slug).HasMaxLength(256);
        builder.Property(x => x.Url).HasMaxLength(1024);
    }
}
