using ApplicationLayer.Interfaces.Binance;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using System.Text.Json;

namespace InfrastructureLayer.BusinessLogic.Services.Binance;

[InjectAsScoped]
public class BinanceCandleDataService : ICandleDataService
{
    private readonly HttpClient _httpClient;

    public BinanceCandleDataService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://api.binance.com");
    }

    public async Task<IEnumerable<Candle_1m>> GetCandlesAsync(string symbol, string interval, int limit = 500)
    {
        var url = $"https://api.binance.com/api/v3/klines?symbol={symbol}&interval={interval}&limit={limit}";
        var json = await _httpClient.GetStringAsync(url);
        var data = JsonSerializer.Deserialize<List<List<JsonElement>>>(json)!;

        return data.Select(x => new Candle_1m
        {
            OpenTime = DateTimeOffset.FromUnixTimeMilliseconds(x[0].GetInt64()).UtcDateTime,
            Open = x[1].GetDecimal(),
            High = x[2].GetDecimal(),
            Low = x[3].GetDecimal(),
            Close = x[4].GetDecimal(),
            Volume = x[5].GetDecimal(),
            CloseTime = DateTimeOffset.FromUnixTimeMilliseconds(x[6].GetInt64()).UtcDateTime
        });
    }
}