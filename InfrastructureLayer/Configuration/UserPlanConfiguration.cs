using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureLayer.Configuration;

public class UserPlanConfiguration : BaseEntityConfiguration<UserPlan>
{
    public override void Configure(EntityTypeBuilder<UserPlan> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.StartDate)
            .IsRequired();

        builder.ToTable("UserPlans", t
            => { t.HasCheckConstraint("CK_UserPlan_EndDate", "[EndDate] IS NULL OR [EndDate] >= [StartDate]"); });

        builder.HasOne(x => x.UserAccount)
               .WithMany(x => x.UserPlans)
               .HasForeignKey(x => x.UserAccountId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Plan)
               .WithMany(x => x.UserPlans)
               .HasForeignKey(x => x.PlanId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.UserAccountId, x.PlanId, x.IsActive });
    }
}