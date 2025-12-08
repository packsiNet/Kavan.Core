namespace ApplicationLayer.DTOs.Channels;

public class ChannelDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Slug { get; set; }
    public string UniqueCode { get; set; }
    
    public int TypeId { get; set; }
    public string Type { get; set; }
    
    public int AccessTypeId { get; set; }
    public string AccessType { get; set; }
    
    public bool IsPremium { get; set; }
    
    public decimal Price { get; set; }
    public string Currency { get; set; }
    
    public string BannerUrl { get; set; }
    public string LogoUrl { get; set; }
    
    public bool IsActive { get; set; } // Added back
    
    public bool IsJoined { get; set; }
    public int MembersCount { get; set; }
    public int RatingsCount { get; set; }
    public double AverageStars { get; set; }
}
