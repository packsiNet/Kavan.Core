using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureLayer.Configuration;

public class Candle_1dConfiguration : CandleBaseConfiguration<Candle_1d>
{
    public override void Configure(EntityTypeBuilder<Candle_1d> builder)
    {
        base.Configure(builder);
    }
}