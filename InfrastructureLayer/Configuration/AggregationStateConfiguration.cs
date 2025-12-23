using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureLayer.Configuration;

public class AggregationStateConfiguration : BaseEntityConfiguration<AggregationState>
{
    public override void Configure(EntityTypeBuilder<AggregationState> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Timeframe).HasMaxLength(10).IsRequired();
        
        // Unique index ensures one state per crypto+timeframe
        builder.HasIndex(x => new { x.CryptocurrencyId, x.Timeframe }).IsUnique();
    }
}
