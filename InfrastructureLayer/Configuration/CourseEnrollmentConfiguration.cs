using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureLayer.Configuration;

public class CourseEnrollmentConfiguration : IEntityTypeConfiguration<CourseEnrollment>
{
    public void Configure(EntityTypeBuilder<CourseEnrollment> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.PricePaid).HasColumnType("decimal(18,2)").HasDefaultValue(0);
        builder.Property(x => x.CouponCode).HasMaxLength(50);
        builder.Property(x => x.PaymentReference).HasMaxLength(100);
        builder.Property(x => x.EnrolledAt).IsRequired();
        builder.HasIndex(x => new { x.UserAccountId, x.CourseId }).IsUnique();
        builder.HasOne(x => x.Course).WithMany().HasForeignKey(x => x.CourseId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.UserAccount).WithMany().HasForeignKey(x => x.UserAccountId).OnDelete(DeleteBehavior.Restrict);
    }
}