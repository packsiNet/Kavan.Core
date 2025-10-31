using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureLayer.Configuration;

public class PlanFeatureConfiguration : BaseEntityConfiguration<PlanFeature>
{
    public override void Configure(EntityTypeBuilder<PlanFeature> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Key)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Value)
            .HasMaxLength(200);

        builder.Property(x => x.Unit)
            .HasMaxLength(20);

        builder.HasIndex(x => new { x.PlanId, x.Key }).IsUnique();

        builder.HasOne(x => x.Plan)
               .WithMany(x => x.Features)
               .HasForeignKey(x => x.PlanId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}