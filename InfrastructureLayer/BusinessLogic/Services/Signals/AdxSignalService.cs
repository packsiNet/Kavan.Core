using ApplicationLayer.Dto.Signals;
using ApplicationLayer.Interfaces.Services.Signals;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using InfrastructureLayer.Context;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureLayer.BusinessLogic.Services.Signals;

[InjectAsScoped]
public class AdxSignalService(ApplicationDbContext db, ISignalLoggingService logger) : IAdxSignalService
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

    private static (List<decimal> adx, List<decimal> diPlus, List<decimal> diMinus) ComputeAdxDiSeries(List<CandleBase> c, int period)
    {
        var n = c.Count;
        var trList = new List<decimal>();
        var dmPlusList = new List<decimal>();
        var dmMinusList = new List<decimal>();
        for (var i = 1; i < n; i++)
        {
            var upMove = c[i].High - c[i - 1].High;
            var downMove = c[i - 1].Low - c[i].Low;
            var dmPlus = (upMove > 0m && upMove > downMove) ? upMove : 0m;
            var dmMinus = (downMove > 0m && downMove > upMove) ? downMove : 0m;
            var tr = Math.Max(c[i].High - c[i].Low, Math.Max(Math.Abs(c[i].High - c[i - 1].Close), Math.Abs(c[i].Low - c[i - 1].Close)));
            dmPlusList.Add(dmPlus);
            dmMinusList.Add(dmMinus);
            trList.Add(tr);
        }
        if (trList.Count < period) return (new List<decimal>(), new List<decimal>(), new List<decimal>());

        decimal smoothTR = trList.Take(period).Sum();
        decimal smoothDMPlus = dmPlusList.Take(period).Sum();
        decimal smoothDMMinus = dmMinusList.Take(period).Sum();
        var diPlusSeries = new List<decimal>();
        var diMinusSeries = new List<decimal>();
        var dxSeries = new List<decimal>();

        var diPlus = smoothTR == 0m ? 0m : 100m * (smoothDMPlus / smoothTR);
        var diMinus = smoothTR == 0m ? 0m : 100m * (smoothDMMinus / smoothTR);
        diPlusSeries.Add(diPlus);
        diMinusSeries.Add(diMinus);
        var dx = (diPlus + diMinus) == 0m ? 0m : 100m * Math.Abs(diPlus - diMinus) / (diPlus + diMinus);
        dxSeries.Add(dx);

        for (var i = period; i < trList.Count; i++)
        {
            smoothTR = smoothTR - (smoothTR / period) + trList[i];
            smoothDMPlus = smoothDMPlus - (smoothDMPlus / period) + dmPlusList[i];
            smoothDMMinus = smoothDMMinus - (smoothDMMinus / period) + dmMinusList[i];
            diPlus = smoothTR == 0m ? 0m : 100m * (smoothDMPlus / smoothTR);
            diMinus = smoothTR == 0m ? 0m : 100m * (smoothDMMinus / smoothTR);
            diPlusSeries.Add(diPlus);
            diMinusSeries.Add(diMinus);
            dx = (diPlus + diMinus) == 0m ? 0m : 100m * Math.Abs(diPlus - diMinus) / (diPlus + diMinus);
            dxSeries.Add(dx);
        }

        var adxSeries = new List<decimal>();
        if (dxSeries.Count >= period)
        {
            var adxInit = dxSeries.Take(period).Average();
            adxSeries.Add(adxInit);
            for (var i = period; i < dxSeries.Count; i++)
            {
                var prevAdx = adxSeries[^1];
                var nextAdx = ((prevAdx * (period - 1)) + dxSeries[i]) / period;
                adxSeries.Add(nextAdx);
            }
        }

        return (adxSeries, diPlusSeries, diMinusSeries);
    }

    private async Task<List<BreakoutResult>> DetectAdxAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod, int period, decimal threshold, bool above)
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
                var lookback = Math.Max(lookbackPeriod > 0 ? lookbackPeriod : 200, 100);
                var c = await FetchCandlesAsync(tf, cryptoId, lookback);
                if (c.Count < period + 30) continue;
                var atr = ComputeAtr(c, 14);
                var series = ComputeAdxDiSeries(c, period);
                if (series.adx.Count < 2) continue;
                var last = c[^1];
                var prev = c[^2];
                var adxLast = series.adx[^1];
                var adxPrev = series.adx[^2];
                var diPlusLast = series.diPlus[^1];
                var diMinusLast = series.diMinus[^1];
                var avgVol = c.TakeLast(20).Average(x => x.Volume);
                var volRatio = avgVol == 0 ? 0 : (last.Volume / avgVol);
                var tol = Math.Max(last.Close * 0.0025m, atr * 0.25m);
                var condUp = above && adxLast >= threshold;
                var condDown = !above && adxLast <= threshold;
                if (condUp || condDown)
                {
                    var dirUp = diPlusLast >= diMinusLast;
                    results.Add(new BreakoutResult
                    {
                        Symbol = symbol,
                        Timeframe = tf,
                        IsBreakout = true,
                        Direction = dirUp ? BreakoutDirection.Up : BreakoutDirection.Down,
                        BreakoutLevel = adxLast,
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
                        category: "ADX",
                        symbol: symbol,
                        cryptocurrencyId: cryptoId,
                        timeframe: tf,
                        signalName: above ? "ADXAboveThreshold" : "ADXBelowThreshold",
                        direction: dirUp ? 1 : -1,
                        breakoutLevel: adxLast,
                        nearestResistance: dirUp ? 70m : 50m,
                        nearestSupport: dirUp ? 50m : 30m,
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

    private async Task<List<BreakoutResult>> DetectDiAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod, int period, bool plusAbove)
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
                var lookback = Math.Max(lookbackPeriod > 0 ? lookbackPeriod : 200, 100);
                var c = await FetchCandlesAsync(tf, cryptoId, lookback);
                if (c.Count < period + 30) continue;
                var atr = ComputeAtr(c, 14);
                var series = ComputeAdxDiSeries(c, period);
                if (series.diPlus.Count < 2 || series.diMinus.Count < 2) continue;
                var last = c[^1];
                var prev = c[^2];
                var diPlusLast = series.diPlus[^1];
                var diMinusLast = series.diMinus[^1];
                var avgVol = c.TakeLast(20).Average(x => x.Volume);
                var volRatio = avgVol == 0 ? 0 : (last.Volume / avgVol);
                var tol = Math.Max(last.Close * 0.0025m, atr * 0.25m);
                var cond = plusAbove ? diPlusLast > diMinusLast : diMinusLast > diPlusLast;
                if (cond)
                {
                    results.Add(new BreakoutResult
                    {
                        Symbol = symbol,
                        Timeframe = tf,
                        IsBreakout = true,
                        Direction = plusAbove ? BreakoutDirection.Up : BreakoutDirection.Down,
                        BreakoutLevel = plusAbove ? diPlusLast : diMinusLast,
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
                        category: "ADX",
                        symbol: symbol,
                        cryptocurrencyId: cryptoId,
                        timeframe: tf,
                        signalName: plusAbove ? "DIPlusAboveDIMinus" : "DIMinusAboveDIPlus",
                        direction: plusAbove ? 1 : -1,
                        breakoutLevel: plusAbove ? diPlusLast : diMinusLast,
                        nearestResistance: plusAbove ? 0m : 0m,
                        nearestSupport: plusAbove ? 0m : 0m,
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

    public Task<List<BreakoutResult>> DetectAdxAboveAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod, int period, decimal threshold)
        => DetectAdxAsync(symbols, timeframes, lookbackPeriod, period, threshold, true);

    public Task<List<BreakoutResult>> DetectAdxBelowAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod, int period, decimal threshold)
        => DetectAdxAsync(symbols, timeframes, lookbackPeriod, period, threshold, false);

    public Task<List<BreakoutResult>> DetectDiPlusAboveDiMinusAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod, int period)
        => DetectDiAsync(symbols, timeframes, lookbackPeriod, period, true);

    public Task<List<BreakoutResult>> DetectDiMinusAboveDiPlusAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod, int period)
        => DetectDiAsync(symbols, timeframes, lookbackPeriod, period, false);
}