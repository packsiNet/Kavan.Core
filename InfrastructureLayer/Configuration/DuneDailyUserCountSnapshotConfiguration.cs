using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureLayer.Configuration;

public class DuneDailyUserCountSnapshotConfiguration : IEntityTypeConfiguration<BitcoinActiveAddress>
{
    public void Configure(EntityTypeBuilder<BitcoinActiveAddress> builder)
    {
        builder.HasIndex(x => x.Time).IsUnique();
        builder.Property(x => x.ExecutionId).HasMaxLength(64);
        builder.Property(x => x.Users).HasColumnType("decimal(28,10)");
    }
}
