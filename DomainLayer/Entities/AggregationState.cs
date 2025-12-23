using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities;

public class AggregationState : BaseEntityModel
{
    public int CryptocurrencyId { get; set; }
    
    // e.g. "5m", "1h"
    public string Timeframe { get; set; } = null!;
    
    // The OpenTime of the LAST successfully aggregated candle
    public DateTime LastProcessedOpenTime { get; set; }
    
    public DateTime LastUpdatedUtc { get; set; }

    public Cryptocurrency Cryptocurrency { get; set; } = null!;
}
