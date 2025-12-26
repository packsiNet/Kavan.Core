using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureLayer.Configuration;

public class FinancialPeriodConfiguration : IEntityTypeConfiguration<FinancialPeriod>
{
    public void Configure(EntityTypeBuilder<FinancialPeriod> builder)
    {
        builder.Ignore(x => x.PeriodTypeEnum);

        builder.HasIndex(x => x.UserAccountId);
        builder.HasIndex(x => x.IsClosed);

        builder.Property(x => x.StartDateUtc).IsRequired();
        builder.Property(x => x.EndDateUtc).IsRequired();
        builder.Property(x => x.PeriodType).IsRequired();
    }
}
