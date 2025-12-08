using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureLayer.Configuration;

public class IdeaCommentConfiguration : BaseEntityConfiguration<IdeaComment>
{
    public override void Configure(EntityTypeBuilder<IdeaComment> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Comment)
            .IsRequired()
            .HasMaxLength(1000);

        builder.HasOne(x => x.Idea)
            .WithMany()
            .HasForeignKey(x => x.IdeaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}