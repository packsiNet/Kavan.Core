using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities;

public class NewsInstrument : BaseEntityModel, IAuditableEntity
{
    public int NewsPostId { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public string Url { get; set; } = string.Empty;

    public decimal? MarketCapUsd { get; set; }

    public decimal? PriceInUsd { get; set; }

    public decimal? PriceInBtc { get; set; }

    public decimal? PriceInEth { get; set; }

    public decimal? PriceInEur { get; set; }

    public int? MarketRank { get; set; }

    public NewsPost NewsPost { get; set; } = null!;
}
