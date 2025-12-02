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
