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
public class BollingerSignalService(ApplicationDbContext db, ISignalLoggingService logger) : IBollingerSignalService
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

    private static (decimal Middle, decimal Upper, decimal Lower) ComputeBands(List<CandleBase> candlesAsc, int period, decimal k)
    {
        var closes = candlesAsc.TakeLast(period).Select(x => x.Close).ToList();
        var middle = closes.Average();
        var mean = (double)middle;
        var variance = closes.Select(c => Math.Pow((double)(c - middle), 2)).Average();
        var std = (decimal)Math.Sqrt(variance);
        var upper = middle + k * std;
        var lower = middle - k * std;
        return (middle, upper, lower);
    }

    private static (decimal Middle, decimal Upper, decimal Lower) ComputePrevBands(List<CandleBase> candlesAsc, int period, decimal k)
    {
        var closes = candlesAsc.SkipLast(1).TakeLast(period).Select(x => x.Close).ToList();
        var middle = closes.Average();
        var variance = closes.Select(c => Math.Pow((double)(c - middle), 2)).Average();
        var std = (decimal)Math.Sqrt(variance);
        var upper = middle + k * std;
        var lower = middle - k * std;
        return (middle, upper, lower);
    }

    private async Task<List<BreakoutResult>> DetectPositionAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod, bool above)
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
                var lookback = Math.Max(lookbackPeriod > 0 ? lookbackPeriod : 120, 60);
                var c = await FetchCandlesAsync(tf, cryptoId, lookback);
                if (c.Count < 30) continue;
                var atr = ComputeAtr(c, 14);
                var last = c[^1];
                var prev = c[^2];
                var avgVol = c.TakeLast(20).Average(x => x.Volume);
                var volRatio = avgVol == 0 ? 0 : (last.Volume / avgVol);
                var tol = Math.Max(last.Close * 0.0025m, atr * 0.25m);
                var bands = ComputeBands(c, 20, 2m);
                var up = above && last.Close >= bands.Upper + Math.Max(tol, atr * 0.1m);
                var down = !above && last.Close <= bands.Lower - Math.Max(tol, atr * 0.1m);
                if (up || down)
                {
                    results.Add(new BreakoutResult
                    {
                        Symbol = symbol,
                        Timeframe = tf,
                        IsBreakout = true,
                        Direction = up ? BreakoutDirection.Up : BreakoutDirection.Down,
                        BreakoutLevel = up ? bands.Upper : bands.Lower,
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
                        category: "Bollinger",
                        symbol: symbol,
                        cryptocurrencyId: cryptoId,
                        timeframe: tf,
                        signalName: up ? "PriceAboveBollingerUpper" : "PriceBelowBollingerLower",
                        direction: up ? 1 : -1,
                        breakoutLevel: up ? bands.Upper : bands.Lower,
                        nearestResistance: up ? bands.Upper : bands.Middle,
                        nearestSupport: up ? bands.Middle : bands.Lower,
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

    private async Task<List<BreakoutResult>> DetectBreakoutAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod, bool upper)
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
                var lookback = Math.Max(lookbackPeriod > 0 ? lookbackPeriod : 120, 60);
                var c = await FetchCandlesAsync(tf, cryptoId, lookback);
                if (c.Count < 30) continue;
                var atr = ComputeAtr(c, 14);
                var last = c[^1];
                var prev = c[^2];
                var avgVol = c.TakeLast(20).Average(x => x.Volume);
                var volRatio = avgVol == 0 ? 0 : (last.Volume / avgVol);
                var tol = Math.Max(last.Close * 0.0025m, atr * 0.25m);
                var bands = ComputeBands(c, 20, 2m);
                var prevBands = ComputePrevBands(c, 20, 2m);
                var body = Math.Abs(last.Close - last.Open);
                var wickTopOk = (last.High - last.Close) <= (atr * 0.6m);
                var wickBottomOk = (last.Open - last.Low) <= (atr * 0.6m);
                var crossedUp = prev.Close <= prevBands.Upper && last.Close >= bands.Upper + Math.Max(tol, atr * 0.1m);
                var crossedDown = prev.Close >= prevBands.Lower && last.Close <= bands.Lower - Math.Max(tol, atr * 0.1m);
                var strongBody = body >= (atr * 0.5m);
                var volOk = volRatio >= 1m;
                var pass = (upper && crossedUp && strongBody && volOk && wickTopOk) || (!upper && crossedDown && strongBody && volOk && wickBottomOk);
                if (pass)
                {
                    results.Add(new BreakoutResult
                    {
                        Symbol = symbol,
                        Timeframe = tf,
                        IsBreakout = true,
                        Direction = upper ? BreakoutDirection.Up : BreakoutDirection.Down,
                        BreakoutLevel = upper ? bands.Upper : bands.Lower,
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
                        category: "Bollinger",
                        symbol: symbol,
                        cryptocurrencyId: cryptoId,
                        timeframe: tf,
                        signalName: upper ? "BollingerUpperBreakout" : "BollingerLowerBreakout",
                        direction: upper ? 1 : -1,
                        breakoutLevel: upper ? bands.Upper : bands.Lower,
                        nearestResistance: upper ? bands.Upper : bands.Middle,
                        nearestSupport: upper ? bands.Middle : bands.Lower,
                        pivotR1: r1, pivotR2: r2, pivotR3: r3,
                        pivotS1: s1, pivotS2: s2, pivotS3: s3,
                        atr: atr,
                        tolerance: tol,
                        volumeRatio: volRatio,
                        bodySize: body,
                        lastCandle: last,
                        snapshotCandles: c);
                }
            }
        }
        return results;
    }

    private async Task<List<BreakoutResult>> DetectSqueezeAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod)
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
                if (c.Count < 40) continue;
                var atr = ComputeAtr(c, 14);
                var last = c[^1];
                var avgVol = c.TakeLast(20).Average(x => x.Volume);
                var bands = ComputeBands(c, 20, 2m);
                var width = bands.Upper - bands.Lower;
                var ratio = bands.Middle == 0 ? 0 : width / bands.Middle;
                var bodyAvg = c.TakeLast(10).Average(x => Math.Abs(x.Close - x.Open));
                var volLow = avgVol == 0 ? true : (last.Volume / avgVol) <= 1m;
                var cond = ratio <= 0.02m && bodyAvg <= atr * 0.3m && volLow;
                if (cond)
                {
                    results.Add(new BreakoutResult
                    {
                        Symbol = symbol,
                        Timeframe = tf,
                        IsBreakout = true,
                        Direction = BreakoutDirection.Up,
                        BreakoutLevel = bands.Middle,
                        CandleTime = last.CloseTime,
                        VolumeRatio = avgVol == 0 ? 0 : (last.Volume / avgVol)
                    });
                    var prev = c[^2];
                    var p = (prev.High + prev.Low + prev.Close) / 3m;
                    var r1 = 2m * p - prev.Low;
                    var s1 = 2m * p - prev.High;
                    var r2 = p + (prev.High - prev.Low);
                    var s2 = p - (prev.High - prev.Low);
                    var r3 = prev.High + 2m * (p - prev.Low);
                    var s3 = prev.Low - 2m * (prev.High - p);
                    await _logger.LogSignalAsync(
                        category: "Bollinger",
                        symbol: symbol,
                        cryptocurrencyId: cryptoId,
                        timeframe: tf,
                        signalName: "BollingerSqueeze",
                        direction: 0,
                        breakoutLevel: bands.Middle,
                        nearestResistance: bands.Upper,
                        nearestSupport: bands.Lower,
                        pivotR1: r1, pivotR2: r2, pivotR3: r3,
                        pivotS1: s1, pivotS2: s2, pivotS3: s3,
                        atr: atr,
                        tolerance: Math.Max(last.Close * 0.0025m, atr * 0.25m),
                        volumeRatio: avgVol == 0 ? 0 : (last.Volume / avgVol),
                        bodySize: Math.Abs(last.Close - last.Open),
                        lastCandle: last,
                        snapshotCandles: c);
                }
            }
        }
        return results;
    }

    public Task<List<BreakoutResult>> DetectPriceAboveUpperBandAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod)
        => DetectPositionAsync(symbols, timeframes, lookbackPeriod, true);

    public Task<List<BreakoutResult>> DetectPriceBelowLowerBandAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod)
        => DetectPositionAsync(symbols, timeframes, lookbackPeriod, false);

    public Task<List<BreakoutResult>> DetectUpperBandBreakoutAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod)
        => DetectBreakoutAsync(symbols, timeframes, lookbackPeriod, true);

    public Task<List<BreakoutResult>> DetectLowerBandBreakoutAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod)
        => DetectBreakoutAsync(symbols, timeframes, lookbackPeriod, false);

    public Task<List<BreakoutResult>> DetectBandSqueezeAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod)
        => DetectSqueezeAsync(symbols, timeframes, lookbackPeriod);
}