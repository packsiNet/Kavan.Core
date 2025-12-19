using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities;

public class Cryptocurrency : BaseEntityModel, IAuditableEntity
{
    public string Category { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Symbol { get; set; } = string.Empty;

    public string BaseAsset { get; set; } = string.Empty;

    public string QuoteAsset { get; set; } = string.Empty;

    public ICollection<Candle_1m> Candles_1m { get; set; } = [];

    public ICollection<Candle_5m> Candles_5m { get; set; } = [];

    public ICollection<Candle_1h> Candles_1h { get; set; } = [];

    public ICollection<Candle_4h> Candles_4h { get; set; } = [];

    public ICollection<Candle_1d> Candles_1d { get; set; } = [];
}