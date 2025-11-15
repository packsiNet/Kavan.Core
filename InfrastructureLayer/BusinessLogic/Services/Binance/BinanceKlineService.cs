using ApplicationLayer.Interfaces.Services;
using DomainLayer.Common.Attributes;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace InfrastructureLayer.BusinessLogic.Services.Binance;

[InjectAsScoped]
public class BinanceKlineService : IBinanceKlineService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<BinanceKlineService> _logger;
    private const string BinanceApiBaseUrl = "https://api.binance.com";

    public BinanceKlineService(IHttpClientFactory httpClientFactory, ILogger<BinanceKlineService> logger)
    {
        _httpClient = httpClientFactory.CreateClient("BinanceClient");
        _httpClient.BaseAddress = new Uri(BinanceApiBaseUrl);
        _logger = logger;
    }

    public async Task<List<BinanceKlineDto>> GetKlinesAsync(string symbol, int limit = 5, long? startTime = null, long? endTime = null)
    {
        try
        {
            var queryParams = new List<string>
            {
                $"symbol={symbol}",
                "interval=1m",
                $"limit={limit}"
            };

            if (startTime.HasValue)
                queryParams.Add($"startTime={startTime.Value}");

            if (endTime.HasValue)
                queryParams.Add($"endTime={endTime.Value}");

            var url = $"/api/v3/klines?{string.Join("&", queryParams)}";

            _logger.LogInformation("Fetching klines from Binance for symbol: {Symbol}, Limit: {Limit}", symbol, limit);

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var klines = JsonSerializer.Deserialize<List<List<JsonElement>>>(content);

            if (klines == null || klines.Count == 0)
            {
                _logger.LogWarning("No klines returned from Binance for symbol: {Symbol}", symbol);
                return new List<BinanceKlineDto>();
            }

            var result = klines.Select(k => new BinanceKlineDto
            {
                OpenTime = k[0].GetInt64(),
                OpenPrice = decimal.Parse(k[1].GetString() ?? "0"),
                HighPrice = decimal.Parse(k[2].GetString() ?? "0"),
                LowPrice = decimal.Parse(k[3].GetString() ?? "0"),
                ClosePrice = decimal.Parse(k[4].GetString() ?? "0"),
                Volume = decimal.Parse(k[5].GetString() ?? "0"),
                CloseTime = k[6].GetInt64(),
                QuoteAssetVolume = decimal.Parse(k[7].GetString() ?? "0"),
                NumberOfTrades = k[8].GetInt32(),
                TakerBuyBaseAssetVolume = decimal.Parse(k[9].GetString() ?? "0"),
                TakerBuyQuoteAssetVolume = decimal.Parse(k[10].GetString() ?? "0")
            }).ToList();

            _logger.LogInformation("Successfully fetched {Count} klines for symbol: {Symbol}", result.Count, symbol);
            return result;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error while fetching klines from Binance for symbol: {Symbol}", symbol);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while fetching klines from Binance for symbol: {Symbol}", symbol);
            throw;
        }
    }
}