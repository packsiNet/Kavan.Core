using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureLayer.Configuration;

public class CryptocurrencyConfiguration : BaseEntityConfiguration<Cryptocurrency>
{
    public override void Configure(EntityTypeBuilder<Cryptocurrency> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Category)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Symbol)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(x => x.Symbol).IsUnique();
    }
}