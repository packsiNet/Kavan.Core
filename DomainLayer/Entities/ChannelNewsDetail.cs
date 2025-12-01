using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities;

public class ChannelNewsDetail : BaseEntityModel
{
    public int PostId { get; set; }
    public string Url { get; set; } = string.Empty;
}