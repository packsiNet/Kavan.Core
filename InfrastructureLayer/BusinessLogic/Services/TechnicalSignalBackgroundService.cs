using ApplicationLayer.Interfaces;
using ApplicationLayer.Interfaces.Services;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace InfrastructureLayer.BusinessLogic.Services;

public class TechnicalSignalBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TechnicalSignalBackgroundService> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(5); // Run every 5 minutes

    public TechnicalSignalBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<TechnicalSignalBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Technical Signal Background Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessSignalsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing technical signals");
            }

            await Task.Delay(_interval, stoppingToken);
        }
    }

    private async Task ProcessSignalsAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var signalEvaluatorFactory = scope.ServiceProvider.GetRequiredService<ISignalEvaluatorFactory>();
        var cryptoRepository = scope.ServiceProvider.GetRequiredService<IRepository<Cryptocurrency>>();
        var signalRepository = scope.ServiceProvider.GetRequiredService<IRepository<TechnicalSignal>>();
        var candle1mRepository = scope.ServiceProvider.GetRequiredService<IRepository<Candle_1m>>();
        var candle5mRepository = scope.ServiceProvider.GetRequiredService<IRepository<Candle_5m>>();
        var candle1hRepository = scope.ServiceProvider.GetRequiredService<IRepository<Candle_1h>>();
        var candle4hRepository = scope.ServiceProvider.GetRequiredService<IRepository<Candle_4h>>();
        var candle1dRepository = scope.ServiceProvider.GetRequiredService<IRepository<Candle_1d>>();

        try
        {
            _logger.LogInformation("Starting technical signal processing");

            // Get all cryptocurrencies
            var cryptocurrencies = await cryptoRepository.Query().ToListAsync();
            var evaluators = signalEvaluatorFactory.GetAllEvaluators();
            var timeFrames = new[] { "1m", "5m", "1h", "4h", "1d" };

            var allSignals = new List<TechnicalSignal>();

            foreach (var crypto in cryptocurrencies)
            {
                foreach (var timeFrame in timeFrames)
                {
                    var candles = await GetCandlesForTimeFrame(
                        candle1mRepository, candle5mRepository, candle1hRepository,
                        candle4hRepository, candle1dRepository, crypto.Id, timeFrame);

                    if (!candles.Any())
                        continue;

                    foreach (var evaluator in evaluators)
                    {
                        if (!evaluator.SupportedTimeFrames.Contains(timeFrame))
                            continue;

                        try
                        {
                            var signals = await evaluator.EvaluateAsync(
                                crypto.Symbol,
                                timeFrame,
                                candles,
                                cancellationToken);

                            allSignals.AddRange(signals);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex,
                                "Error evaluating {Evaluator} for {Symbol} on {TimeFrame}",
                                evaluator.IndicatorName, crypto.Symbol, timeFrame);
                        }
                    }
                }
            }

            // Clean old signals (keep only last 24 hours)
            await CleanOldSignalsAsync(signalRepository);

            // Save new signals
            if (allSignals.Any())
            {
                await signalRepository.AddRangeAsync(allSignals);
                await unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Processed {Count} technical signals", allSignals.Count);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ProcessSignalsAsync");
        }
    }

    private async Task<IEnumerable<CandleBase>> GetCandlesForTimeFrame(
        IRepository<Candle_1m> candle1mRepo,
        IRepository<Candle_5m> candle5mRepo,
        IRepository<Candle_1h> candle1hRepo,
        IRepository<Candle_4h> candle4hRepo,
        IRepository<Candle_1d> candle1dRepo,
        int cryptoId,
        string timeFrame)
    {
        var limit = 500; // Get last 500 candles for analysis

        return timeFrame switch
        {
            "1m" => await candle1mRepo.Query()
                .Where(c => c.CryptocurrencyId == cryptoId)
                .OrderByDescending(c => c.OpenTime)
                .Take(limit)
                .ToListAsync(),
            "5m" => await candle5mRepo.Query()
                .Where(c => c.CryptocurrencyId == cryptoId)
                .OrderByDescending(c => c.OpenTime)
                .Take(limit)
                .ToListAsync(),
            "1h" => await candle1hRepo.Query()
                .Where(c => c.CryptocurrencyId == cryptoId)
                .OrderByDescending(c => c.OpenTime)
                .Take(limit)
                .ToListAsync(),
            "4h" => await candle4hRepo.Query()
                .Where(c => c.CryptocurrencyId == cryptoId)
                .OrderByDescending(c => c.OpenTime)
                .Take(limit)
                .ToListAsync(),
            "1d" => await candle1dRepo.Query()
                .Where(c => c.CryptocurrencyId == cryptoId)
                .OrderByDescending(c => c.OpenTime)
                .Take(limit)
                .ToListAsync(),
            _ => Enumerable.Empty<CandleBase>()
        };
    }

    private async Task CleanOldSignalsAsync(IRepository<TechnicalSignal> signalRepository)
    {
        var cutoffTime = DateTime.UtcNow.AddHours(-24);
        var oldSignals = await signalRepository.Query()
            .Where(s => s.CreatedAt < cutoffTime)
            .ToListAsync();

        if (oldSignals.Any())
        {
            signalRepository.RemoveRange(oldSignals);
            _logger.LogInformation("Cleaned {Count} old signals", oldSignals.Count());
        }
    }
}