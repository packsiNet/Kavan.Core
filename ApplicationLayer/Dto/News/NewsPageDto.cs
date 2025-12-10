namespace ApplicationLayer.Dto.News;

public class NewsPageDto
{
    public NewsPostDto[] Items { get; set; } = Array.Empty<NewsPostDto>();
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
