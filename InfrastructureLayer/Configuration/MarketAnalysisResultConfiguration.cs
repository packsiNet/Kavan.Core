using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureLayer.Configuration;

public class MarketAnalysisResultConfiguration : BaseEntityConfiguration<MarketAnalysisResult>
{
    public override void Configure(EntityTypeBuilder<MarketAnalysisResult> builder)
    {
        base.Configure(builder);

        builder.ToTable("MarketAnalysisResults");

        builder.Property(x => x.RequestId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.AnalyzedAt)
            .IsRequired();

        builder.Property(x => x.ProcessingTime)
            .IsRequired();

        builder.Property(x => x.TotalSymbolsAnalyzed)
            .IsRequired();

        builder.Property(x => x.SignalsGenerated)
            .IsRequired();

        // Configure complex properties as JSON columns
        builder.Property(x => x.Signals)
            .HasColumnType("nvarchar(max)")
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions)null),
                v => System.Text.Json.JsonSerializer.Deserialize<List<TradingSignal>>(v, (System.Text.Json.JsonSerializerOptions)null) ?? new List<TradingSignal>());

        builder.Property(x => x.Metadata)
            .HasColumnType("nvarchar(max)")
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions)null),
                v => System.Text.Json.JsonSerializer.Deserialize<AnalysisMetadata>(v, (System.Text.Json.JsonSerializerOptions)null) ?? new AnalysisMetadata());

        // Indexes for better performance
        builder.HasIndex(x => x.RequestId)
            .IsUnique();

        builder.HasIndex(x => x.AnalyzedAt);
    }
}