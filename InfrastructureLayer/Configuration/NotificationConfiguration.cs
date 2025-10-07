using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureLayer.Configuration;

public class NotificationConfiguration : BaseEntityConfiguration<Notification>
{
    public override void Configure(EntityTypeBuilder<Notification> builder)
    {
        base.Configure(builder);

        builder.Property(n => n.Title)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(n => n.Message)
               .IsRequired()
               .HasMaxLength(1000);

        builder.Property(n => n.IsRead)
               .HasDefaultValue(false);

        builder.Property(n => n.CreatedAt)
               .IsRequired();

        builder.HasOne(n => n.UserAccount)
               .WithMany(n => n.Notifications)
               .HasForeignKey(n => n.UserAccountId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}