using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureLayer.Configuration;

public class TradeConfiguration : IEntityTypeConfiguration<Trade>
{
    public void Configure(EntityTypeBuilder<Trade> builder)
    {
        builder.Ignore(x => x.SideEnum);
        builder.Ignore(x => x.StatusEnum);

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.Symbol);
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.FinancialPeriodId);

        builder.Property(x => x.Symbol).HasMaxLength(50).IsRequired();
        builder.Property(x => x.EntryPrice).HasColumnType("decimal(28,10)");
        builder.Property(x => x.StopLoss).HasColumnType("decimal(28,10)");
        builder.Property(x => x.Quantity).HasColumnType("decimal(28,10)");
        
        builder.OwnsOne(x => x.Emotion, e =>
        {
            e.Property(p => p.EmotionBeforeEntry).HasMaxLength(500);
            e.Property(p => p.ConfidenceLevel);
            e.Property(p => p.PlanCompliance);
        });

        builder.OwnsOne(x => x.Result, r =>
        {
            r.Property(p => p.ExitPrice).HasColumnType("decimal(28,10)");
            r.Property(p => p.RMultiple).HasColumnType("decimal(18,2)");
            r.Property(p => p.PnLPercent).HasColumnType("decimal(18,2)");
            r.Property(p => p.HoldingTime);
        });
        
        builder.HasMany(x => x.TakeProfits)
            .WithOne()
            .HasForeignKey(x => x.TradeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class TradeTpConfiguration : IEntityTypeConfiguration<TradeTp>
{
    public void Configure(EntityTypeBuilder<TradeTp> builder)
    {
        builder.HasIndex(x => x.TradeId);
        builder.Property(x => x.Price).HasColumnType("decimal(28,10)");
    }
}
