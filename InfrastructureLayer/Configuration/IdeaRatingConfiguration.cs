using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureLayer.Configuration;

public class IdeaRatingConfiguration : BaseEntityConfiguration<IdeaRating>
{
    public override void Configure(EntityTypeBuilder<IdeaRating> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Rating)
            .IsRequired();

        builder.HasOne(x => x.Idea)
            .WithMany()
            .HasForeignKey(x => x.IdeaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.IdeaId, x.UserId }).IsUnique();
    }
}