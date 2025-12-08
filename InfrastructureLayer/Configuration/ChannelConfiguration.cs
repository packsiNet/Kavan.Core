using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureLayer.Configuration;

public class ChannelConfiguration : IEntityTypeConfiguration<Channel>
{
    public void Configure(EntityTypeBuilder<Channel> builder)
    {
        builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Slug).HasMaxLength(220).IsRequired();
        // Category/Type is now int
        builder.Property(x => x.Type).IsRequired();
        // AccessType is now int
        builder.Property(x => x.AccessType).IsRequired();
        
        builder.Property(x => x.UniqueCode).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Currency).HasMaxLength(10);
        builder.Property(x => x.Price).HasColumnType("decimal(18,2)");
        builder.Property(x => x.BannerUrl).HasMaxLength(500);
        builder.Property(x => x.LogoUrl).HasMaxLength(500);

        builder.HasIndex(x => x.Slug).IsUnique();
        builder.HasIndex(x => x.UniqueCode).IsUnique();
        builder.HasIndex(x => x.OwnerUserId);
    }
}

public class ChannelMembershipConfiguration : IEntityTypeConfiguration<ChannelMembership>
{
    public void Configure(EntityTypeBuilder<ChannelMembership> builder)
    {
        builder.HasIndex(x => new { x.ChannelId, x.UserId }).IsUnique();
    }
}

public class ChannelRatingConfiguration : IEntityTypeConfiguration<ChannelRating>
{
    public void Configure(EntityTypeBuilder<ChannelRating> builder)
    {
        builder.Property(x => x.Stars).IsRequired();
        builder.HasIndex(x => new { x.ChannelId, x.UserId }).IsUnique();
        builder.HasIndex(x => x.ChannelId);
    }
}

public class ChannelPostConfiguration : IEntityTypeConfiguration<ChannelPost>
{
    public void Configure(EntityTypeBuilder<ChannelPost> builder)
    {
        builder.Property(x => x.Type).HasMaxLength(20).IsRequired();
        builder.Property(x => x.Title).HasMaxLength(200);
        builder.Property(x => x.ImageUrl).HasMaxLength(500);
        builder.HasIndex(x => x.ChannelId);
    }
}

public class ChannelPostReactionConfiguration : IEntityTypeConfiguration<ChannelPostReaction>
{
    public void Configure(EntityTypeBuilder<ChannelPostReaction> builder)
    {
        builder.Property(x => x.Reaction).HasMaxLength(10).IsRequired();
        builder.HasIndex(x => new { x.PostId, x.UserId }).IsUnique();
        builder.HasIndex(x => x.PostId);
    }
}

public class ChannelPostCommentConfiguration : IEntityTypeConfiguration<ChannelPostComment>
{
    public void Configure(EntityTypeBuilder<ChannelPostComment> builder)
    {
        builder.Property(x => x.Comment).HasMaxLength(1000).IsRequired();
        builder.HasIndex(x => x.PostId);
    }
}

public class ChannelSignalDetailConfiguration : IEntityTypeConfiguration<ChannelSignalDetail>
{
    public void Configure(EntityTypeBuilder<ChannelSignalDetail> builder)
    {
        builder.Property(x => x.Symbol).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Timeframe).HasMaxLength(20).IsRequired();
        builder.Property(x => x.TradeType).HasMaxLength(20).IsRequired();
    }
}

public class ChannelSignalEntryConfiguration : IEntityTypeConfiguration<ChannelSignalEntry>
{
    public void Configure(EntityTypeBuilder<ChannelSignalEntry> builder)
    {
        builder.HasIndex(x => x.PostId);
    }
}

public class ChannelSignalTpConfiguration : IEntityTypeConfiguration<ChannelSignalTp>
{
    public void Configure(EntityTypeBuilder<ChannelSignalTp> builder)
    {
        builder.HasIndex(x => x.PostId);
    }
}

public class ChannelNewsDetailConfiguration : IEntityTypeConfiguration<ChannelNewsDetail>
{
    public void Configure(EntityTypeBuilder<ChannelNewsDetail> builder)
    {
        builder.Property(x => x.Url).HasMaxLength(1000);
    }
}
