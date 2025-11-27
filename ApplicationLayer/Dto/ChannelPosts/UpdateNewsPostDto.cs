namespace ApplicationLayer.DTOs.ChannelPosts;

public class UpdateNewsPostDto
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string Url { get; set; }
    public bool IsActive { get; set; }
}