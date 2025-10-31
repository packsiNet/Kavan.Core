using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureLayer.Configuration;

public class PlanConfiguration : BaseEntityConfiguration<Plan>
{
    public override void Configure(EntityTypeBuilder<Plan> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Code)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.PriceMonthly)
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.PriceYearly)
            .HasColumnType("decimal(18,2)");

        builder.HasIndex(x => x.Code).IsUnique();

        builder.HasMany(x => x.Features)
               .WithOne(x => x.Plan)
               .HasForeignKey(x => x.PlanId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.UserPlans)
               .WithOne(x => x.Plan)
               .HasForeignKey(x => x.PlanId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}