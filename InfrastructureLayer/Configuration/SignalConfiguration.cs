using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureLayer.Configuration;

public class SignalConfiguration : BaseEntityConfiguration<Signal>
{
    public override void Configure(EntityTypeBuilder<Signal> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Symbol)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.TimeFrame)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(x => x.SignalType)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(x => x.Rsi).HasPrecision(18, 8);
        builder.Property(x => x.Ema).HasPrecision(18, 8);
        builder.Property(x => x.Macd).HasPrecision(18, 8);

        builder.Property(x => x.Timestamp).IsRequired();

        builder.HasOne(x => x.Cryptocurrency)
            .WithMany()
            .HasForeignKey(x => x.CryptocurrencyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.CryptocurrencyId, x.TimeFrame, x.Timestamp })
               .IsUnique();
    }
}