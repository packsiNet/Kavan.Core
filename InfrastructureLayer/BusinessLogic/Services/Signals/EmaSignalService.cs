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
public class EmaSignalService(ApplicationDbContext db, ISignalLoggingService logger) : IEmasignalService
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

    private static decimal ComputeEma(List<CandleBase> candlesAsc, int period)
    {
        var subset = candlesAsc.TakeLast(period).ToList();
        if (subset.Count == 0) return 0m;
        var alpha = 2m / (period + 1);
        var ema = subset.Average(c => c.Close);
        for (var i = 1; i < subset.Count; i++)
        {
            ema = subset[i].Close * alpha + ema * (1m - alpha);
        }
        return ema;
    }

    private static decimal ComputePrevEma(List<CandleBase> candlesAsc, int period)
    {
        var subset = candlesAsc.SkipLast(1).TakeLast(period).ToList();
        if (subset.Count == 0) return 0m;
        var alpha = 2m / (period + 1);
        var ema = subset.Average(c => c.Close);
        for (var i = 1; i < subset.Count; i++)
        {
            ema = subset[i].Close * alpha + ema * (1m - alpha);
        }
        return ema;
    }

    private async Task<List<BreakoutResult>> DetectPositionAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod, int period, bool above)
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
                var baseLb = Math.Max(period + 60, 60);
                var lookback = Math.Max(lookbackPeriod > 0 ? lookbackPeriod : baseLb, 60);
                var candlesAsc = await FetchCandlesAsync(tf, cryptoId, lookback);
                if (candlesAsc.Count < period + 1) continue;

                var last = candlesAsc[^1];
                var prev = candlesAsc[^2];
                var atr = ComputeAtr(candlesAsc, 14);
                var tol = Math.Max(last.Close * 0.0025m, atr * 0.25m);
                var avgVol = candlesAsc.TakeLast(20).Average(c => c.Volume);
                var volRatio = avgVol == 0 ? 0 : (last.Volume / avgVol);

                var ema = ComputeEma(candlesAsc, period);
                var prevEma = ComputePrevEma(candlesAsc, period);
                var slope = ema - prevEma;
                var body = Math.Abs(last.Close - last.Open);

                var up = above && last.Close >= ema + Math.Max(tol, atr * 0.1m) && slope >= 0m;
                var down = !above && last.Close <= ema - Math.Max(tol, atr * 0.1m) && slope <= 0m;

                if (up || down)
                {
                    results.Add(new BreakoutResult
                    {
                        Symbol = symbol,
                        Timeframe = tf,
                        IsBreakout = true,
                        Direction = up ? BreakoutDirection.Up : BreakoutDirection.Down,
                        BreakoutLevel = ema,
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
                        category: "EMA",
                        symbol: symbol,
                        cryptocurrencyId: cryptoId,
                        timeframe: tf,
                        signalName: above ? $"PriceAboveEMA{period}" : $"PriceBelowEMA{period}",
                        direction: up ? 1 : -1,
                        breakoutLevel: ema,
                        nearestResistance: up ? 0m : ema,
                        nearestSupport: up ? ema : 0m,
                        pivotR1: r1, pivotR2: r2, pivotR3: r3,
                        pivotS1: s1, pivotS2: s2, pivotS3: s3,
                        atr: atr,
                        tolerance: tol,
                        volumeRatio: volRatio,
                        bodySize: body,
                        lastCandle: last,
                        snapshotCandles: candlesAsc);
                }
            }
        }

        return results;
    }

    private async Task<List<BreakoutResult>> DetectBreakoutAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod, int period, bool up)
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
                var baseLb = Math.Max(period + 60, 60);
                var lookback = Math.Max(lookbackPeriod > 0 ? lookbackPeriod : baseLb, 60);
                var candlesAsc = await FetchCandlesAsync(tf, cryptoId, lookback);
                if (candlesAsc.Count < period + 2) continue;

                var last = candlesAsc[^1];
                var prev = candlesAsc[^2];
                var atr = ComputeAtr(candlesAsc, 14);
                var tol = Math.Max(last.Close * 0.0025m, atr * 0.25m);
                var avgVol = candlesAsc.TakeLast(20).Average(c => c.Volume);
                var volRatio = avgVol == 0 ? 0 : (last.Volume / avgVol);

                var ema = ComputeEma(candlesAsc, period);
                var prevEma = ComputePrevEma(candlesAsc, period);
                var slope = ema - prevEma;

                var body = Math.Abs(last.Close - last.Open);
                var wickTopOk = (last.High - last.Close) <= (atr * 0.6m);
                var wickBottomOk = (last.Open - last.Low) <= (atr * 0.6m);

                var crossedUp = prev.Close <= prevEma && last.Close >= ema + Math.Max(tol, atr * 0.1m);
                var crossedDown = prev.Close >= prevEma && last.Close <= ema - Math.Max(tol, atr * 0.1m);

                var strongBody = body >= (atr * 0.5m);
                var volOk = volRatio >= 1m;
                var slopeOkUp = slope >= 0m;
                var slopeOkDown = slope <= 0m;

                var pass = (up && crossedUp && strongBody && volOk && wickTopOk && slopeOkUp) || (!up && crossedDown && strongBody && volOk && wickBottomOk && slopeOkDown);

                if (pass)
                {
                    results.Add(new BreakoutResult
                    {
                        Symbol = symbol,
                        Timeframe = tf,
                        IsBreakout = true,
                        Direction = up ? BreakoutDirection.Up : BreakoutDirection.Down,
                        BreakoutLevel = ema,
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
                        category: "EMA",
                        symbol: symbol,
                        cryptocurrencyId: cryptoId,
                        timeframe: tf,
                        signalName: up ? $"EMA{period}BreakoutUp" : $"EMA{period}BreakoutDown",
                        direction: up ? 1 : -1,
                        breakoutLevel: ema,
                        nearestResistance: up ? 0m : ema,
                        nearestSupport: up ? ema : 0m,
                        pivotR1: r1, pivotR2: r2, pivotR3: r3,
                        pivotS1: s1, pivotS2: s2, pivotS3: s3,
                        atr: atr,
                        tolerance: tol,
                        volumeRatio: volRatio,
                        bodySize: body,
                        lastCandle: last,
                        snapshotCandles: candlesAsc);
                }
            }
        }

        return results;
    }

    public Task<List<BreakoutResult>> DetectPriceAboveEma10Async(List<string> symbols, List<string> timeframes, int lookbackPeriod)
        => DetectPositionAsync(symbols, timeframes, lookbackPeriod, 10, true);

    public Task<List<BreakoutResult>> DetectPriceBelowEma10Async(List<string> symbols, List<string> timeframes, int lookbackPeriod)
        => DetectPositionAsync(symbols, timeframes, lookbackPeriod, 10, false);

    public Task<List<BreakoutResult>> DetectPriceAboveEma50Async(List<string> symbols, List<string> timeframes, int lookbackPeriod)
        => DetectPositionAsync(symbols, timeframes, lookbackPeriod, 50, true);

    public Task<List<BreakoutResult>> DetectPriceBelowEma50Async(List<string> symbols, List<string> timeframes, int lookbackPeriod)
        => DetectPositionAsync(symbols, timeframes, lookbackPeriod, 50, false);

    public Task<List<BreakoutResult>> DetectPriceAboveEma200Async(List<string> symbols, List<string> timeframes, int lookbackPeriod)
        => DetectPositionAsync(symbols, timeframes, lookbackPeriod, 200, true);

    public Task<List<BreakoutResult>> DetectPriceBelowEma200Async(List<string> symbols, List<string> timeframes, int lookbackPeriod)
        => DetectPositionAsync(symbols, timeframes, lookbackPeriod, 200, false);

    public Task<List<BreakoutResult>> DetectEma20BreakoutUpAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod)
        => DetectBreakoutAsync(symbols, timeframes, lookbackPeriod, 20, true);

    public Task<List<BreakoutResult>> DetectEma20BreakoutDownAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod)
        => DetectBreakoutAsync(symbols, timeframes, lookbackPeriod, 20, false);

    public Task<List<BreakoutResult>> DetectEma100BreakoutUpAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod)
        => DetectBreakoutAsync(symbols, timeframes, lookbackPeriod, 100, true);

    public Task<List<BreakoutResult>> DetectEma100BreakoutDownAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod)
        => DetectBreakoutAsync(symbols, timeframes, lookbackPeriod, 100, false);

    private async Task<List<BreakoutResult>> DetectEmaCrossAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod, int fast, int slow, bool bullish)
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
                var baseLb = Math.Max(slow + 60, 60);
                var lookback = Math.Max(lookbackPeriod > 0 ? lookbackPeriod : baseLb, 60);
                var candlesAsc = await FetchCandlesAsync(tf, cryptoId, lookback);
                if (candlesAsc.Count < slow + 2) continue;

                var last = candlesAsc[^1];
                var prev = candlesAsc[^2];
                var atr = ComputeAtr(candlesAsc, 14);
                var tol = Math.Max(last.Close * 0.0025m, atr * 0.25m);
                var avgVol = candlesAsc.TakeLast(20).Average(c => c.Volume);
                var volRatio = avgVol == 0 ? 0 : (last.Volume / avgVol);

                var emaFast = ComputeEma(candlesAsc, fast);
                var emaSlow = ComputeEma(candlesAsc, slow);
                var prevFast = ComputePrevEma(candlesAsc, fast);
                var prevSlow = ComputePrevEma(candlesAsc, slow);

                var slopeFast = emaFast - prevFast;
                var slopeSlow = emaSlow - prevSlow;

                var crossUp = prevFast <= prevSlow && emaFast > emaSlow;
                var crossDown = prevFast >= prevSlow && emaFast < emaSlow;

                var body = Math.Abs(last.Close - last.Open);
                var priceAboveBoth = last.Close >= Math.Max(emaFast, emaSlow) + Math.Max(tol, atr * 0.1m);
                var priceBelowBoth = last.Close <= Math.Min(emaFast, emaSlow) - Math.Max(tol, atr * 0.1m);

                var passBull = bullish && crossUp && slopeFast > 0m && slopeSlow >= 0m && priceAboveBoth && volRatio >= 1m && body >= (atr * 0.4m);
                var passBear = !bullish && crossDown && slopeFast < 0m && slopeSlow <= 0m && priceBelowBoth && volRatio >= 1m && body >= (atr * 0.4m);

                if (passBull || passBear)
                {
                    results.Add(new BreakoutResult
                    {
                        Symbol = symbol,
                        Timeframe = tf,
                        IsBreakout = true,
                        Direction = passBull ? BreakoutDirection.Up : BreakoutDirection.Down,
                        BreakoutLevel = emaSlow,
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
                        category: "EMA",
                        symbol: symbol,
                        cryptocurrencyId: cryptoId,
                        timeframe: tf,
                        signalName: bullish ? $"EMA{fast}_{slow}BullishCross" : $"EMA{fast}_{slow}BearishCross",
                        direction: passBull ? 1 : -1,
                        breakoutLevel: emaSlow,
                        nearestResistance: passBull ? 0m : emaSlow,
                        nearestSupport: passBull ? emaSlow : 0m,
                        pivotR1: r1, pivotR2: r2, pivotR3: r3,
                        pivotS1: s1, pivotS2: s2, pivotS3: s3,
                        atr: atr,
                        tolerance: tol,
                        volumeRatio: volRatio,
                        bodySize: body,
                        lastCandle: last,
                        snapshotCandles: candlesAsc);
                }
            }
        }

        return results;
    }

    public Task<List<BreakoutResult>> DetectEma50_200BullishCrossAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod)
        => DetectEmaCrossAsync(symbols, timeframes, lookbackPeriod, 50, 200, true);

    public Task<List<BreakoutResult>> DetectEma50_200BearishCrossAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod)
        => DetectEmaCrossAsync(symbols, timeframes, lookbackPeriod, 50, 200, false);
}