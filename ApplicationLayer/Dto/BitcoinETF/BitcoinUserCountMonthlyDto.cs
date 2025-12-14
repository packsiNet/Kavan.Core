namespace ApplicationLayer.Dto.BitcoinETF;

public class BitcoinUserCountMonthlyDto
{
    public DateTime MonthStart { get; set; }
    public DateTime MonthEnd { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal Users { get; set; }
}
