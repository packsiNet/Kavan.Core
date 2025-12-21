using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureLayer.Configuration;

public class Candle_15mConfiguration : CandleBaseConfiguration<Candle_15m>
{
    public override void Configure(EntityTypeBuilder<Candle_15m> builder)
    {
        base.Configure(builder);
        builder.HasOne(x => x.Cryptocurrency)
            .WithMany(c => c.Candles_15m)
            .HasForeignKey(x => x.CryptocurrencyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
