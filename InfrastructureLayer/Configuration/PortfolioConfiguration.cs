using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureLayer.Configuration;

public class PortfolioEntryConfiguration : IEntityTypeConfiguration<PortfolioEntry>
{
    public void Configure(EntityTypeBuilder<PortfolioEntry> builder)
    {
        builder.Property(x => x.Symbol).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Quantity).HasPrecision(18, 8);
        builder.Property(x => x.BuyPrice).HasPrecision(18, 8);
        builder.HasIndex(x => new { x.Symbol });
        builder.HasIndex(x => new { x.CryptocurrencyId });
        builder.HasIndex(x => new { x.Symbol, x.BuyDate });
    }
}