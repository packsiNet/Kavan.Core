using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureLayer.Configuration
{
    public class SignalConfiguration : IEntityTypeConfiguration<Signal>
    {
        public void Configure(EntityTypeBuilder<Signal> builder)
        {
            builder.ToTable("Signals");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Symbol).HasMaxLength(50).IsRequired();
            builder.Property(x => x.Timeframe).HasMaxLength(10).IsRequired();
            builder.Property(x => x.SignalCategory).HasMaxLength(50).IsRequired();
            builder.Property(x => x.SignalName).HasMaxLength(50).IsRequired();

            builder.HasOne(x => x.Cryptocurrency)
                   .WithMany()
                   .HasForeignKey(x => x.CryptocurrencyId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Candles)
                   .WithOne(c => c.Signal)
                   .HasForeignKey(c => c.SignalId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new { x.Symbol, x.Timeframe, x.SignalCategory, x.SignalName, x.Direction, x.SignalTime });
            builder.HasIndex(x => new { x.CryptocurrencyId, x.SignalTime });
        }
    }
}