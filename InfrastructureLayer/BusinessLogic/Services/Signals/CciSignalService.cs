using System;
using System.Linq;
using System.Collections.Generic;
using ApplicationLayer.Dto.Signals;
using ApplicationLayer.Interfaces.Services.Signals;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using InfrastructureLayer.Context;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureLayer.BusinessLogic.Services.Signals;

[InjectAsScoped]
public class CciSignalService(ApplicationDbContext db, ISignalLoggingService logger) : ICciSignalService
{
    private readonly ApplicationDbContext _db = db;
    private readonly ISignalLoggingService _logger = logger;

    private async Task<List<CandleBase>> FetchCandlesAsync(string timeframe, int cryptoId, int lookback)
    {
        switch (timeframe)
        {
            case "1m":
            {
                var list = await _db.Set<Candle_1m>()
                    .Where(c => c.CryptocurrencyId == cryptoId)
                    .OrderByDescending(c => c.OpenTime)
                    .Take(lookback)
                    .ToListAsync();
                return list.OrderBy(c => c.OpenTime).Cast<CandleBase>().ToList();
            }
            case "5m":
            {
                var list = await _db.Set<Candle_5m>()
                    .Where(c => c.CryptocurrencyId == cryptoId)
                    .OrderByDescending(c => c.OpenTime)
                    .Take(lookback)
                    .ToListAsync();
                return list.OrderBy(c => c.OpenTime).Cast<CandleBase>().ToList();
            }
            case "1h":
            {
                var list = await _db.Set<Candle_1h>()
                    .Where(c => c.CryptocurrencyId == cryptoId)
                    .OrderByDescending(c => c.OpenTime)
                    .Take(lookback)
                    .ToListAsync();
                return list.OrderBy(c => c.OpenTime).Cast<CandleBase>().ToList();
            }
            case "4h":
            {
                var list = await _db.Set<Candle_4h>()
                    .Where(c => c.CryptocurrencyId == cryptoId)
                    .OrderByDescending(c => c.OpenTime)
                    .Take(lookback)
                    .ToListAsync();
                return list.OrderBy(c => c.OpenTime).Cast<CandleBase>().ToList();
            }
            case "1d":
            {
                var list = await _db.Set<Candle_1d>()
                    .Where(c => c.CryptocurrencyId == cryptoId)
                    .OrderByDescending(c => c.OpenTime)
                    .Take(lookback)
                    .ToListAsync();
                return list.OrderBy(c => c.OpenTime).Cast<CandleBase>().ToList();
            }
            default:
            {
                var list = await _db.Set<Candle_1h>()
                    .Where(c => c.CryptocurrencyId == cryptoId)
                    .OrderByDescending(c => c.OpenTime)
                    .Take(lookback)
                    .ToListAsync();
                return list.OrderBy(c => c.OpenTime).Cast<CandleBase>().ToList();
            }
        }
    }

    private static List<decimal> ComputeCciSeries(List<CandleBase> c, int period)
    {
        var ccis = new List<decimal>();
        if (c.Count < period) return ccis;
        var tps = c.Select(x => (x.High + x.Low + x.Close) / 3m).ToList();
        for (var i = period - 1; i < tps.Count; i++)
        {
            var window = tps.Skip(i - (period - 1)).Take(period).ToList();
            var sma = window.Average();
            var md = window.Select(x => Math.Abs(x - sma)).Average();
            var denom = md == 0m ? 1m : 0.015m * md;
            var cci = (window[^1] - sma) / denom;
            ccis.Add(cci);
        }
        return ccis;
    }

    private static decimal ComputeAtr(List<CandleBase> candlesAsc, int period)
    {
        if (candlesAsc.Count < period + 1) return 0m;
        var trs = new List<decimal>();
        for (var i = 1; i < candlesAsc.Count; i++)
        {
            var h = candlesAsc[i].High;
            var l = candlesAsc[i].Low;
            var pc = candlesAsc[i - 1].Close;
            var tr = Math.Max(h - l, Math.Max(Math.Abs(h - pc), Math.Abs(l - pc)));
            trs.Add(tr);
        }
        return trs.TakeLast(period).Average();
    }

