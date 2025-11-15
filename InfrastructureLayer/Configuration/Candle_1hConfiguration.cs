using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureLayer.Configuration;

public class Candle_1hConfiguration : CandleBaseConfiguration<Candle_1h>
{
    public override void Configure(EntityTypeBuilder<Candle_1h> builder)
    {
        base.Configure(builder);
    }
}