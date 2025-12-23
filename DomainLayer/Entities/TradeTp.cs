using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities;

public class TradeTp : BaseEntityModel
{
    public int TradeId { get; set; }
    public decimal Price { get; set; }
    public bool IsHit { get; set; }
}
