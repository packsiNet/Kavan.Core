using System.Threading.Tasks;

namespace ApplicationLayer.Interfaces.External;

public interface IMarketDataProxyService
{
    Task<string> GetBinanceKlines(string symbol, string interval);
    Task<string> GetCoinbaseCandles(string symbol, int granularity);
    Task<decimal> GetCurrentPriceAsync(string symbol); // Helper for Manual Close
}

