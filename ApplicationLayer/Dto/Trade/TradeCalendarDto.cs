namespace ApplicationLayer.Dto.Trade;

public class TradeCalendarDto
{
    public DateTime Date { get; set; }
    public int TradeCount { get; set; }
    public decimal TotalPnLPercent { get; set; }
    public decimal TotalPnLAmount { get; set; }
}
