using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities;

public class Signal : BaseEntityModel, IAuditableEntity
{
    public int CryptocurrencyId { get; set; }

    public string Symbol { get; set; } = string.Empty;

    public string TimeFrame { get; set; } = string.Empty; // 1m,5m,1h,4h,1d

    public string SignalType { get; set; } = string.Empty; // BUY or SELL

    public decimal? Rsi { get; set; }

    public decimal? Ema { get; set; }

    public decimal? Macd { get; set; }

    public DateTime Timestamp { get; set; }

    public Cryptocurrency Cryptocurrency { get; set; } = null!;
}