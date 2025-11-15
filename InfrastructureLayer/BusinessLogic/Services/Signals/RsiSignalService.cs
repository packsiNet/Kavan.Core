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
public class RsiSignalService(ApplicationDbContext db, ISignalLoggingService logger) : IRsiSignalService
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

    private static List<decimal> ComputeRsiSeries(List<CandleBase> c, int period)
    {
        var rsis = new List<decimal>();
        if (c.Count < period + 1) return rsis;
        var gains = new List<decimal>();
        var losses = new List<decimal>();
        for (var i = 1; i < c.Count; i++)
        {
            var ch = c[i].Close - c[i - 1].Close;
            gains.Add(Math.Max(ch, 0m));
            losses.Add(Math.Max(-ch, 0m));
        }
        var avgGain = gains.Take(period).Average();
        var avgLoss = losses.Take(period).Average();
        var rs = avgLoss == 0 ? 0 : (double)(avgGain / avgLoss);
        var rsi = avgLoss == 0 ? 100m : (decimal)(100.0 - (100.0 / (1.0 + rs)));
        rsis.Add(rsi);
        for (var i = period; i < gains.Count; i++)
        {
            avgGain = ((avgGain * (period - 1)) + gains[i]) / period;
            avgLoss = ((avgLoss * (period - 1)) + losses[i]) / period;
            rs = avgLoss == 0 ? 0 : (double)(avgGain / avgLoss);
            rsi = avgLoss == 0 ? 100m : (decimal)(100.0 - (100.0 / (1.0 + rs)));
            rsis.Add(rsi);
        }
        return rsis;
    }

    private static (int idx1, int idx2) FindLastTwoLows(List<decimal> series)
    {
        var lows = new List<int>();
        for (var i = 1; i < series.Count - 1; i++)
        {
            if (series[i] <= series[i - 1] && series[i] <= series[i + 1]) lows.Add(i);
        }
        if (lows.Count < 2) return (-1, -1);
        return (lows[^2], lows[^1]);
    }

    private static (int idx1, int idx2) FindLastTwoHighs(List<decimal> series)
    {
        var highs = new List<int>();
        for (var i = 1; i < series.Count - 1; i++)
        {
            if (series[i] >= series[i - 1] && series[i] >= series[i + 1]) highs.Add(i);
        }
        if (highs.Count < 2) return (-1, -1);
        return (highs[^2], highs[^1]);
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

    private async Task<List<BreakoutResult>> DetectOverAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod, int period, bool overbought)
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
                var atr = ComputeAtr(c, 14);
                if (c.Count < period + 2) continue;
                var rsis = ComputeRsiSeries(c, period);
                if (rsis.Count < 2) continue;
                var last = c[^1];
                var prev = c[^2];
                var rsiLast = rsis[^1];
                var rsiPrev = rsis[^2];
                var avgVol = c.TakeLast(20).Average(x => x.Volume);
                var volRatio = avgVol == 0 ? 0 : (last.Volume / avgVol);
                var tol = Math.Max(last.Close * 0.0025m, atr * 0.25m);
                var overUp = overbought && rsiLast >= 70m;
                var overDown = !overbought && rsiLast <= 30m;
                if (overUp || overDown)
                {
                    results.Add(new BreakoutResult
                    {
                        Symbol = symbol,
                        Timeframe = tf,
                        IsBreakout = true,
                        Direction = overUp ? BreakoutDirection.Up : BreakoutDirection.Down,
                        BreakoutLevel = rsiLast,
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
                        category: "RSI",
                        symbol: symbol,
                        cryptocurrencyId: cryptoId,
                        timeframe: tf,
                        signalName: overUp ? "RSIOverbought70" : "RSIOversold30",
                        direction: overUp ? 1 : -1,
                        breakoutLevel: rsiLast,
                        nearestResistance: overUp ? rsiLast : 50m,
                        nearestSupport: overUp ? 50m : rsiLast,
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

    private async Task<List<BreakoutResult>> DetectCross50Async(List<string> symbols, List<string> timeframes, int lookbackPeriod, int period, bool up)
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
                var atr = ComputeAtr(c, 14);
                if (c.Count < period + 2) continue;
                var rsis = ComputeRsiSeries(c, period);
                if (rsis.Count < 2) continue;
                var last = c[^1];
                var prev = c[^2];
                var rsiLast = rsis[^1];
                var rsiPrev = rsis[^2];
                var avgVol = c.TakeLast(20).Average(x => x.Volume);
                var volRatio = avgVol == 0 ? 0 : (last.Volume / avgVol);
                var tol = Math.Max(last.Close * 0.0025m, atr * 0.25m);
                var crossedUp = rsiPrev <= 50m && rsiLast > 50m;
                var crossedDown = rsiPrev >= 50m && rsiLast < 50m;
                var pass = (up && crossedUp) || (!up && crossedDown);
                if (pass)
                {
                    results.Add(new BreakoutResult
                    {
                        Symbol = symbol,
                        Timeframe = tf,
                        IsBreakout = true,
                        Direction = up ? BreakoutDirection.Up : BreakoutDirection.Down,
                        BreakoutLevel = rsiLast,
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
                        category: "RSI",
                        symbol: symbol,
                        cryptocurrencyId: cryptoId,
                        timeframe: tf,
                        signalName: up ? "RSICrossAbove50" : "RSICrossBelow50",
                        direction: up ? 1 : -1,
                        breakoutLevel: rsiLast,
                        nearestResistance: up ? rsiLast : 50m,
                        nearestSupport: up ? 50m : rsiLast,
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

    private async Task<List<BreakoutResult>> DetectDivergenceAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod, int period, bool bullish)
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
                if (c.Count < period + 30) continue;
                var rsis = ComputeRsiSeries(c, period);
                if (rsis.Count < 10) continue;
                var atr = ComputeAtr(c, 14);
                var last = c[^1];
                var prev = c[^2];
                var avgVol = c.TakeLast(20).Average(x => x.Volume);
                var volRatio = avgVol == 0 ? 0 : (last.Volume / avgVol);
                var tol = Math.Max(last.Close * 0.0025m, atr * 0.25m);

                var priceSeries = c.Select(x => x.Close).ToList();
                var rsiSeries = rsis;
                var offset = priceSeries.Count - rsiSeries.Count;
                if (offset < 3) offset = 3;

                if (bullish)
                {
                    var lowsP = new List<int>();
                    for (var i = offset + 1; i < priceSeries.Count - 1; i++)
                    {
                        if (priceSeries[i] <= priceSeries[i - 1] && priceSeries[i] <= priceSeries[i + 1]) lowsP.Add(i);
                    }
                    if (lowsP.Count >= 2)
                    {
                        var i1 = lowsP[^2];
                        var i2 = lowsP[^1];
                        var j1 = i1 - offset - 1;
                        var j2 = i2 - offset - 1;
                        if (j1 >= 0 && j2 >= 0 && j2 < rsiSeries.Count)
                        {
                            var priceLowerLow = priceSeries[i2] < priceSeries[i1];
                            var rsiHigherLow = rsiSeries[j2] > rsiSeries[j1];
                            if (priceLowerLow && rsiHigherLow)
                            {
                                results.Add(new BreakoutResult
                                {
                                    Symbol = symbol,
                                    Timeframe = tf,
                                    IsBreakout = true,
                                    Direction = BreakoutDirection.Up,
                                    BreakoutLevel = rsiSeries[^1],
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
                                    category: "RSI",
                                    symbol: symbol,
                                    cryptocurrencyId: cryptoId,
                                    timeframe: tf,
                                    signalName: "RSIBullishDivergence",
                                    direction: 1,
                                    breakoutLevel: rsiSeries[^1],
                                    nearestResistance: 70m,
                                    nearestSupport: 30m,
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
                }
                else
                {
                    var highsP = new List<int>();
                    for (var i = offset + 1; i < priceSeries.Count - 1; i++)
                    {
                        if (priceSeries[i] >= priceSeries[i - 1] && priceSeries[i] >= priceSeries[i + 1]) highsP.Add(i);
                    }
                    if (highsP.Count >= 2)
                    {
                        var i1 = highsP[^2];
                        var i2 = highsP[^1];
                        var j1 = i1 - offset - 1;
                        var j2 = i2 - offset - 1;
                        if (j1 >= 0 && j2 >= 0 && j2 < rsiSeries.Count)
                        {
                            var priceHigherHigh = priceSeries[i2] > priceSeries[i1];
                            var rsiLowerHigh = rsiSeries[j2] < rsiSeries[j1];
                            if (priceHigherHigh && rsiLowerHigh)
                            {
                                results.Add(new BreakoutResult
                                {
                                    Symbol = symbol,
                                    Timeframe = tf,
                                    IsBreakout = true,
                                    Direction = BreakoutDirection.Down,
                                    BreakoutLevel = rsiSeries[^1],
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
                                    category: "RSI",
                                    symbol: symbol,
                                    cryptocurrencyId: cryptoId,
                                    timeframe: tf,
                                    signalName: "RSIBearishDivergence",
                                    direction: -1,
                                    breakoutLevel: rsiSeries[^1],
                                    nearestResistance: 70m,
                                    nearestSupport: 30m,
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
                }
            }
        }
        return results;
    }

    public Task<List<BreakoutResult>> DetectRsiOverboughtAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod, int period)
        => DetectOverAsync(symbols, timeframes, lookbackPeriod, period, true);

    public Task<List<BreakoutResult>> DetectRsiOversoldAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod, int period)
        => DetectOverAsync(symbols, timeframes, lookbackPeriod, period, false);

    public Task<List<BreakoutResult>> DetectRsiCrossAbove50Async(List<string> symbols, List<string> timeframes, int lookbackPeriod, int period)
        => DetectCross50Async(symbols, timeframes, lookbackPeriod, period, true);

    public Task<List<BreakoutResult>> DetectRsiCrossBelow50Async(List<string> symbols, List<string> timeframes, int lookbackPeriod, int period)
        => DetectCross50Async(symbols, timeframes, lookbackPeriod, period, false);

    public Task<List<BreakoutResult>> DetectRsiBullishDivergenceAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod, int period)
        => DetectDivergenceAsync(symbols, timeframes, lookbackPeriod, period, true);

    public Task<List<BreakoutResult>> DetectRsiBearishDivergenceAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod, int period)
        => DetectDivergenceAsync(symbols, timeframes, lookbackPeriod, period, false);
}