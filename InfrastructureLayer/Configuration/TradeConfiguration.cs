using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureLayer.Configuration;

public class TradeConfiguration : IEntityTypeConfiguration<Trade>
{
    public void Configure(EntityTypeBuilder<Trade> builder)
    {
        // Ignore Enums
        builder.Ignore(x => x.SideEnum);
        builder.Ignore(x => x.StatusEnum);

        // Indexes
        builder.HasIndex(x => x.UserAccountId);
        builder.HasIndex(x => x.Symbol);
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.FinancialPeriodId);

        // Properties
        builder.Property(x => x.Symbol)
               .HasMaxLength(50)
               .IsRequired();

        builder.Property(x => x.EntryPrice)
               .HasColumnType("decimal(28,10)");

        builder.Property(x => x.StopLoss)
               .HasColumnType("decimal(28,10)");

        builder.Property(x => x.Quantity)
               .HasColumnType("decimal(28,10)");

        // UserAccount FK (IMPORTANT: NO CASCADE)
        builder.HasOne<UserAccount>()
               .WithMany()
               .HasForeignKey(x => x.UserAccountId)
               .OnDelete(DeleteBehavior.NoAction);

        // FinancialPeriod FK (NO CASCADE)
        builder.HasOne<FinancialPeriod>()
               .WithMany()
               .HasForeignKey(x => x.FinancialPeriodId)
               .OnDelete(DeleteBehavior.NoAction);

        // Owned: Emotion
        builder.OwnsOne(x => x.Emotion, e =>
        {
            e.Property(p => p.EmotionBeforeEntry)
             .HasMaxLength(500);

            e.Property(p => p.ConfidenceLevel);
            e.Property(p => p.PlanCompliance);
        });

        // Owned: Result
        builder.OwnsOne(x => x.Result, r =>
        {
            r.Property(p => p.ExitPrice)
             .HasColumnType("decimal(28,10)");

            r.Property(p => p.RMultiple)
             .HasColumnType("decimal(18,2)");

            r.Property(p => p.PnLPercent)
             .HasColumnType("decimal(18,2)");

            r.Property(p => p.PnL)
             .HasColumnType("decimal(28,10)");

            r.Property(p => p.HoldingTime);
        });

        // Trade → TradeTp (Child entity, CASCADE is OK)
        builder.HasMany(x => x.TakeProfits)
               .WithOne()
               .HasForeignKey(x => x.TradeId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
