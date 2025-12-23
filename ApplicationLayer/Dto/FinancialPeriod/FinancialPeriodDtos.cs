using ApplicationLayer.Dto.BaseDtos;

namespace ApplicationLayer.Dto.FinancialPeriod;

public class CreateFinancialPeriodDto
{
    public DateTime StartDateUtc { get; set; }
    public DateTime EndDateUtc { get; set; }
    public int PeriodType { get; set; } // FinancialPeriodType value
}

public class FinancialPeriodDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime StartDateUtc { get; set; }
    public DateTime EndDateUtc { get; set; }
    public int PeriodType { get; set; }
    public string PeriodTypeName { get; set; }
    public bool IsClosed { get; set; }
    public DateTime CreatedAt { get; set; }
}
