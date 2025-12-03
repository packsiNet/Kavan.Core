using ApplicationLayer.Interfaces.External;
using DomainLayer.Common.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace InfrastructureLayer.BusinessLogic.Services.External;

[InjectAsScoped]
public class OpenRouterAiProvider : IAiProvider
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<OpenRouterAiProvider> _logger;

    public OpenRouterAiProvider(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<OpenRouterAiProvider> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<string> TranslateAsync(string inputText, CancellationToken cancellationToken = default)
    {
        var apiKey = _configuration["OpenRouter:ApiKey"]
                     ?? Environment.GetEnvironmentVariable("OPENROUTER_API_KEY");

        var model = _configuration["OpenRouter:Model"]
                    ?? "google/gemma-3-27b-it:free";

        var baseUrl = _configuration["OpenRouter:BaseUrl"]
                      ?? "https://openrouter.ai/api/v1";

        if (string.IsNullOrWhiteSpace(apiKey))
            throw new InvalidOperationException("OpenRouter API key is not configured");

        var client = _httpClientFactory.CreateClient("OpenRouterClient");
        client.BaseAddress = new Uri(baseUrl);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);


        var payload = new
        {
            model = "google/gemma-3-27b-it:free",
            messages = new[]
            {
                new
                {
                    role = "user",
                    content =
                    "Translate the following text.\n" +
                    "If it is Persian, output ONLY English.\n" +
                    "If it is English, output ONLY Persian.\n" +
                    "Do not repeat the original text.\n" +
                    "Do not add explanations.\n\n" +
                    $"Input:\n{inputText}\n\nOutput:"
                }
            }
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        using var response = await client.PostAsync("/chat/completions", content, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("OpenRouter translation failed: {Status} {Body}",
                               (int)response.StatusCode, body);
            throw new HttpRequestException($"OpenRouter error {(int)response.StatusCode}: {body}");
        }

        using var doc = JsonDocument.Parse(body);
        var choice = doc.RootElement.GetProperty("choices")[0];
        var msg = choice.GetProperty("message");
        var output = msg.GetProperty("content").GetString();

        return output?.Trim() ?? string.Empty;
    }
}
