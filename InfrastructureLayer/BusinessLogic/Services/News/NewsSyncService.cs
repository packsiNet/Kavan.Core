using ApplicationLayer.Dto.News;
using ApplicationLayer.Interfaces;
using ApplicationLayer.Interfaces.External;
using ApplicationLayer.Interfaces.Services.News;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InfrastructureLayer.BusinessLogic.Services.News;

[InjectAsScoped]
public class NewsSyncService : INewsSyncService
{
    private readonly IUnitOfWork _uow;
    private readonly IRepository<NewsPost> _newsRepo;
    private readonly IRepository<NewsInstrument> _instRepo;
    private readonly ICryptoPanicClient _client;
    private readonly ILogger<NewsSyncService> _logger;

    public NewsSyncService(IUnitOfWork uow, IRepository<NewsPost> newsRepo, IRepository<NewsInstrument> instRepo, ICryptoPanicClient client, ILogger<NewsSyncService> logger)
    {
        _uow = uow;
        _newsRepo = newsRepo;
        _instRepo = instRepo;
        _client = client;
        _logger = logger;
    }

    public async Task<int> SyncLatestAsync(CryptoPanicQuery query, CancellationToken cancellationToken)
    {
        var posts = await _client.GetPostsAsync(query, cancellationToken);
        if (posts.Count == 0) return 0;

        var existingIds = await _newsRepo.Query()
            .Select(x => x.ExternalId)
            .ToListAsync(cancellationToken);

        var newCount = 0;

        foreach (var p in posts)
        {
            var isExisting = existingIds.Contains(p.Id);
            if (!isExisting)
            {
                var entity = new NewsPost
                {
                    ExternalId = p.Id,
                    Slug = p.Slug,
                    Title = p.Title,
                    Description = p.Description,
                    PublishedAt = p.PublishedAt,
                    CreatedAtRemote = p.CreatedAt,
                    Kind = p.Kind,
                    OriginalUrl = p.OriginalUrl,
                    Url = p.Url,
                    Image = p.Image,
                    SourceTitle = p.Source.Title,
                    SourceRegion = p.Source.Region,
                    SourceDomain = p.Source.Domain,
                    SourceType = p.Source.Type,
                    VotesNegative = p.Votes.Negative,
                    VotesPositive = p.Votes.Positive,
                    VotesImportant = p.Votes.Important,
                    VotesLiked = p.Votes.Liked,
                    VotesDisliked = p.Votes.Disliked,
                    VotesLol = p.Votes.Lol,
                    VotesToxic = p.Votes.Toxic,
                    VotesSaved = p.Votes.Saved,
                    VotesComments = p.Votes.Comments,
                    PanicScore = p.PanicScore,
                    PanicScore1h = p.PanicScore1h,
                    Author = p.Author,
                    ContentOriginal = p.Content.Original,
                    ContentClean = p.Content.Clean,
                };

                await _newsRepo.AddAsync(entity);
                await _uow.SaveChangesAsync();

                if (p.Instruments.Length > 0)
                {
                    var instruments = p.Instruments.Select(i => new NewsInstrument
                    {
                        NewsPostId = entity.Id,
                        Code = i.Code,
                        Title = i.Title,
                        Slug = i.Slug,
                        Url = i.Url,
                        MarketCapUsd = i.MarketCapUsd,
                        PriceInUsd = i.PriceInUsd,
                        PriceInBtc = i.PriceInBtc,
                        PriceInEth = i.PriceInEth,
                        PriceInEur = i.PriceInEur,
                        MarketRank = i.MarketRank,
                    }).ToArray();
                    await _instRepo.AddRangeAsync(instruments);
                    await _uow.SaveChangesAsync();
                }

                newCount++;
            }
            else
            {
                var entity = await _newsRepo.Query().FirstOrDefaultAsync(x => x.ExternalId == p.Id, cancellationToken);
                if (entity != null)
                {
                    entity.VotesNegative = p.Votes.Negative;
                    entity.VotesPositive = p.Votes.Positive;
                    entity.VotesImportant = p.Votes.Important;
                    entity.VotesLiked = p.Votes.Liked;
                    entity.VotesDisliked = p.Votes.Disliked;
                    entity.VotesLol = p.Votes.Lol;
                    entity.VotesToxic = p.Votes.Toxic;
                    entity.VotesSaved = p.Votes.Saved;
                    entity.VotesComments = p.Votes.Comments;
                    entity.PanicScore = p.PanicScore;
                    entity.PanicScore1h = p.PanicScore1h;
                    await _newsRepo.UpdateAsync(entity);
                    await _uow.SaveChangesAsync();
                }
            }
        }

        _logger.LogInformation("News sync complete, new: {NewCount}, total fetched: {Fetched}", newCount, posts.Count);
        return newCount;
    }
}
