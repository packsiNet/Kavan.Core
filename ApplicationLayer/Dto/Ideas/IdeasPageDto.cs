namespace ApplicationLayer.DTOs.Ideas;

public class IdeasPageDto
{
    public List<IdeaDto> Items { get; set; } = [];
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public bool IsOwner { get; set; }
}