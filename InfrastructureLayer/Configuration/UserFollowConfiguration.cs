using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureLayer.Configuration;

public class UserFollowConfiguration : BaseEntityConfiguration<UserFollow>
{
    public override void Configure(EntityTypeBuilder<UserFollow> builder)
    {
        base.Configure(builder);

        builder.ToTable("UserFollow", "dbo");

        builder.HasIndex(x => new { x.FollowerUserId, x.FolloweeUserId })
               .IsUnique();

        builder.HasOne(x => x.FollowerUser)
               .WithMany()
               .HasForeignKey(x => x.FollowerUserId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.FolloweeUser)
               .WithMany()
               .HasForeignKey(x => x.FolloweeUserId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.Property<int>("FollowerUserId").IsRequired();
        builder.Property<int>("FolloweeUserId").IsRequired();
    }
}

