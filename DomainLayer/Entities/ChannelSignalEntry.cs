using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities;

public class ChannelSignalEntry : BaseEntityModel
{
    public int PostId { get; set; }
    public decimal Price { get; set; }
}