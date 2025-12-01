namespace ApplicationLayer.DTOs.Portfolio;

public class CreatePortfolioEntryDto
{
    public string Symbol { get; set; }
    public decimal Quantity { get; set; }
    public decimal BuyPrice { get; set; }
    public DateTime? BuyDate { get; set; }
}