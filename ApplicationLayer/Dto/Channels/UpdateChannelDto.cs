namespace ApplicationLayer.DTOs.Channels;

public class UpdateChannelDto
{
    public string Name { get; set; }
    public int Category { get; set; }
    public int AccessType { get; set; }
    public string Description { get; set; }
    public bool IsActive { get; set; }
}