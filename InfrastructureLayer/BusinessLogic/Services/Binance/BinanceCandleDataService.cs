using ApplicationLayer.Dto.Cabdles;
using ApplicationLayer.Interfaces.Binance;
using DomainLayer.Common.Attributes;
using System.Text.Json;
using System.Globalization;

namespace InfrastructureLayer.BusinessLogic.Services.Binance;

[InjectAsScoped]
public class BinanceCandleDataService : ICandleDataProvider
{
    private readonly HttpClient _httpClient;

    public BinanceCandleDataService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://api.binance.com");
    }

    public async Task<IEnumerable<CandleDto>> GetKlinesAsync(string symbol, string interval, int limit = 500, DateTime? startTime = null, DateTime? endTime = null)
    {
        var url = $"https://api.binance.com/api/v3/klines?symbol={symbol}&interval={interval}&limit={limit}";
        var json = await _httpClient.GetStringAsync(url);
        var data = JsonSerializer.Deserialize<List<List<JsonElement>>>(json)!;

        return data.Select(x => new CandleDto
        {
            OpenTime = DateTimeOffset.FromUnixTimeMilliseconds(x[0].GetInt64()).UtcDateTime,
            Open = ParseDecimal(x[1]),
            High = ParseDecimal(x[2]),
            Low = ParseDecimal(x[3]),
            Close = ParseDecimal(x[4]),
            Volume = ParseDecimal(x[5]),
            CloseTime = DateTimeOffset.FromUnixTimeMilliseconds(x[6].GetInt64()).UtcDateTime
        });
    }

    private static decimal ParseDecimal(JsonElement element)
    {
        // Binance klines numbers are string-encoded; parse invariantly
        if (element.ValueKind == JsonValueKind.String)
        {
            var s = element.GetString();
            return decimal.Parse(s!, NumberStyles.Float, CultureInfo.InvariantCulture);
        }

        // If numeric token, use GetDecimal safely
        if (element.ValueKind == JsonValueKind.Number)
        {
            return element.GetDecimal();
        }

        // Fallback
        throw new InvalidOperationException($"Unexpected JSON value kind for decimal: {element.ValueKind}");
    }
}