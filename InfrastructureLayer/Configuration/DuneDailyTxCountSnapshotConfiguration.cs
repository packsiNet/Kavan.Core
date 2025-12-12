using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureLayer.Configuration;

public class DuneDailyTxCountSnapshotConfiguration : IEntityTypeConfiguration<DuneDailyTxCountSnapshot>
{
    public void Configure(EntityTypeBuilder<DuneDailyTxCountSnapshot> builder)
    {
        builder.HasIndex(x => x.Time).IsUnique();
        builder.Property(x => x.ExecutionId).HasMaxLength(64);
        builder.Property(x => x.TxCount).HasColumnType("decimal(28,10)");
        builder.Property(x => x.TxCountMovingAverage).HasColumnType("decimal(28,10)");
    }
}
