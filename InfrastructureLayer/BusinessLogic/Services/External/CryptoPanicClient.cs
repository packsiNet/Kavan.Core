using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using ApplicationLayer.Dto.News;
using ApplicationLayer.Interfaces.External;
using DomainLayer.Common.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using InfrastructureLayer.Configuration;
using Microsoft.Extensions.Logging;

namespace InfrastructureLayer.BusinessLogic.Services.External;

[InjectAsScoped]
public class CryptoPanicClient : ICryptoPanicClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly IOptions<CryptoPanicOptions> _options;
    private readonly ILogger<CryptoPanicClient> _logger;

    public CryptoPanicClient(IHttpClientFactory httpClientFactory, IConfiguration configuration, IOptions<CryptoPanicOptions> options, ILogger<CryptoPanicClient> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _options = options;
        _logger = logger;
    }

    public async Task<IReadOnlyList<CryptoPanicPost>> GetPostsAsync(CryptoPanicQuery query, CancellationToken cancellationToken)
    {
        var authToken = _options.Value.AuthToken
                        ?? _configuration["CryptoPanic:AuthToken"]
                        ?? Environment.GetEnvironmentVariable("CRYPTOPANIC_AUTH_TOKEN");
        if (string.IsNullOrWhiteSpace(authToken))
        {
            _logger.LogWarning("CryptoPanic auth token missing");
            return Array.Empty<CryptoPanicPost>();
        }

        var client = _httpClientFactory.CreateClient("CryptoPanicClient");

        var sb = new StringBuilder("posts/?");
        sb.Append("auth_token=").Append(Uri.EscapeDataString(authToken));

        if (query.Public)
            sb.Append("&public=true");

        if (query.Currencies.Length > 0)
            sb.Append("&currencies=").Append(string.Join(',', query.Currencies));

        if (query.Regions.Length > 0)
            sb.Append("&regions=").Append(string.Join(',', query.Regions));

        var filter = string.IsNullOrWhiteSpace(query.Filter) ? "hot,important" : query.Filter;
        sb.Append("&filter=").Append(filter);

        var kind = string.IsNullOrWhiteSpace(query.Kind) ? "news" : query.Kind;
        sb.Append("&kind=").Append(kind);

        if (query.Following == true)
            sb.Append("&following=true");

        if (!string.IsNullOrWhiteSpace(query.Search))
            sb.Append("&search=").Append(Uri.EscapeDataString(query.Search));

        var url = sb.ToString();

        _logger.LogInformation("CryptoPanic BaseAddress: {Base}", client.BaseAddress);
        _logger.LogInformation("CryptoPanic GET {Url}", url);
        using var req = new HttpRequestMessage(HttpMethod.Get, url);
        using var res = await client.SendAsync(req, cancellationToken);
        if (!res.IsSuccessStatusCode)
        {
            _logger.LogWarning("CryptoPanic posts fetch failed: {StatusCode}", res.StatusCode);
            return Array.Empty<CryptoPanicPost>();
        }

        var json = await res.Content.ReadAsStringAsync(cancellationToken);
        _logger.LogInformation("CryptoPanic response length={Len}", json.Length);
        var doc = JsonDocument.Parse(json);
        if (!doc.RootElement.TryGetProperty("results", out var results) || results.ValueKind != JsonValueKind.Array)
        {
            _logger.LogWarning("CryptoPanic response missing 'results' array. Snippet={Snippet}", json.Substring(0, Math.Min(500, json.Length)));
            return Array.Empty<CryptoPanicPost>();
        }

        var list = new List<CryptoPanicPost>(results.GetArrayLength());
        foreach (var item in results.EnumerateArray())
        {
            var post = new CryptoPanicPost
            {
                Id = item.TryGetProperty("id", out var idp) && idp.ValueKind == JsonValueKind.Number ? idp.GetInt32() : 0,
                Slug = item.TryGetProperty("slug", out var slp) ? (slp.GetString() ?? string.Empty) : string.Empty,
                Title = item.TryGetProperty("title", out var tpp) ? (tpp.GetString() ?? string.Empty) : string.Empty,
                Description = item.TryGetProperty("description", out var dp) ? (dp.GetString() ?? string.Empty) : string.Empty,
                PublishedAt = item.TryGetProperty("published_at", out var pp) && pp.ValueKind == JsonValueKind.String ? pp.GetDateTime() : default,
                CreatedAt = item.TryGetProperty("created_at", out var cp) && cp.ValueKind == JsonValueKind.String ? cp.GetDateTime() : default,
                Kind = item.TryGetProperty("kind", out var kp) ? (kp.GetString() ?? string.Empty) : string.Empty,
                OriginalUrl = item.TryGetProperty("original_url", out var oup) ? (oup.GetString() ?? string.Empty) : string.Empty,
                Url = item.TryGetProperty("url", out var up) ? (up.GetString() ?? string.Empty) : string.Empty,
                Image = item.TryGetProperty("image", out var imgProp) ? (imgProp.GetString() ?? string.Empty) : string.Empty,
                Author = item.TryGetProperty("author", out var authorProp) ? (authorProp.GetString() ?? string.Empty) : string.Empty,
            };

            if (item.TryGetProperty("source", out var source))
            {
                post.Source = new SourceDto
                {
                    Title = source.TryGetProperty("title", out var t) ? (t.GetString() ?? string.Empty) : string.Empty,
                    Region = source.TryGetProperty("region", out var r) ? (r.GetString() ?? string.Empty) : string.Empty,
                    Domain = source.TryGetProperty("domain", out var d) ? (d.GetString() ?? string.Empty) : string.Empty,
                    Type = source.TryGetProperty("type", out var ty) ? (ty.GetString() ?? string.Empty) : string.Empty,
                };
            }

            if (item.TryGetProperty("votes", out var votes))
            {
                post.Votes = new VotesDto
                {
                    Negative = votes.TryGetProperty("negative", out var n) ? n.GetInt32() : 0,
                    Positive = votes.TryGetProperty("positive", out var p) ? p.GetInt32() : 0,
                    Important = votes.TryGetProperty("important", out var i) ? i.GetInt32() : 0,
                    Liked = votes.TryGetProperty("liked", out var l) ? l.GetInt32() : 0,
                    Disliked = votes.TryGetProperty("disliked", out var di) ? di.GetInt32() : 0,
                    Lol = votes.TryGetProperty("lol", out var lo) ? lo.GetInt32() : 0,
                    Toxic = votes.TryGetProperty("toxic", out var tx) ? tx.GetInt32() : 0,
                    Saved = votes.TryGetProperty("saved", out var s) ? s.GetInt32() : 0,
                    Comments = votes.TryGetProperty("comments", out var c) ? c.GetInt32() : 0,
                };
            }

            if (item.TryGetProperty("panic_score", out var ps))
                post.PanicScore = ps.ValueKind == JsonValueKind.Number ? ps.GetInt32() : null;
            if (item.TryGetProperty("panic_score_1h", out var ps1))
                post.PanicScore1h = ps1.ValueKind == JsonValueKind.Number ? ps1.GetInt32() : null;

            if (item.TryGetProperty("content", out var content))
            {
                post.Content = new ContentDto
                {
                    Original = content.TryGetProperty("original", out var o) ? o.GetString() : null,
                    Clean = content.TryGetProperty("clean", out var cl) ? cl.GetString() : null,
                };
            }

            if (item.TryGetProperty("instruments", out var instruments) && instruments.ValueKind == JsonValueKind.Array)
            {
                var instList = new List<InstrumentDto>();
                foreach (var inst in instruments.EnumerateArray())
                {
                    instList.Add(new InstrumentDto
                    {
                        Code = inst.TryGetProperty("code", out var c) ? (c.GetString() ?? string.Empty) : string.Empty,
                        Title = inst.TryGetProperty("title", out var ti) ? (ti.GetString() ?? string.Empty) : string.Empty,
                        Slug = inst.TryGetProperty("slug", out var sl) ? (sl.GetString() ?? string.Empty) : string.Empty,
                        Url = inst.TryGetProperty("url", out var u) ? (u.GetString() ?? string.Empty) : string.Empty,
                        MarketCapUsd = inst.TryGetProperty("market_cap_usd", out var mc) && mc.ValueKind == JsonValueKind.Number ? mc.GetDecimal() : null,
                        PriceInUsd = inst.TryGetProperty("price_in_usd", out var pu) && pu.ValueKind == JsonValueKind.Number ? pu.GetDecimal() : null,
                        PriceInBtc = inst.TryGetProperty("price_in_btc", out var pb) && pb.ValueKind == JsonValueKind.Number ? pb.GetDecimal() : null,
                        PriceInEth = inst.TryGetProperty("price_in_eth", out var pe) && pe.ValueKind == JsonValueKind.Number ? pe.GetDecimal() : null,
                        PriceInEur = inst.TryGetProperty("price_in_eur", out var peur) && peur.ValueKind == JsonValueKind.Number ? peur.GetDecimal() : null,
                        MarketRank = inst.TryGetProperty("market_rank", out var mr) && mr.ValueKind == JsonValueKind.Number ? mr.GetInt32() : null,
                    });
                }
                post.Instruments = instList.ToArray();
            }

            list.Add(post);
        }

        return list;
    }
}
