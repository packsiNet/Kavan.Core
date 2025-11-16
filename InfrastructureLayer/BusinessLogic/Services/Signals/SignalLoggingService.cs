using ApplicationLayer.Interfaces.Services.Signals;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using InfrastructureLayer.Context;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace InfrastructureLayer.BusinessLogic.Services.Signals;

[InjectAsScoped]
public class SignalLoggingService(ApplicationDbContext db) : ISignalLoggingService
{
    private readonly ApplicationDbContext _db = db;

    private async Task<bool> IsDuplicateAsync(
        string category,
        string symbol,
        string timeframe,
        string signalName,
        int direction,
        decimal breakoutLevel,
        decimal tolerance,
        DateTime candidateCloseTime)
    {
        var last = await _db.Set<Signal>()
            .Where(x => x.Symbol == symbol
                        && x.Timeframe == timeframe
                        && x.SignalCategory == category
                        && x.SignalName == signalName
                        && x.Direction == direction)
            .OrderByDescending(x => x.SignalTime)
            .FirstOrDefaultAsync();

        if (last == null)
        {
            Log.Debug("No previous signal for {Symbol}-{Timeframe}-{Category}-{Name}-{Dir}", symbol, timeframe, category, signalName, direction);
            return false;
        }

        var nearSameLevel = Math.Abs(last.BreakoutLevel - breakoutLevel) <= tolerance;
        if (!nearSameLevel)
        {
            Log.Debug("Level differs beyond tolerance for {Symbol}-{Timeframe}-{Category}-{Name}: last={LastLevel} new={NewLevel} tol={Tol}", symbol, timeframe, category, signalName, last.BreakoutLevel, breakoutLevel, tolerance);
            return false;
        }

        // Check invalidation after last signal: if price moved back across level, then a new breakout can be recorded
        // For Up breakout: invalidated if any candle closes <= level - tolerance
        // For Down breakout: invalidated if any candle closes >= level + tolerance

        var candles = await FetchCandlesRangeAsync(timeframe, last.CryptocurrencyId, last.SignalTime, candidateCloseTime);
        if (candles.Count == 0)
        {
            Log.Debug("Duplicate due to no new candles since last signal for {Symbol}-{Timeframe}-{Category}-{Name}", symbol, timeframe, category, signalName);
            return true;
        }

        if (direction > 0)
        {
            var invalidated = candles.Any(c => c.Close <= breakoutLevel - tolerance);
            if (invalidated)
                Log.Debug("Invalidated up-breakout; new signal allowed for {Symbol}-{Timeframe}-{Category}-{Name}", symbol, timeframe, category, signalName);
            else
                Log.Debug("Continuation without invalidation; duplicate up-breakout for {Symbol}-{Timeframe}-{Category}-{Name}", symbol, timeframe, category, signalName);
            return !invalidated;
        }
        else if (direction < 0)
        {
            var invalidated = candles.Any(c => c.Close >= breakoutLevel + tolerance);
            if (invalidated)
                Log.Debug("Invalidated down-breakout; new signal allowed for {Symbol}-{Timeframe}-{Category}-{Name}", symbol, timeframe, category, signalName);
            else
                Log.Debug("Continuation without invalidation; duplicate down-breakout for {Symbol}-{Timeframe}-{Category}-{Name}", symbol, timeframe, category, signalName);
            return !invalidated;
        }
        var invalidatedNeutral = candles.Any(c => c.Close >= breakoutLevel + tolerance || c.Close <= breakoutLevel - tolerance);
        if (invalidatedNeutral)
            Log.Debug("Neutral invalidated by movement beyond tolerance; new signal allowed for {Symbol}-{Timeframe}-{Category}-{Name}", symbol, timeframe, category, signalName);
        else
            Log.Debug("Neutral continuation within tolerance; duplicate for {Symbol}-{Timeframe}-{Category}-{Name}", symbol, timeframe, category, signalName);
        return !invalidatedNeutral;
    }

