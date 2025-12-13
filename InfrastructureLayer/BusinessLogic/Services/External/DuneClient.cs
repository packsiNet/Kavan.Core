using System.Text.Json;
using ApplicationLayer.Dto.External.Dune;
using ApplicationLayer.Interfaces.External;
using DomainLayer.Common.Attributes;
using InfrastructureLayer.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace InfrastructureLayer.BusinessLogic.Services.External;

[InjectAsScoped]
    public class DuneClient : IDuneClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly IOptions<DuneOptions> _options;
    private readonly ILogger<DuneClient> _logger;

    public DuneClient(IHttpClientFactory httpClientFactory, IConfiguration configuration, IOptions<DuneOptions> options, ILogger<DuneClient> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _options = options;
        _logger = logger;
    }

    public async Task<DuneQueryResultDto<T>> GetQueryResultsAsync<T>(int queryId, int limit, CancellationToken cancellationToken)
    {
        var apiKey = _options.Value.ApiKey;
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            apiKey = _configuration["Dune:ApiKey"]
                     ?? _configuration["DUNE_API_KEY"]
                     ?? _configuration["Dune__ApiKey"]
                     ?? Environment.GetEnvironmentVariable("DUNE_API_KEY")
                     ?? Environment.GetEnvironmentVariable("Dune__ApiKey");
        }
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            _logger.LogWarning("Dune API key missing");
            return null;
        }

        var client = _httpClientFactory.CreateClient("DuneClient");
        var url = $"query/{queryId}/results?limit={limit}";
        using var req = new HttpRequestMessage(HttpMethod.Get, url);
        req.Headers.Add("x-dune-api-key", apiKey);
        using var res = await client.SendAsync(req, cancellationToken);
        if (!res.IsSuccessStatusCode)
        {
            var body = await res.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogWarning("Dune GET {Url} failed: {Status} Body={Body}", url, res.StatusCode, body.Length > 256 ? body.Substring(0, 256) : body);
            return null;
        }

        var json = await res.Content.ReadAsStringAsync(cancellationToken);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var dto = JsonSerializer.Deserialize<DuneQueryResultDto<T>>(json, options);
        return dto;
    }
}
