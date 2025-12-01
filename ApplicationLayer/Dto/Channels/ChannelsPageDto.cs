namespace ApplicationLayer.DTOs.Channels;

public class ChannelsPageDto
{
    public List<ChannelDto> Items { get; set; } = [];
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}