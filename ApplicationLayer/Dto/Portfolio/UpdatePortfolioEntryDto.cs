namespace ApplicationLayer.DTOs.Portfolio;

public class UpdatePortfolioEntryDto
{
    public decimal Quantity { get; set; }
    public decimal BuyPrice { get; set; }
    public DateTime? BuyDate { get; set; }
    public bool IsActive { get; set; }
}