using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureLayer.Configuration;

public class CryptocurrencyConfiguration : BaseEntityConfiguration<Cryptocurrency>
{
    public override void Configure(EntityTypeBuilder<Cryptocurrency> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Symbol)
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(x => x.Symbol).IsUnique();
    }
}