namespace ApplicationLayer.DTOs.Channels;

public class ChannelDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
    public string Category { get; set; }
    public string AccessType { get; set; }
    public string Description { get; set; }
    public bool IsActive { get; set; }
    public int MembersCount { get; set; }
    public int RatingsCount { get; set; }
    public double AverageStars { get; set; }
}