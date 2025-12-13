using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureLayer.Configuration;

public class DuneGasPriceSnapshotConfiguration : IEntityTypeConfiguration<DuneGasPriceSnapshot>
{
    public void Configure(EntityTypeBuilder<DuneGasPriceSnapshot> builder)
    {
        builder.HasIndex(x => x.Time).IsUnique();
        builder.Property(x => x.ExecutionId).HasMaxLength(64);
        builder.Property(x => x.MedianGasPriceGwei).HasColumnType("decimal(38,20)");
        builder.Property(x => x.EthTransferPriceUsd).HasColumnType("decimal(38,20)");
    }
}
