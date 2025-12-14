namespace ApplicationLayer.Dto.BitcoinETF;

public class BitcoinUserCountWeeklyDto
{
    public DateTime WeekStart { get; set; }
    public DateTime WeekEnd { get; set; }
    public decimal Users { get; set; }
}
