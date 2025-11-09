using ApplicationLayer.Interfaces;
using ApplicationLayer.Interfaces.Services;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace InfrastructureLayer.BusinessLogic.Services.Binance;

public class CandleFetcherBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<CandleFetcherBackgroundService> _logger;
    private readonly IConfiguration _configuration;
    private int _intervalMinutes;

    public CandleFetcherBackgroundService(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<CandleFetcherBackgroundService> logger,
        IConfiguration configuration)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
        _configuration = configuration;
        _intervalMinutes = _configuration.GetValue<int>("CandleFetcher:IntervalMinutes", 1);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Candle Fetcher Background Service started. Interval: {Interval} minutes", _intervalMinutes);

        // تاخیر اولیه برای اطمینان از آماده بودن سیستم
        await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await FetchCandlesForAllSymbolsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Critical error in Candle Fetcher Background Service");
            }

            // انتظار برای اجرای بعدی
            await Task.Delay(TimeSpan.FromMinutes(_intervalMinutes), stoppingToken);
        }

        _logger.LogInformation("Candle Fetcher Background Service stopped");
    }

    private async Task FetchCandlesForAllSymbolsAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var candleManagementService = scope.ServiceProvider.GetRequiredService<ICandleManagementService>();

        try
        {
            // دریافت تمام رمزارزهای فعال (فقط IsActive و حذف‌نشده)
            var cryptoRepo = scope.ServiceProvider.GetRequiredService<IRepository<Cryptocurrency>>();
            var activeCryptocurrencies = await cryptoRepo.Query()
                .Where(c => c.IsActive && !c.IsDeleted)
                .ToListAsync(cancellationToken);

            if (activeCryptocurrencies.Count == 0)
            {
                _logger.LogWarning("No active cryptocurrencies found in database");
                return;
            }

            _logger.LogInformation("Processing {Count} cryptocurrencies", activeCryptocurrencies.Count);

            // پردازش هر رمزارز با مکانیزم retry
            foreach (var crypto in activeCryptocurrencies)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                await ProcessSymbolWithRetryAsync(crypto, candleManagementService, cancellationToken);

                // تاخیر کوتاه بین درخواست‌ها برای جلوگیری از Rate Limiting
                await Task.Delay(100, cancellationToken);
            }

            _logger.LogInformation("Successfully completed processing all cryptocurrencies");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching candles for all symbols");
        }
    }

    private async Task ProcessSymbolWithRetryAsync(
        Cryptocurrency cryptocurrency,
        ICandleManagementService candleManagementService,
        CancellationToken cancellationToken,
        int maxRetries = 3)
    {
        var retryCount = 0;
        var retryDelay = TimeSpan.FromSeconds(5);

        while (retryCount < maxRetries)
        {
            try
            {
                await candleManagementService.ProcessCandlesForSymbolAsync(cryptocurrency);
                return; // موفقیت - خروج از حلقه
            }
            catch (HttpRequestException ex)
            {
                retryCount++;
                if (retryCount >= maxRetries)
                {
                    _logger.LogError(ex, "Failed to process symbol {Symbol} after {RetryCount} retries",
                        cryptocurrency.Symbol, maxRetries);
                    return;
                }

                _logger.LogWarning("Error processing symbol {Symbol}. Retry {RetryCount}/{MaxRetries}. Error: {Error}",
                    cryptocurrency.Symbol, retryCount, maxRetries, ex.Message);

                await Task.Delay(retryDelay * retryCount, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error processing symbol {Symbol}", cryptocurrency.Symbol);
                return; // برای خطاهای غیرمنتظره، تلاش مجدد نمی‌کنیم
            }
        }
    }
}