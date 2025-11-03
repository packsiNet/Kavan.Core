using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureLayer.Configuration;

public class Candle_5mConfiguration : CandleBaseConfiguration<Candle_5m>
{
    public override void Configure(EntityTypeBuilder<Candle_5m> builder)
    {
        base.Configure(builder);
    }
}
