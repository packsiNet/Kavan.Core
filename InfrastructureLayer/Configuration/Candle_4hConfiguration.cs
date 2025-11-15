using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureLayer.Configuration;

public class Candle_4hConfiguration : CandleBaseConfiguration<Candle_4h>
{
    public override void Configure(EntityTypeBuilder<Candle_4h> builder)
    {
        base.Configure(builder);
    }
}