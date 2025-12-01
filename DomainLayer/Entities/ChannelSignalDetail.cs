using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities;

public class ChannelSignalDetail : BaseEntityModel
{
    public int PostId { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public string Timeframe { get; set; } = string.Empty;
    public string TradeType { get; set; } = string.Empty;
    public decimal StopLoss { get; set; }
}