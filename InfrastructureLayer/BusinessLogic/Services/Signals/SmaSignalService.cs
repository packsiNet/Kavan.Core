using ApplicationLayer.Dto.Signals;
using ApplicationLayer.Interfaces.Services.Signals;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using InfrastructureLayer.Context;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureLayer.BusinessLogic.Services.Signals;

[InjectAsScoped]
public class SmaSignalService(ApplicationDbContext db, ISignalLoggingService logger) : ISmaSignalService
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

    private static decimal ComputeSma(List<CandleBase> candlesAsc, int period)
        => candlesAsc.TakeLast(period).Average(c => c.Close);

    private static decimal ComputePrevSma(List<CandleBase> candlesAsc, int period)
        => candlesAsc.SkipLast(1).TakeLast(period).Average(c => c.Close);

    private async Task<List<BreakoutResult>> DetectSmaCrossAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod, int fast, int slow, bool bullish)
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

                var smaFast = ComputeSma(candlesAsc, fast);
                var smaSlow = candlesAsc.TakeLast(slow).Average(c => c.Close);
                var prevFast = ComputePrevSma(candlesAsc, fast);
                var prevSlow = candlesAsc.SkipLast(1).TakeLast(slow).Average(c => c.Close);

                var slopeFast = smaFast - prevFast;
                var slopeSlow = smaSlow - prevSlow;

                var crossUp = prevFast <= prevSlow && smaFast > smaSlow;
                var crossDown = prevFast >= prevSlow && smaFast < smaSlow;

                var body = Math.Abs(last.Close - last.Open);
                var priceAboveBoth = last.Close >= Math.Max(smaFast, smaSlow) + Math.Max(tol, atr * 0.1m);
                var priceBelowBoth = last.Close <= Math.Min(smaFast, smaSlow) - Math.Max(tol, atr * 0.1m);

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
                        BreakoutLevel = smaSlow,
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
                        category: "SMA",
                        symbol: symbol,
                        cryptocurrencyId: cryptoId,
                        timeframe: tf,
                        signalName: bullish ? $"SMA{fast}_{slow}BullishCross" : $"SMA{fast}_{slow}BearishCross",
                        direction: passBull ? 1 : -1,
                        breakoutLevel: smaSlow,
                        nearestResistance: passBull ? 0m : smaSlow,
                        nearestSupport: passBull ? smaSlow : 0m,
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

    public Task<List<BreakoutResult>> DetectSma50_200BullishCrossAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod)
        => DetectSmaCrossAsync(symbols, timeframes, lookbackPeriod, 50, 200, true);

    public Task<List<BreakoutResult>> DetectSma50_200BearishCrossAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod)
        => DetectSmaCrossAsync(symbols, timeframes, lookbackPeriod, 50, 200, false);

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

                var sma = ComputeSma(candlesAsc, period);
                var prevSma = ComputePrevSma(candlesAsc, period);
                var slope = sma - prevSma;

                var body = Math.Abs(last.Close - last.Open);
                var wickTopOk = (last.High - last.Close) <= (atr * 0.6m);
                var wickBottomOk = (last.Open - last.Low) <= (atr * 0.6m);

                var crossedUp = prev.Close <= prevSma && last.Close >= sma + Math.Max(tol, atr * 0.1m);
                var crossedDown = prev.Close >= prevSma && last.Close <= sma - Math.Max(tol, atr * 0.1m);

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
                        BreakoutLevel = sma,
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
                        category: "SMA",
                        symbol: symbol,
                        cryptocurrencyId: cryptoId,
                        timeframe: tf,
                        signalName: up ? $"SMA{period}BreakoutUp" : $"SMA{period}BreakoutDown",
                        direction: up ? 1 : -1,
                        breakoutLevel: sma,
                        nearestResistance: up ? 0m : sma,
                        nearestSupport: up ? sma : 0m,
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

    public Task<List<BreakoutResult>> DetectSma20BreakoutUpAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod)
        => DetectBreakoutAsync(symbols, timeframes, lookbackPeriod, 20, true);

    public Task<List<BreakoutResult>> DetectSma20BreakoutDownAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod)
        => DetectBreakoutAsync(symbols, timeframes, lookbackPeriod, 20, false);

    public Task<List<BreakoutResult>> DetectSma100BreakoutUpAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod)
        => DetectBreakoutAsync(symbols, timeframes, lookbackPeriod, 100, true);

    public Task<List<BreakoutResult>> DetectSma100BreakoutDownAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod)
        => DetectBreakoutAsync(symbols, timeframes, lookbackPeriod, 100, false);

    private async Task<List<BreakoutResult>> DetectAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod, int period, bool above)
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

                var sma = ComputeSma(candlesAsc, period);
                var prevSma = ComputePrevSma(candlesAsc, period);
                var slope = sma - prevSma;
                var dist = Math.Abs(last.Close - sma);
                var volRatio = avgVol == 0 ? 0 : (last.Volume / avgVol);

                var body = Math.Abs(last.Close - last.Open);
                var up = above && last.Close >= sma + Math.Max(tol, atr * 0.1m) && slope >= 0m;
                var down = !above && last.Close <= sma - Math.Max(tol, atr * 0.1m) && slope <= 0m;

                if (up || down)
                {
                    results.Add(new BreakoutResult
                    {
                        Symbol = symbol,
                        Timeframe = tf,
                        IsBreakout = true,
                        Direction = up ? BreakoutDirection.Up : BreakoutDirection.Down,
                        BreakoutLevel = sma,
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
                        category: "SMA",
                        symbol: symbol,
                        cryptocurrencyId: cryptoId,
                        timeframe: tf,
                        signalName: above ? $"PriceAboveSMA{period}" : $"PriceBelowSMA{period}",
                        direction: up ? 1 : -1,
                        breakoutLevel: sma,
                        nearestResistance: up ? 0m : sma,
                        nearestSupport: up ? sma : 0m,
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

    public Task<List<BreakoutResult>> DetectPriceAboveSma10Async(List<string> symbols, List<string> timeframes, int lookbackPeriod)
        => DetectAsync(symbols, timeframes, lookbackPeriod, 10, true);

    public Task<List<BreakoutResult>> DetectPriceBelowSma10Async(List<string> symbols, List<string> timeframes, int lookbackPeriod)
        => DetectAsync(symbols, timeframes, lookbackPeriod, 10, false);

    public Task<List<BreakoutResult>> DetectPriceAboveSma50Async(List<string> symbols, List<string> timeframes, int lookbackPeriod)
        => DetectAsync(symbols, timeframes, lookbackPeriod, 50, true);

    public Task<List<BreakoutResult>> DetectPriceBelowSma50Async(List<string> symbols, List<string> timeframes, int lookbackPeriod)
        => DetectAsync(symbols, timeframes, lookbackPeriod, 50, false);

    public Task<List<BreakoutResult>> DetectPriceAboveSma200Async(List<string> symbols, List<string> timeframes, int lookbackPeriod)
        => DetectAsync(symbols, timeframes, lookbackPeriod, 200, true);

    public Task<List<BreakoutResult>> DetectPriceBelowSma200Async(List<string> symbols, List<string> timeframes, int lookbackPeriod)
        => DetectAsync(symbols, timeframes, lookbackPeriod, 200, false);
}