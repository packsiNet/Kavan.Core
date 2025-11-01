using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureLayer.Configuration;

public class TechnicalSignalConfiguration : IEntityTypeConfiguration<TechnicalSignal>
{
    public void Configure(EntityTypeBuilder<TechnicalSignal> builder)
    {
        builder.ToTable("TechnicalSignals");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Symbol)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.IndicatorCategory)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.IndicatorName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.ConditionTitle)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.SignalType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(x => x.DetailedSignalType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(x => x.TimeFrame)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.Value)
            .HasPrecision(18, 8);

        builder.Property(x => x.AdditionalData)
            .HasColumnType("nvarchar(max)");

        // Indexes for better query performance
        builder.HasIndex(x => x.Symbol);
        builder.HasIndex(x => x.IndicatorCategory);
        builder.HasIndex(x => x.IndicatorName);
        builder.HasIndex(x => x.SignalType);
        builder.HasIndex(x => x.DetailedSignalType);
        builder.HasIndex(x => x.TimeFrame);
        builder.HasIndex(x => x.CreatedAt);
        
        // Composite index for common filtering scenarios
        builder.HasIndex(x => new { x.Symbol, x.IndicatorCategory, x.SignalType, x.DetailedSignalType, x.TimeFrame });
    }
}