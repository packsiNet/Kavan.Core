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
        var client = _httpClientFactory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.binance.com/api/v3/klines?symbol={symbol}&interval={interval}");
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Headers.UserAgent.ParseAdd("Kavan-Core/MarketDataProxyService");
        using var response = await client.SendAsync(request);
        var body = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            throw new System.Exception($"Binance request failed ({(int)response.StatusCode}): {body}");
        return body;
    }

    public async Task<string> GetCoinbaseCandles(string symbol, int granularity)
    {
        var client = _httpClientFactory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.exchange.coinbase.com/products/{symbol}/candles?granularity={granularity}");
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Headers.UserAgent.ParseAdd("Kavan-Core/MarketDataProxyService");
        using var response = await client.SendAsync(request);
        var body = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            throw new System.Exception($"Coinbase request failed ({(int)response.StatusCode}): {body}");
        return body;
    }
}

