using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities;

public class SignalCandle : BaseEntityModel, IAuditableEntity
{
    public int SignalId { get; set; }

    public Signal Signal { get; set; } = null!;

    public int Index { get; set; } // position in the snapshot window

    public string Timeframe { get; set; } = string.Empty;

    public DateTime OpenTime { get; set; }

    public DateTime CloseTime { get; set; }

    public decimal Open { get; set; }

    public decimal High { get; set; }

    public decimal Low { get; set; }

    public decimal Close { get; set; }

    public decimal Volume { get; set; }

    public bool IsTrigger { get; set; }
}