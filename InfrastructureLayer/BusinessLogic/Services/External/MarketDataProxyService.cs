using ApplicationLayer.Interfaces.External;
using DomainLayer.Common.Attributes;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace InfrastructureLayer.BusinessLogic.Services.External;

[InjectAsScoped]
public class MarketDataProxyService : IMarketDataProxyService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public MarketDataProxyService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<string> GetBinanceKlines(string symbol, string interval)
    {
        var client = _httpClientFactory.CreateClient("BinanceClient");
        using var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v3/klines?symbol={symbol}&interval={interval}");
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Headers.UserAgent.ParseAdd("Kavan-Core/MarketDataProxyService");
        using var response = await SendAsyncWithRetry(client, request);
        var body = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            throw new System.Exception($"Binance request failed ({(int)response.StatusCode}): {body}");
        return body;
    }

    public async Task<string> GetCoinbaseCandles(string symbol, int granularity)
    {
        var client = _httpClientFactory.CreateClient("CoinbaseClient");
        using var request = new HttpRequestMessage(HttpMethod.Get, $"/products/{symbol}/candles?granularity={granularity}");
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Headers.UserAgent.ParseAdd("Kavan-Core/MarketDataProxyService");
        using var response = await SendAsyncWithRetry(client, request);
        var body = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            throw new System.Exception($"Coinbase request failed ({(int)response.StatusCode}): {body}");
        return body;
    }

    public async Task<decimal> GetCurrentPriceAsync(string symbol)
    {
        // Fetch 1m kline for latest price from Binance
        // Format: [ [OpenTime, Open, High, Low, Close, Volume, ...], ... ]
        // We take the Close price of the latest candle.
        
        var json = await GetBinanceKlines(symbol, "1m");
        // Simple parsing or use Newtonsoft/System.Text.Json
        // Ideally use a proper library but here we just need the price.
        
        // Assuming the response is a JSON array of arrays.
        // We can parse it as JsonElement
        
        using var doc = System.Text.Json.JsonDocument.Parse(json);
        var root = doc.RootElement;
        var count = root.GetArrayLength();
        if (count == 0) throw new Exception("No market data available.");
        
        var latest = root[count - 1]; // Last candle
        // Index 4 is Close Price
        var closePriceStr = latest[4].GetString();
        
        if (decimal.TryParse(closePriceStr, out var price))
        {
            return price;
        }
        
        throw new Exception("Failed to parse price from market data.");
    }

    private static async Task<HttpResponseMessage> SendAsyncWithRetry(HttpClient client, HttpRequestMessage request, int maxAttempts = 3)
    {
        Exception last = null;
        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                return await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            }
            catch (HttpRequestException ex)
            {
                last = ex;
            }
            catch (TaskCanceledException ex)
            {
                last = ex;
            }
            if (attempt < maxAttempts)
                await Task.Delay(TimeSpan.FromMilliseconds(250 * attempt * attempt));
        }
        throw new System.Exception($"Network request failed: {last?.Message}");
    }
}
