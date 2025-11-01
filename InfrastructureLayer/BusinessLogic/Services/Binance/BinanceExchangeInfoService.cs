using ApplicationLayer.Dto.Binance;
using ApplicationLayer.Interfaces.Binance;
using DomainLayer.Common.Attributes;
using System.Text.Json;

namespace InfrastructureLayer.BusinessLogic.Services.Binance;

[InjectAsScoped]
public class BinanceExchangeInfoService : IBinanceExchangeInfoService
{
    private readonly HttpClient _httpClient;

    public BinanceExchangeInfoService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://api.binance.com");
    }

    public async Task<List<BinanceSymbolDto>> GetExchangeSymbolsAsync()
    {
        try
        {
            var response = await _httpClient.GetStringAsync("/api/v3/exchangeInfo");
            var exchangeInfo = JsonSerializer.Deserialize<BinanceExchangeInfoDto>(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            return exchangeInfo?.Symbols ?? [];
        }
        catch (Exception ex)
        {
            // Log the error and return empty list as fallback
            Console.WriteLine($"Error fetching exchange info: {ex.Message}");
            return new List<BinanceSymbolDto>();
        }
    }

    public async Task<List<BinanceSymbolDto>> GetUsdtTradingPairsAsync()
    {
        var allSymbols = await GetExchangeSymbolsAsync();
        
        return allSymbols
            .Where(s => s.QuoteAsset == "USDT" && 
                       s.Status == "TRADING" && 
                       s.IsSpotTradingAllowed)
            .OrderBy(s => s.Symbol)
            .ToList();
    }
}