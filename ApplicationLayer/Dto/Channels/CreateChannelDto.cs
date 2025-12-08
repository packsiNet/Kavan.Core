namespace ApplicationLayer.DTOs.Channels;

public class CreateChannelDto
{
    public string Title { get; set; }
    public string Description { get; set; }
    public int Type { get; set; } // 1=News, 2=Signal
    public int AccessType { get; set; } // 1=Free, 2=VIP
    public decimal Price { get; set; }
    public string Currency { get; set; }
    public string BannerUrl { get; set; }
    public string LogoUrl { get; set; }
}
