namespace ApplicationLayer.Dto.News;

public class CryptoPanicPost
{
    public int Id { get; set; }

    public string Slug { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime PublishedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public string Kind { get; set; } = string.Empty;

    public string OriginalUrl { get; set; } = string.Empty;

    public string Url { get; set; } = string.Empty;

    public string Image { get; set; } = string.Empty;

    public SourceDto Source { get; set; } = new();

    public InstrumentDto[] Instruments { get; set; } = Array.Empty<InstrumentDto>();

    public VotesDto Votes { get; set; } = new();

    public int? PanicScore { get; set; }

    public int? PanicScore1h { get; set; }

    public string Author { get; set; } = string.Empty;

    public ContentDto Content { get; set; } = new();
}

public class SourceDto
{
    public string Title { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string Domain { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
}

public class InstrumentDto
{
    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public decimal? MarketCapUsd { get; set; }
    public decimal? PriceInUsd { get; set; }
    public decimal? PriceInBtc { get; set; }
    public decimal? PriceInEth { get; set; }
    public decimal? PriceInEur { get; set; }
    public int? MarketRank { get; set; }
}

public class VotesDto
{
    public int Negative { get; set; }
    public int Positive { get; set; }
    public int Important { get; set; }
    public int Liked { get; set; }
    public int Disliked { get; set; }
    public int Lol { get; set; }
    public int Toxic { get; set; }
    public int Saved { get; set; }
    public int Comments { get; set; }
}

public class ContentDto
{
    public string? Original { get; set; }
    public string? Clean { get; set; }
}
