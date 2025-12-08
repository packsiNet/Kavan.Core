using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities;

public class Channel : BaseEntityModel, IAuditableEntity
{
    public int OwnerUserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    
    // 1 = News, 2 = Signal
    public int Type { get; set; }
    
    // 1 = Free, 2 = VIP
    public int AccessType { get; set; }
    
    public string UniqueCode { get; set; } = string.Empty;
    
    public decimal Price { get; set; }
    public string Currency { get; set; } = string.Empty;
    
    public string BannerUrl { get; set; } = string.Empty;
    public string LogoUrl { get; set; } = string.Empty;
}
