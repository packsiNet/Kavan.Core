using ApplicationLayer.Dto.Trade;
using ApplicationLayer.Interfaces;
using ApplicationLayer.Interfaces.Services;
using DomainLayer.Common.Enums;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

#nullable enable

namespace InfrastructureLayer.BusinessLogic.Services.Trading;

public class MarketMonitoringService(
    IServiceProvider _serviceProvider,
    ILogger<MarketMonitoringService> _logger
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Market Monitoring Service is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await MonitorTradesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while monitoring trades.");
            }

            // Check every 5 seconds (can be adjusted)
            await Task.Delay(5000, stoppingToken);
        }
    }

    // Simplified CloseTradeDto used internally by monitoring service to hold calculation results
    private class CloseTradeDto
    {
        public decimal ExitPrice { get; set; }
        public ExitReason ExitReason { get; set; }
    }

    private async Task MonitorTradesAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var tradeRepo = scope.ServiceProvider.GetRequiredService<IRepository<Trade>>();
        var tradeService = scope.ServiceProvider.GetRequiredService<ITradeService>();
        var candleRepo = scope.ServiceProvider.GetRequiredService<IRepository<Candle_1m>>();
        var cryptoRepo = scope.ServiceProvider.GetRequiredService<IRepository<Cryptocurrency>>();

        // 1. Get all Open Trades
        var openTrades = await tradeRepo.Query()
            .Where(x => x.Status == TradeStatus.Open)
            .Include(x => x.TakeProfits)
            .ToListAsync(stoppingToken);

        if (!openTrades.Any()) return;

        // 2. Get distinct symbols
        var symbols = openTrades.Select(x => x.Symbol).Distinct().ToList();

        // 3. Get latest candles for these symbols
        // Optimize: Get CryptoIds first
        var cryptos = await cryptoRepo.Query()
            .Where(x => symbols.Contains(x.Symbol))
            .Select(x => new { x.Id, x.Symbol })
            .ToListAsync(stoppingToken);

        var cryptoMap = cryptos.ToDictionary(x => x.Symbol, x => x.Id);

        foreach (var trade in openTrades)
        {
            if (!cryptoMap.TryGetValue(trade.Symbol, out var cryptoId))
            {
                _logger.LogWarning($"Cryptocurrency not found for symbol: {trade.Symbol}");
                continue;
            }

            // Get latest candle
            var candle = await candleRepo.Query()
                .Where(x => x.CryptocurrencyId == cryptoId)
                .OrderByDescending(x => x.OpenTime)
                .FirstOrDefaultAsync(stoppingToken);

            if (candle == null) continue;

            // 4. Check SL/TP
            // Need to check if price hit SL or TP based on High/Low
            
            CloseTradeDto? exitResult = null;

            if (trade.Side == TradeSide.Long)
            {
                // Check SL (Low price hits SL)
                if (candle.Low <= trade.StopLoss)
                {
                    exitResult = new CloseTradeDto
                    {
                        ExitPrice = trade.StopLoss,
                        ExitReason = ExitReason.StopLoss
                    };
                }
                // Check TP (High price hits TP)
                else
                {
                    var hitTp = trade.TakeProfits.Where(x => !x.IsHit && candle.High >= x.Price).OrderBy(x => x.Price).FirstOrDefault();
                    if (hitTp != null)
                    {
                         exitResult = new CloseTradeDto
                         {
                             ExitPrice = hitTp.Price,
                             ExitReason = ExitReason.TakeProfit
                         };
                    }
                }
            }
            else // Short
            {
                // Check SL (High price hits SL)
                if (candle.High >= trade.StopLoss)
                {
                    exitResult = new CloseTradeDto
                    {
                        ExitPrice = trade.StopLoss,
                        ExitReason = ExitReason.StopLoss
                    };
                }
                // Check TP (Low price hits TP)
                else
                {
                    var hitTp = trade.TakeProfits.Where(x => !x.IsHit && candle.Low <= x.Price).OrderByDescending(x => x.Price).FirstOrDefault();
                    if (hitTp != null)
                    {
                        exitResult = new CloseTradeDto
                        {
                            ExitPrice = hitTp.Price,
                            ExitReason = ExitReason.TakeProfit
                        };
                    }
                }
            }

            if (exitResult != null)
                {
                    // Close the trade
                    _logger.LogInformation($"Closing trade {trade.Id} due to {exitResult.ExitReason} at {exitResult.ExitPrice}");
                    await tradeService.CloseTradeInternalAsync(trade.Id, exitResult.ExitPrice, exitResult.ExitReason);
                }
            }
        }
    }