    private async Task<List<BreakoutResult>> DetectLevelAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod, int period, bool above100)
    {
        var results = new List<BreakoutResult>();
        var tfList = (timeframes != null && timeframes.Count > 0) ? timeframes : ["1m", "5m", "1h", "4h", "1d"];
        var symbolList = (symbols != null && symbols.Count > 0) ? symbols : await _db.Set<Cryptocurrency>().Select(c => c.Symbol).ToListAsync();
        foreach (var symbol in symbolList)
        {
            var cryptoId = await _db.Set<Cryptocurrency>().Where(c => c.Symbol == symbol).Select(c => c.Id).FirstOrDefaultAsync();
            if (cryptoId == 0) continue;
            foreach (var tf in tfList)
            {
                var lookback = Math.Max(lookbackPeriod > 0 ? lookbackPeriod : 150, 80);
                var c = await FetchCandlesAsync(tf, cryptoId, lookback);
                if (c.Count < period + 2) continue;
                var atr = ComputeAtr(c, 14);
                var ccis = ComputeCciSeries(c, period);
                if (ccis.Count < 2) continue;
                var last = c[^1];
                var prev = c[^2];
                var cciLast = ccis[^1];
                var avgVol = c.TakeLast(20).Average(x => x.Volume);
                var volRatio = avgVol == 0 ? 0 : (last.Volume / avgVol);
                var tol = Math.Max(last.Close * 0.0025m, atr * 0.25m);
                var up = above100 && cciLast >= 100m;
                var down = !above100 && cciLast <= -100m;
                if (up || down)
                {
                    results.Add(new BreakoutResult
                    {
                        Symbol = symbol,
                        Timeframe = tf,
                        IsBreakout = true,
                        Direction = up ? BreakoutDirection.Up : BreakoutDirection.Down,
                        BreakoutLevel = cciLast,
                        CandleTime = last.CloseTime,
                        VolumeRatio = volRatio
                    });
                    var p = (prev.High + prev.Low + prev.Close) / 3m;
                    var r1 = 2m * p - prev.Low;
                    var s1 = 2m * p - prev.High;
                    var r2 = p + (prev.High - prev.Low);
                    var s2 = p - (prev.High - prev.Low);
                    var r3 = prev.High + 2m * (p - prev.Low);
                    var s3 = prev.Low - 2m * (prev.High - p);
                    await _logger.LogSignalAsync(
                        category: "CCI",
                        symbol: symbol,
                        cryptocurrencyId: cryptoId,
                        timeframe: tf,
                        signalName: up ? "CCIAbove100" : "CCIBelowMinus100",
                        direction: up ? 1 : -1,
                        breakoutLevel: cciLast,
                        nearestResistance: 100m,
                        nearestSupport: -100m,
                        pivotR1: r1, pivotR2: r2, pivotR3: r3,
                        pivotS1: s1, pivotS2: s2, pivotS3: s3,
                        atr: atr,
                        tolerance: tol,
                        volumeRatio: volRatio,
                        bodySize: Math.Abs(last.Close - last.Open),
                        lastCandle: last,
                        snapshotCandles: c);
                }
            }
        }
        return results;
    }

    private async Task<List<BreakoutResult>> DetectZeroCrossAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod, int period, bool up)
    {
        var results = new List<BreakoutResult>();
        var tfList = (timeframes != null && timeframes.Count > 0) ? timeframes : ["1m", "5m", "1h", "4h", "1d"];
        var symbolList = (symbols != null && symbols.Count > 0) ? symbols : await _db.Set<Cryptocurrency>().Select(c => c.Symbol).ToListAsync();
        foreach (var symbol in symbolList)
        {
            var cryptoId = await _db.Set<Cryptocurrency>().Where(c => c.Symbol == symbol).Select(c => c.Id).FirstOrDefaultAsync();
            if (cryptoId == 0) continue;
            foreach (var tf in tfList)
            {
                var lookback = Math.Max(lookbackPeriod > 0 ? lookbackPeriod : 150, 80);
                var c = await FetchCandlesAsync(tf, cryptoId, lookback);
                if (c.Count < period + 2) continue;
                var atr = ComputeAtr(c, 14);
                var ccis = ComputeCciSeries(c, period);
                if (ccis.Count < 2) continue;
                var last = c[^1];
                var prev = c[^2];
                var cciLast = ccis[^1];
                var cciPrev = ccis[^2];
                var avgVol = c.TakeLast(20).Average(x => x.Volume);
                var volRatio = avgVol == 0 ? 0 : (last.Volume / avgVol);
                var tol = Math.Max(last.Close * 0.0025m, atr * 0.25m);
                var crossedUp = cciPrev <= 0m && cciLast > 0m;
                var crossedDown = cciPrev >= 0m && cciLast < 0m;
                var pass = (up && crossedUp) || (!up && crossedDown);
                if (pass)
                {
                    results.Add(new BreakoutResult
                    {
                        Symbol = symbol,
                        Timeframe = tf,
                        IsBreakout = true,
                        Direction = up ? BreakoutDirection.Up : BreakoutDirection.Down,
                        BreakoutLevel = cciLast,
                        CandleTime = last.CloseTime,
                        VolumeRatio = volRatio
                    });
                    var p = (prev.High + prev.Low + prev.Close) / 3m;
                    var r1 = 2m * p - prev.Low;
                    var s1 = 2m * p - prev.High;
                    var r2 = p + (prev.High - prev.Low);
                    var s2 = p - (prev.High - prev.Low);
                    var r3 = prev.High + 2m * (p - prev.Low);
                    var s3 = prev.Low - 2m * (prev.High - p);
                    await _logger.LogSignalAsync(
                        category: "CCI",
                        symbol: symbol,
                        cryptocurrencyId: cryptoId,
                        timeframe: tf,
                        signalName: up ? "CCICrossAboveZero" : "CCICrossBelowZero",
                        direction: up ? 1 : -1,
                        breakoutLevel: cciLast,
                        nearestResistance: 100m,
                        nearestSupport: -100m,
                        pivotR1: r1, pivotR2: r2, pivotR3: r3,
                        pivotS1: s1, pivotS2: s2, pivotS3: s3,
                        atr: atr,
                        tolerance: tol,
                        volumeRatio: volRatio,
                        bodySize: Math.Abs(last.Close - last.Open),
                        lastCandle: last,
                        snapshotCandles: c);
                }
            }
        }
        return results;
    }

    public Task<List<BreakoutResult>> DetectCciAbove100Async(List<string> symbols, List<string> timeframes, int lookbackPeriod, int period)
        => DetectLevelAsync(symbols, timeframes, lookbackPeriod, period, true);

    public Task<List<BreakoutResult>> DetectCciBelowMinus100Async(List<string> symbols, List<string> timeframes, int lookbackPeriod, int period)
        => DetectLevelAsync(symbols, timeframes, lookbackPeriod, period, false);

    public Task<List<BreakoutResult>> DetectCciCrossAboveZeroAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod, int period)
        => DetectZeroCrossAsync(symbols, timeframes, lookbackPeriod, period, true);

    public Task<List<BreakoutResult>> DetectCciCrossBelowZeroAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod, int period)
        => DetectZeroCrossAsync(symbols, timeframes, lookbackPeriod, period, false);
}