    private async Task<List<CandleBase>> FetchCandlesRangeAsync(string timeframe, int cryptoId, DateTime from, DateTime to)
    {
        switch (timeframe)
        {
            case "1m":
                {
                    var list = await _db.Set<Candle_1m>()
                        .Where(c => c.CryptocurrencyId == cryptoId && c.OpenTime > from && c.CloseTime <= to)
                        .OrderBy(c => c.OpenTime)
                        .ToListAsync();
                    return list.Cast<CandleBase>().ToList();
                }
            case "5m":
                {
                    var list = await _db.Set<Candle_5m>()
                        .Where(c => c.CryptocurrencyId == cryptoId && c.OpenTime > from && c.CloseTime <= to)
                        .OrderBy(c => c.OpenTime)
                        .ToListAsync();
                    return list.Cast<CandleBase>().ToList();
                }
            case "1h":
                {
                    var list = await _db.Set<Candle_1h>()
                        .Where(c => c.CryptocurrencyId == cryptoId && c.OpenTime > from && c.CloseTime <= to)
                        .OrderBy(c => c.OpenTime)
                        .ToListAsync();
                    return list.Cast<CandleBase>().ToList();
                }
            case "4h":
                {
                    var list = await _db.Set<Candle_4h>()
                        .Where(c => c.CryptocurrencyId == cryptoId && c.OpenTime > from && c.CloseTime <= to)
                        .OrderBy(c => c.OpenTime)
                        .ToListAsync();
                    return list.Cast<CandleBase>().ToList();
                }
            case "1d":
                {
                    var list = await _db.Set<Candle_1d>()
                        .Where(c => c.CryptocurrencyId == cryptoId && c.OpenTime > from && c.CloseTime <= to)
                        .OrderBy(c => c.OpenTime)
                        .ToListAsync();
                    return list.Cast<CandleBase>().ToList();
                }
            default:
                {
                    var list = await _db.Set<Candle_1h>()
                        .Where(c => c.CryptocurrencyId == cryptoId && c.OpenTime > from && c.CloseTime <= to)
                        .OrderBy(c => c.OpenTime)
                        .ToListAsync();
                    return list.Cast<CandleBase>().ToList();
                }
        }
    }

    public async Task<int> LogBreakoutAsync(
        string symbol,
        int cryptocurrencyId,
        string timeframe,
        string signalName,
        int direction,
        decimal breakoutLevel,
        decimal nearestResistance,
        decimal nearestSupport,
        decimal pivotR1,
        decimal pivotR2,
        decimal pivotR3,
        decimal pivotS1,
        decimal pivotS2,
        decimal pivotS3,
        decimal atr,
        decimal tolerance,
        decimal volumeRatio,
        decimal bodySize,
        CandleBase lastCandle,
        List<CandleBase> snapshotCandles)
    {
        // Deduplication gate: avoid recording the same breakout continuation
        var isDup = await IsDuplicateAsync("Technical", symbol, timeframe, signalName, direction, breakoutLevel, tolerance, lastCandle.CloseTime);
        if (isDup)
        {
            Log.Debug("Skipped duplicate signal {Symbol}-{Timeframe}-{Name}-{Dir} at {Time}", symbol, timeframe, signalName, direction, lastCandle.CloseTime);
            return 0;
        }

        var log = new Signal
        {
            CryptocurrencyId = cryptocurrencyId,
            Symbol = symbol,
            Timeframe = timeframe,
            SignalTime = lastCandle.CloseTime,
            SignalCategory = "Technical",
            SignalName = signalName,
            Direction = direction,
            BreakoutLevel = breakoutLevel,
            NearestResistance = nearestResistance,
            NearestSupport = nearestSupport,
            PivotR1 = pivotR1,
            PivotR2 = pivotR2,
            PivotR3 = pivotR3,
            PivotS1 = pivotS1,
            PivotS2 = pivotS2,
            PivotS3 = pivotS3,
            Atr = atr,
            Tolerance = tolerance,
            VolumeRatio = volumeRatio,
            BodySize = bodySize,
            CandleOpenTime = lastCandle.OpenTime,
            CandleCloseTime = lastCandle.CloseTime,
            CandleOpen = lastCandle.Open,
            CandleHigh = lastCandle.High,
            CandleLow = lastCandle.Low,
            CandleClose = lastCandle.Close,
            CandleVolume = lastCandle.Volume
        };

        _db.Add(log);
        await _db.SaveChangesAsync();

        if (snapshotCandles is { Count: > 0 })
        {
            var ordered = snapshotCandles.OrderBy(c => c.OpenTime).ToList();
            var list = new List<SignalCandle>();
            for (int i = 0; i < ordered.Count; i++)
            {
                var c = ordered[i];
                list.Add(new SignalCandle
                {
                    SignalId = log.Id,
                    Index = i,
                    Timeframe = timeframe,
                    OpenTime = c.OpenTime,
                    CloseTime = c.CloseTime,
                    Open = c.Open,
                    High = c.High,
                    Low = c.Low,
                    Close = c.Close,
                    Volume = c.Volume
                });
            }

            _db.AddRange(list);
            await _db.SaveChangesAsync();
        }

        return log.Id;
    }

