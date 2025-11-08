using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureLayer.Configuration
{
    public class SignalCandleConfiguration : IEntityTypeConfiguration<SignalCandle>
    {
        public void Configure(EntityTypeBuilder<SignalCandle> builder)
        {
            builder.ToTable("SignalCandles");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Timeframe).HasMaxLength(10).IsRequired();
            builder.Property(x => x.Index).IsRequired();
        }
    }
}