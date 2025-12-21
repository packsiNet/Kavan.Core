using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureLayer.Configuration;

public abstract class CandleBaseConfiguration<T> : BaseEntityConfiguration<T> where T : CandleBase
{
    public override void Configure(EntityTypeBuilder<T> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Open).HasPrecision(18, 8);
        builder.Property(x => x.High).HasPrecision(18, 8);
        builder.Property(x => x.Low).HasPrecision(18, 8);
        builder.Property(x => x.Close).HasPrecision(18, 8);
        builder.Property(x => x.Volume).HasPrecision(18, 8);

        builder.HasIndex(x => new { x.CryptocurrencyId, x.OpenTime })
               .IsUnique();

        builder.HasIndex(x => x.OpenTime);
    }
}
