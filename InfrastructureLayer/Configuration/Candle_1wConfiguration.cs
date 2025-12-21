using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureLayer.Configuration;

public class Candle_1wConfiguration : CandleBaseConfiguration<Candle_1w>
{
    public override void Configure(EntityTypeBuilder<Candle_1w> builder)
    {
        base.Configure(builder);
        builder.HasOne(x => x.Cryptocurrency)
            .WithMany(c => c.Candles_1w)
            .HasForeignKey(x => x.CryptocurrencyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}