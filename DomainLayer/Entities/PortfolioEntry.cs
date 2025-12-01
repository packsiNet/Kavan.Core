using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities;

public class PortfolioEntry : BaseEntityModel, IAuditableEntity
{
    public int CryptocurrencyId { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal BuyPrice { get; set; }
    public DateTime BuyDate { get; set; }
}