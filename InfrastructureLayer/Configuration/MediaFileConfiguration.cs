using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureLayer.Configuration;

public class MediaFileConfiguration : IEntityTypeConfiguration<MediaFile>
{
    public void Configure(EntityTypeBuilder<MediaFile> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.FileName).HasMaxLength(260).IsRequired();
        builder.Property(x => x.StorageKey).HasMaxLength(260).IsRequired();
        builder.Property(x => x.MimeType).HasMaxLength(100).IsRequired();
        builder.Property(x => x.SizeBytes).IsRequired();
        builder.Property(x => x.MediaFileTypeValue).IsRequired();
        builder.Property(x => x.IsStreamOnly).HasDefaultValue(true);
        builder.HasIndex(x => new { x.LessonId, x.MediaFileTypeValue });
        builder.HasOne(x => x.Lesson).WithMany(x => x.MediaFiles).HasForeignKey(x => x.LessonId).OnDelete(DeleteBehavior.Restrict);
    }
}