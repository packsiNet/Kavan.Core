using ApplicationLayer.Interfaces.External;
using DomainLayer.Common.Attributes;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace InfrastructureLayer.BusinessLogic.Services.External;

[InjectAsScoped]
public class OpenRouterAiProvider : IAiProvider
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public OpenRouterAiProvider(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task<string> TranslateAsync(string inputText, CancellationToken cancellationToken = default)
    {
        var apiKey = _configuration["OpenRouter:ApiKey"] ?? Environment.GetEnvironmentVariable("OPENROUTER_API_KEY");
        var model = _configuration["OpenRouter:Model"] ?? "google/gemma-3-27b-it:free";
        var baseUrl = _configuration["OpenRouter:BaseUrl"] ?? "https://openrouter.ai/api/v1";

        if (string.IsNullOrWhiteSpace(apiKey))
            throw new InvalidOperationException("OpenRouter API key is not configured");

        var client = _httpClientFactory.CreateClient("OpenRouterClient");
        client.BaseAddress = new Uri(baseUrl);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var prompt = "You are a translator. If the input is Persian (Farsi), respond with the English translation only. If the input is English, respond with the Persian translation only. Do not add explanations or extra text.";

        var payload = new
        {
            model,
            messages = new[]
            {
                new { role = "system", content = prompt },
                new { role = "user", content = inputText }
            }
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        using var response = await client.PostAsync("/chat/completions", content, cancellationToken);
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        using var doc = JsonDocument.Parse(body);
        var choice = doc.RootElement.GetProperty("choices")[0];
        var msg = choice.GetProperty("message");
        var output = msg.GetProperty("content").GetString();
        return output ?? string.Empty;
    }
}

