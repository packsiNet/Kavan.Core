using Microsoft.AspNetCore.Http;

namespace ApplicationLayer.DTOs.ChannelPosts;

public class CreateNewsPostDto
{
    public int ChannelId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public IFormFile Image { get; set; }
    public string Url { get; set; }
}