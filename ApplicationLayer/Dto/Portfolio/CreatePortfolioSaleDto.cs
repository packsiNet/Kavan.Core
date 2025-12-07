namespace ApplicationLayer.DTOs.Portfolio;

public class CreatePortfolioSaleDto
{
    public string Symbol { get; set; }
    public decimal Quantity { get; set; }
    public decimal SellPrice { get; set; }
    public DateTime? SellDate { get; set; }
}

