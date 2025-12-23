using ApplicationLayer.Interfaces;
using ApplicationLayer.Interfaces.Services.Candles;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InfrastructureLayer.BusinessLogic.Services.Candles;

[InjectAsScoped]
public class CandleAggregatorService(
    IUnitOfWork _unitOfWork,
    IRepository<Cryptocurrency> _cryptoRepo,
    IRepository<AggregationState> _stateRepo,
    IRepository<Candle_1m> _candle1mRepo,
    IRepository<Candle_5m> _candle5mRepo,
    IRepository<Candle_15m> _candle15mRepo,
    IRepository<Candle_1h> _candle1hRepo,
    IRepository<Candle_4h> _candle4hRepo,
    IRepository<Candle_1d> _candle1dRepo,
    IRepository<Candle_1w> _candle1wRepo,
    ILogger<CandleAggregatorService> _logger
) : ICandleAggregatorService
{
    public async Task ExecuteAggregationAsync(CancellationToken cancellationToken)
    {
        var cryptos = await _cryptoRepo.Query().ToListAsync(cancellationToken);

        foreach (var crypto in cryptos)
        {
            if (cancellationToken.IsCancellationRequested) break;

            await AggregateForCryptoAsync(crypto, cancellationToken);
        }
    }

    private async Task AggregateForCryptoAsync(Cryptocurrency crypto, CancellationToken ct)
    {
        // 5m
        await AggregateTimeframeAsync<Candle_5m>(crypto, _candle5mRepo, "5m", TimeSpan.FromMinutes(5), ct);
        // 15m
        await AggregateTimeframeAsync<Candle_15m>(crypto, _candle15mRepo, "15m", TimeSpan.FromMinutes(15), ct);
        // 1h
        await AggregateTimeframeAsync<Candle_1h>(crypto, _candle1hRepo, "1h", TimeSpan.FromHours(1), ct);
        // 4h
        await AggregateTimeframeAsync<Candle_4h>(crypto, _candle4hRepo, "4h", TimeSpan.FromHours(4), ct);
        // 1d
        await AggregateTimeframeAsync<Candle_1d>(crypto, _candle1dRepo, "1d", TimeSpan.FromDays(1), ct);
        // 1w
        await AggregateTimeframeAsync<Candle_1w>(crypto, _candle1wRepo, "1w", TimeSpan.FromDays(7), ct);
    }

    private async Task AggregateTimeframeAsync<TTarget>(
        Cryptocurrency crypto, 
        IRepository<TTarget> targetRepo,
        string timeframeKey,
        TimeSpan timeframe, 
        CancellationToken ct) 
        where TTarget : CandleBase, new()
    {
        try 
        {
            // 1. Get last state from Watermark
            var state = await _stateRepo.Query()
                .FirstOrDefaultAsync(s => s.CryptocurrencyId == crypto.Id && s.Timeframe == timeframeKey, ct);
            
            DateTime startTime;
            
            if (state != null)
            {
                 // Continue from next interval
                startTime = state.LastProcessedOpenTime.Add(timeframe);
            }
            else
            {
                // Fallback: Check DB if migration happened or just start from scratch
                // For safety, we can check the target table too, but Watermark is the source of truth.
                // If Watermark is missing, we assume we need to start from the beginning.
                
                var first1m = await _candle1mRepo.Query()
                    .Where(c => c.CryptocurrencyId == crypto.Id)
                    .OrderBy(c => c.OpenTime)
                    .FirstOrDefaultAsync(ct);
                
                if (first1m == null) return; // No data to aggregate
                
                startTime = AlignToTimeframe(first1m.OpenTime, timeframe);
                
                // Initialize state
                state = new AggregationState
                {
                    CryptocurrencyId = crypto.Id,
                    Timeframe = timeframeKey,
                    LastProcessedOpenTime = startTime.Add(timeframe.Negate()), // Point to "previous" fictitious candle
                    LastUpdatedUtc = DateTime.UtcNow
                };
                await _stateRepo.AddAsync(state);
                await _unitOfWork.SaveChangesAsync(ct);
            }

            var now = DateTime.UtcNow;
            
            // Iterate through expected intervals until now
            // We only process CLOSED candles.
            // A candle starting at T with duration D closes at T+D.
            // We can only aggregate if Now > T+D (Strictly closed).
            
            var nextSlot = startTime;
            
            var newCandles = new List<TTarget>();
            
            // Limit batch size to avoid memory issues
            int batchCount = 0;
            const int MAX_BATCH = 1000;

            // Ensure candle is strictly closed (now > slotEnd)
            while (nextSlot + timeframe < now)
            {
                var slotEnd = nextSlot + timeframe;
                
                // Query 1m candles in range [nextSlot, slotEnd)
                var sourceCandles = await _candle1mRepo.Query()
                    .Where(c => c.CryptocurrencyId == crypto.Id && c.OpenTime >= nextSlot && c.OpenTime < slotEnd)
                    .OrderBy(c => c.OpenTime)
                    .ToListAsync(ct);
                
                // Gap Detection: Ensure we have enough data to form a valid candle
                // 1 week = 7 * 24 * 60 = 10080 minutes
                double expectedMinutes = timeframe.TotalMinutes;
                int actualCount = sourceCandles.Count;
                
                // Rule: ActualCount >= ExpectedCount * 0.95
                if (actualCount >= expectedMinutes * 0.95)
                {
                    var open = sourceCandles.First().Open;
                    var close = sourceCandles.Last().Close;
                    var high = sourceCandles.Max(c => c.High);
                    var low = sourceCandles.Min(c => c.Low);
                    var volume = sourceCandles.Sum(c => c.Volume);
                    var trades = sourceCandles.Sum(c => c.NumberOfTrades);
                    
                    var candle = new TTarget
                    {
                        CryptocurrencyId = crypto.Id,
                        OpenTime = nextSlot,
                        CloseTime = slotEnd,
                        Open = open,
                        Close = close,
                        High = high,
                        Low = low,
                        Volume = volume,
                        NumberOfTrades = trades,
                        IsFinal = true,
                        LastUpdatedUtc = DateTime.UtcNow
                    };
                    
                    newCandles.Add(candle);
                    batchCount++;
                    
                    // Update state in memory (will be saved with batch)
                    state.LastProcessedOpenTime = nextSlot;
                    state.LastUpdatedUtc = DateTime.UtcNow;
                }
                // Else: Skip this interval (Gap), but advance nextSlot
                
                nextSlot = nextSlot.Add(timeframe);

                if (batchCount >= MAX_BATCH)
                {
                    await targetRepo.AddRangeAsync(newCandles);
                    // Update state persistence
                    _stateRepo.Update(state); 
                    await _unitOfWork.SaveChangesAsync(ct);
                    
                    newCandles.Clear();
                    batchCount = 0;
                }
            }
            
            if (newCandles.Any())
            {
                await targetRepo.AddRangeAsync(newCandles);
                _stateRepo.Update(state);
                await _unitOfWork.SaveChangesAsync(ct);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error aggregating {crypto.Symbol} {timeframe}");
        }
    }

    private DateTime AlignToTimeframe(DateTime time, TimeSpan timeframe)
    {
        // Special case for 1w: Start Monday 00:00 UTC (ISO 8601 Week)
        // DayOfWeek.Monday is 1. Sunday is 0.
        // If time is Sunday (0), we want previous Monday (-6 days).
        // If time is Monday (1), we want same day (0 days).
        // If time is Tuesday (2), we want previous Monday (-1 day).
        if (timeframe.TotalDays == 7)
        {
            var diff = time.DayOfWeek - DayOfWeek.Monday;
            if (diff < 0) diff += 7;
            return time.AddDays(-1 * diff).Date;
        }
        
        if (timeframe.TotalDays == 1) return time.Date;
        
        var ticks = time.Ticks;
        var timeframeTicks = timeframe.Ticks;
        var alignedTicks = ticks - (ticks % timeframeTicks);
        return new DateTime(alignedTicks, time.Kind);
    }
}
