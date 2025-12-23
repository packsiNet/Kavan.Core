using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities;

public class NewsPost : BaseEntityModel, IAuditableEntity
{
    public int ExternalId { get; set; }

    public string Slug { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime PublishedAt { get; set; }

    public DateTime CreatedAtRemote { get; set; }

    public string Kind { get; set; } = string.Empty;

    public string OriginalUrl { get; set; } = string.Empty;

    public string Url { get; set; } = string.Empty;

    public string Image { get; set; } = string.Empty;

    public string SourceTitle { get; set; } = string.Empty;

    public string SourceRegion { get; set; } = string.Empty;

    public string SourceDomain { get; set; } = string.Empty;

    public string SourceType { get; set; } = string.Empty;

    public int VotesNegative { get; set; }

    public int VotesPositive { get; set; }

    public int VotesImportant { get; set; }

    public int VotesLiked { get; set; }

    public int VotesDisliked { get; set; }

    public int VotesLol { get; set; }

    public int VotesToxic { get; set; }

    public int VotesSaved { get; set; }

    public int VotesComments { get; set; }

    public int? PanicScore { get; set; }

    public int? PanicScore1h { get; set; }

    public string Author { get; set; } = string.Empty;

    public string ContentOriginal { get; set; }

    public string ContentClean { get; set; }

    public ICollection<NewsInstrument> Instruments { get; set; } = [];
}
