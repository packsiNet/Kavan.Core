namespace ApplicationLayer.DTOs.Portfolio;

public class PortfolioEntryDto
{
    public int Id { get; set; }
    public string Symbol { get; set; }
    public decimal Quantity { get; set; }
    public decimal BuyPrice { get; set; }
    public DateTime BuyDate { get; set; }
    public DateTime CreatedAt { get; set; }
}