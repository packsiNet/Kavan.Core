using ApplicationLayer.Interfaces.Binance;
using Microsoft.Extensions.Hosting;

namespace ApplicationLayer.Services;

public class CandleUpdaterHostedService(IBinanceSymbolSyncService _symbolSyncService, ICandleUpdaterService _candleUpdater) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var symbols = await _symbolSyncService.GetActiveSymbolsAsync(); // حالا از DB می‌گیره

        foreach (var symbol in symbols)
        {
            await _candleUpdater.UpdateCandlesAsync(symbol, "1m");
        }

        // بعدش یک delay
        await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
    }
}