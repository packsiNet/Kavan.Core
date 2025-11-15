using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureLayer.Configuration;

public class Candle_1mConfiguration : CandleBaseConfiguration<Candle_1m>
{
    public override void Configure(EntityTypeBuilder<Candle_1m> builder)
    {
        base.Configure(builder);
    }
}