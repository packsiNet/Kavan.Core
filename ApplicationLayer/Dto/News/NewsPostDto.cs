namespace ApplicationLayer.Dto.News;

public class NewsPostDto
{
    public int Id { get; set; }
    public int ExternalId { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime PublishedAt { get; set; }
    public string Kind { get; set; } = string.Empty;
    public string OriginalUrl { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public string SourceTitle { get; set; } = string.Empty;
    public string SourceRegion { get; set; } = string.Empty;
    public string SourceDomain { get; set; } = string.Empty;
    public string SourceType { get; set; } = string.Empty;
    public int VotesPositive { get; set; }
    public int VotesNegative { get; set; }
    public int VotesImportant { get; set; }
    public int? PanicScore { get; set; }
    public int? PanicScore1h { get; set; }
    public string Author { get; set; } = string.Empty;
    public string ContentClean { get; set; }
    public NewsInstrumentDto[] Instruments { get; set; } = Array.Empty<NewsInstrumentDto>();
}

public class NewsInstrumentDto
{
    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int? MarketRank { get; set; }
}
