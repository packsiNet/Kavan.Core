using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities;

public class CandleBase : BaseEntityModel, IAuditableEntity
{
    public int CryptocurrencyId { get; set; }

    public DateTime OpenTime { get; set; }

    public DateTime CloseTime { get; set; }

    public decimal Open { get; set; }

    public decimal High { get; set; }

    public decimal Low { get; set; }

    public decimal Close { get; set; }

    public decimal Volume { get; set; }

    public int NumberOfTrades { get; set; }

    public Cryptocurrency Cryptocurrency { get; set; } = null!;
}