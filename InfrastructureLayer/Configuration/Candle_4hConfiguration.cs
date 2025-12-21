using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureLayer.Configuration;

public class Candle_4hConfiguration : CandleBaseConfiguration<Candle_4h>
{
    public override void Configure(EntityTypeBuilder<Candle_4h> builder)
    {
        base.Configure(builder);
        builder.HasOne(x => x.Cryptocurrency)
            .WithMany(c => c.Candles_4h)
            .HasForeignKey(x => x.CryptocurrencyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