    public async Task<int> LogSignalAsync(
        string category,
        string symbol,
        int cryptocurrencyId,
        string timeframe,
        string signalName,
        int direction,
        decimal breakoutLevel,
        decimal nearestResistance,
        decimal nearestSupport,
        decimal pivotR1,
        decimal pivotR2,
        decimal pivotR3,
        decimal pivotS1,
        decimal pivotS2,
        decimal pivotS3,
        decimal atr,
        decimal tolerance,
        decimal volumeRatio,
        decimal bodySize,
        CandleBase lastCandle,
        List<CandleBase> snapshotCandles)
    {
        var isDup = await IsDuplicateAsync(category, symbol, timeframe, signalName, direction, breakoutLevel, tolerance, lastCandle.CloseTime);
        if (isDup)
        {
            Log.Debug("Skipped duplicate signal {Symbol}-{Timeframe}-{Category}-{Name}-{Dir} at {Time}", symbol, timeframe, category, signalName, direction, lastCandle.CloseTime);
            return 0;
        }

        var log = new Signal
        {
            CryptocurrencyId = cryptocurrencyId,
            Symbol = symbol,
            Timeframe = timeframe,
            SignalTime = lastCandle.CloseTime,
            SignalCategory = category,
            SignalName = signalName,
            Direction = direction,
            BreakoutLevel = breakoutLevel,
            NearestResistance = nearestResistance,
            NearestSupport = nearestSupport,
            PivotR1 = pivotR1,
            PivotR2 = pivotR2,
            PivotR3 = pivotR3,
            PivotS1 = pivotS1,
            PivotS2 = pivotS2,
            PivotS3 = pivotS3,
            Atr = atr,
            Tolerance = tolerance,
            VolumeRatio = volumeRatio,
            BodySize = bodySize,
            CandleOpenTime = lastCandle.OpenTime,
            CandleCloseTime = lastCandle.CloseTime,
            CandleOpen = lastCandle.Open,
            CandleHigh = lastCandle.High,
            CandleLow = lastCandle.Low,
            CandleClose = lastCandle.Close,
            CandleVolume = lastCandle.Volume
        };

        _db.Add(log);
        await _db.SaveChangesAsync();

        if (snapshotCandles is { Count: > 0 })
        {
            var ordered = snapshotCandles.OrderBy(c => c.OpenTime).ToList();
            var list = new List<SignalCandle>();
            for (int i = 0; i < ordered.Count; i++)
            {
                var c = ordered[i];
                list.Add(new SignalCandle
                {
                    SignalId = log.Id,
                    Index = i,
                    Timeframe = timeframe,
                    OpenTime = c.OpenTime,
                    CloseTime = c.CloseTime,
                    Open = c.Open,
                    High = c.High,
                    Low = c.Low,
                    Close = c.Close,
                    Volume = c.Volume
                });
            }

            _db.AddRange(list);
            await _db.SaveChangesAsync();
        }

        return log.Id;
    }
}