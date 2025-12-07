using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities;

public class PortfolioSale : BaseEntityModel, IAuditableEntity
{
    public int CryptocurrencyId { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal SellPrice { get; set; }
    public DateTime SellDate { get; set; }
}

