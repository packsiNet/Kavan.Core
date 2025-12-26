using DomainLayer.Common.BaseEntities;
using DomainLayer.Common.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomainLayer.Entities;

public class FinancialPeriod : BaseEntityModel, IAuditableEntity
{
    public int UserAccountId { get; set; }
    public UserAccount UserAccount { get; set; }

    public DateTime StartDateUtc { get; set; }
    public DateTime EndDateUtc { get; set; }
    public int PeriodType { get; set; } // FinancialPeriodType
    public bool IsClosed { get; set; }

    // Helper property to work with Enum
    [NotMapped]
    public FinancialPeriodType PeriodTypeEnum
    {
        get => FinancialPeriodType.FromValue(PeriodType);
        set => PeriodType = value.Value;
    }
}
