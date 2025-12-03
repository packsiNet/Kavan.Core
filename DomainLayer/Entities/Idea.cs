using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities;

public class Idea : BaseEntityModel, IAuditableEntity
{
    public string Symbol { get; set; } = string.Empty;

    public int? CryptocurrencyId { get; set; }

    public string Timeframe { get; set; } = string.Empty;

    public string Trend { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;
    public string TitleTranslate { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
    public string DescriptionTranslate { get; set; } = string.Empty;

    public string ImageUrl { get; set; } = string.Empty;

    public bool IsPublic { get; set; }

    public string Tags { get; set; } = string.Empty;
}
