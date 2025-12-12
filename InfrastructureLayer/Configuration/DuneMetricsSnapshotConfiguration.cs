using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureLayer.Configuration;

public class DuneMetricsSnapshotConfiguration : IEntityTypeConfiguration<DuneMetricsSnapshot>
{
    public void Configure(EntityTypeBuilder<DuneMetricsSnapshot> builder)
    {
        builder.HasIndex(x => x.ExecutionId).IsUnique();
        builder.Property(x => x.ExecutionId).HasMaxLength(64);

        builder.Property(x => x.TvlInThousands).HasColumnType("decimal(28,10)");
        builder.Property(x => x.UsdTvlInBillions).HasColumnType("decimal(28,10)");
        builder.Property(x => x.PastWeekFlows).HasColumnType("decimal(28,10)");
        builder.Property(x => x.FlowsUsdSinceApprovalInThousands).HasColumnType("decimal(28,10)");
        builder.Property(x => x.PastWeekFlowsUsdInThousands).HasColumnType("decimal(28,10)");
        builder.Property(x => x.PercentageOfBtc).HasColumnType("decimal(28,10)");
        builder.Property(x => x.BtcSupply).HasColumnType("decimal(28,10)");
        builder.Property(x => x.SixMonthsAnnualisedImpactOnSupply).HasColumnType("decimal(28,10)");
        builder.Property(x => x.ThreeMonthsAnnualisedImpactOnSupply).HasColumnType("decimal(28,10)");
        builder.Property(x => x.MonthlyAnnualisedImpactOnSupply).HasColumnType("decimal(28,10)");
        builder.Property(x => x.ByWeeklyAnnualisedImpactOnSupply).HasColumnType("decimal(28,10)");
        builder.Property(x => x.WeekAnnualisedImpactOnSupply).HasColumnType("decimal(28,10)");
    }
}
