using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureLayer.Configuration;

public class DuneEtfIssuerFlowSnapshotConfiguration : BaseEntityConfiguration<DuneEtfIssuerFlowSnapshot>
{
    public override void Configure(EntityTypeBuilder<DuneEtfIssuerFlowSnapshot> builder)
    {
        base.Configure(builder);

        builder.ToTable(nameof(DuneEtfIssuerFlowSnapshot), schema: "dbo");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("DuneEtfIssuerFlowSnapshotId");

        builder.Property(x => x.ExecutionId).HasMaxLength(64).IsRequired(false);
        builder.Property(x => x.QueryId).IsRequired();
        builder.Property(x => x.SubmittedAt).IsRequired();
        builder.Property(x => x.ExpiresAt).IsRequired();
        builder.Property(x => x.ExecutionStartedAt).IsRequired();
        builder.Property(x => x.ExecutionEndedAt).IsRequired();
        builder.Property(x => x.RowCount).IsRequired();

        builder.Property(x => x.Time).IsRequired();
        builder.Property(x => x.Issuer).HasMaxLength(128).IsRequired();
        builder.Property(x => x.EtfTicker).HasMaxLength(32).IsRequired();
        builder.Property(x => x.Amount).HasColumnType("decimal(28,10)");
        builder.Property(x => x.AmountUsd).HasColumnType("decimal(28,10)");
        builder.Property(x => x.AmountNetFlow).HasColumnType("decimal(28,10)");
        builder.Property(x => x.AmountUsdNetFlow).HasColumnType("decimal(28,10)");

        builder.HasIndex(x => new { x.Time, x.EtfTicker }).IsUnique();
    }
}
