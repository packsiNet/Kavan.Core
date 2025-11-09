using ApplicationLayer.Dto.Signals;
using ApplicationLayer.Interfaces.Services.Signals;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Serilog;
using InfrastructureLayer.Context;

namespace InfrastructureLayer.BusinessLogic.Services.Signals;

[InjectAsScoped]
public class SignalService(ApplicationDbContext db, ISignalLoggingService logger) : ISignalService
{
    private readonly ApplicationDbContext _db = db;
    private readonly ISignalLoggingService _logger = logger;
    private const int AtrPeriod = 14;

    public async Task<List<BreakoutResult>> DetectBreakoutsAsync(
        List<string> symbols,
        List<string> timeframes,
        int lookbackPeriod)
    {
        // Backward-compatible aggregator: merge resistance breakouts and support breakdowns
        var up = await DetectResistanceBreakoutsAsync(symbols, timeframes, lookbackPeriod);
        var down = await DetectSupportBreakdownsAsync(symbols, timeframes, lookbackPeriod);
        return up.Concat(down).ToList();
    }

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

    private async Task<int> ResolveEffectiveLookbackAsync(string timeframe, int cryptoId, int requested)
    {
        if (requested > 0) return requested;

        var baseMap = timeframe switch
        {
            "1m" => 600,
            "5m" => 360,
            "1h" => 200,
            "4h" => 180,
            "1d" => 120,
            _ => 200
        };

        var seed = Math.Max(baseMap, AtrPeriod + 20);
        var seedCandles = await FetchCandlesAsync(timeframe, cryptoId, seed);
        if (seedCandles.Count < Math.Max(seed, AtrPeriod + 2)) return baseMap;

        var atr = ComputeAtr(seedCandles, AtrPeriod);
        var last = seedCandles[^1];
        var atrPct = last.Close == 0 ? 0 : (atr / last.Close);

        var factor = atrPct >= 0.015m ? 1.5m
                    : atrPct <= 0.007m ? 0.7m
                    : 1.0m;

        var effDec = (decimal)baseMap * factor;
        if (effDec < 60m) effDec = 60m;
        if (effDec > 5000m) effDec = 5000m;

        return (int)effDec;
    }

    public async Task<List<BreakoutResult>> DetectResistanceBreakoutsAsync(
        List<string> symbols,
        List<string> timeframes,
        int lookbackPeriod)
    {
        var results = new List<BreakoutResult>();

        var tfList = (timeframes != null && timeframes.Count > 0)
            ? timeframes
            : ["1m", "5m", "1h", "4h", "1d"];

        var symbolList = (symbols != null && symbols.Count > 0)
            ? symbols
            : await _db.Set<Cryptocurrency>().Select(c => c.Symbol).ToListAsync();

        foreach (var symbol in symbolList)
        {
            int cryptoId;
            try
            {
                cryptoId = await _db.Set<Cryptocurrency>()
                    .Where(c => c.Symbol == symbol)
                    .Select(c => c.Id)
                    .FirstOrDefaultAsync();
                if (cryptoId == 0)
                {
                    Log.Information("Skipped {Symbol}: cryptocurrency not found", symbol);
                    continue;
                }
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Failed to resolve cryptocurrency id for {Symbol}", symbol);
                continue;
            }

            foreach (var tf in tfList)
            {
                try
                {
                    var effectiveLookback = await ResolveEffectiveLookbackAsync(tf, cryptoId, lookbackPeriod);
                    var candlesAsc = await FetchCandlesAsync(tf, cryptoId, effectiveLookback);
                    if (candlesAsc.Count < Math.Max(effectiveLookback, AtrPeriod + 2))
                    {
                        Log.Information("Skipped {Symbol}-{Timeframe}: insufficient candles ({Count}/{Required})", symbol, tf, candlesAsc.Count, Math.Max(lookbackPeriod, AtrPeriod + 2));
                        continue;
                    }

                    var atr = ComputeAtr(candlesAsc, AtrPeriod);
                    var tolerance = Math.Max(candlesAsc[^1].Close * 0.0025m, atr * 0.25m); // ~0.25 ATR or 0.25%
                    var avgVol = candlesAsc.TakeLast(20).Average(c => c.Volume);
                    var last = candlesAsc[^1];
                    var prev = candlesAsc[^2];

                    var swingHighs = GetSwingHighs(candlesAsc);
                    var levels = ClusterLevels(swingHighs, tolerance);
                    var nearestResistance = levels
                        .Select(l => l.Level)
                        .Where(lvl => lvl >= last.Close * 0.8m) // drop obviously old levels
                        .OrderBy(lvl => Math.Abs(lvl - last.Close))
                        .FirstOrDefault();

                    if (nearestResistance == 0)
                        continue;

                    var bodySize = Math.Abs(last.Close - last.Open);
                    var bodyOk = bodySize >= (atr * 0.6m) && last.Close > last.Open;
                    var closeBeyond = last.Close >= nearestResistance + Math.Max(tolerance, atr * 0.2m);
                    var preBelow = prev.Close <= nearestResistance;
                    var volOk = avgVol == 0 ? false : (last.Volume / avgVol) >= 1.3m;
                    var wickOk = (last.High - last.Close) <= (atr * 0.5m); // avoid long rejection wick

                    if (closeBeyond && bodyOk && volOk && preBelow && wickOk)
                    {
                        results.Add(new BreakoutResult
                        {
                            Symbol = symbol,
                            Timeframe = tf,
                            IsBreakout = true,
                            Direction = BreakoutDirection.Up,
                            BreakoutLevel = nearestResistance,
                            CandleTime = last.CloseTime,
                            VolumeRatio = avgVol == 0 ? 0 : (last.Volume / avgVol)
                        });

                        // Compute classic pivot points from previous candle
                        var p = (prev.High + prev.Low + prev.Close) / 3m;
                        var r1 = 2m * p - prev.Low;
                        var s1 = 2m * p - prev.High;
                        var r2 = p + (prev.High - prev.Low);
                        var s2 = p - (prev.High - prev.Low);
                        var r3 = prev.High + 2m * (p - prev.Low);
                        var s3 = prev.Low - 2m * (prev.High - p);

                        // Log signal with snapshot candles
                        await _logger.LogBreakoutAsync(
                            symbol,
                            cryptoId,
                            tf,
                            "ResistanceBreakout",
                            1,
                            nearestResistance,
                            nearestResistance,
                            0m,
                            r1, r2, r3, s1, s2, s3,
                            atr,
                            tolerance,
                            avgVol == 0 ? 0 : (last.Volume / avgVol),
                            Math.Abs(last.Close - last.Open),
                            last,
                            candlesAsc);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Resistance breakout detection failed for {Symbol}-{Timeframe}", symbol, tf);
                    continue;
                }
            }
        }

        return results;
    }

    public async Task<List<BreakoutResult>> DetectSupportBreakdownsAsync(
        List<string> symbols,
        List<string> timeframes,
        int lookbackPeriod)
    {
        var results = new List<BreakoutResult>();

        var tfList = (timeframes != null && timeframes.Count > 0)
            ? timeframes
            : ["1m", "5m", "1h", "4h", "1d"];

        var symbolList = (symbols != null && symbols.Count > 0)
            ? symbols
            : await _db.Set<Cryptocurrency>().Select(c => c.Symbol).ToListAsync();

        foreach (var symbol in symbolList)
        {
            int cryptoId;
            try
            {
                cryptoId = await _db.Set<Cryptocurrency>()
                    .Where(c => c.Symbol == symbol)
                    .Select(c => c.Id)
                    .FirstOrDefaultAsync();
                if (cryptoId == 0)
                {
                    Log.Information("Skipped {Symbol}: cryptocurrency not found", symbol);
                    continue;
                }
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Failed to resolve cryptocurrency id for {Symbol}", symbol);
                continue;
            }

            foreach (var tf in tfList)
            {
                try
                {
                    var effectiveLookback = await ResolveEffectiveLookbackAsync(tf, cryptoId, lookbackPeriod);
                    var candlesAsc = await FetchCandlesAsync(tf, cryptoId, effectiveLookback);
                    if (candlesAsc.Count < Math.Max(effectiveLookback, AtrPeriod + 2))
                    {
                        Log.Information("Skipped {Symbol}-{Timeframe}: insufficient candles ({Count}/{Required})", symbol, tf, candlesAsc.Count, Math.Max(lookbackPeriod, AtrPeriod + 2));
                        continue;
                    }

                    var atr = ComputeAtr(candlesAsc, AtrPeriod);
                    var tolerance = Math.Max(candlesAsc[^1].Close * 0.0025m, atr * 0.25m);
                    var avgVol = candlesAsc.TakeLast(20).Average(c => c.Volume);
                    var last = candlesAsc[^1];
                    var prev = candlesAsc[^2];

                    var swingLows = GetSwingLows(candlesAsc);
                    var levels = ClusterLevels(swingLows, tolerance);
                    var nearestSupport = levels
                        .Select(l => l.Level)
                        .Where(lvl => lvl <= last.Close * 1.2m)
                        .OrderBy(lvl => Math.Abs(lvl - last.Close))
                        .FirstOrDefault();

                    if (nearestSupport == 0)
                        continue;

                    var bodySize = Math.Abs(last.Close - last.Open);
                    var bodyOk = bodySize >= (atr * 0.6m) && last.Close < last.Open;
                    var closeBeyond = last.Close <= nearestSupport - Math.Max(tolerance, atr * 0.2m);
                    var preAbove = prev.Close >= nearestSupport;
                    var volOk = avgVol == 0 ? false : (last.Volume / avgVol) >= 1.3m;
                    var wickOk = (last.Close - last.Low) <= (atr * 0.5m);

                    if (closeBeyond && bodyOk && volOk && preAbove && wickOk)
                    {
                        results.Add(new BreakoutResult
                        {
                            Symbol = symbol,
                            Timeframe = tf,
                            IsBreakout = true,
                            Direction = BreakoutDirection.Down,
                            BreakoutLevel = nearestSupport,
                            CandleTime = last.CloseTime,
                            VolumeRatio = avgVol == 0 ? 0 : (last.Volume / avgVol)
                        });

                        // Compute classic pivot points from previous candle
                        var p = (prev.High + prev.Low + prev.Close) / 3m;
                        var r1 = 2m * p - prev.Low;
                        var s1 = 2m * p - prev.High;
                        var r2 = p + (prev.High - prev.Low);
                        var s2 = p - (prev.High - prev.Low);
                        var r3 = prev.High + 2m * (p - prev.Low);
                        var s3 = prev.Low - 2m * (prev.High - p);

                        await _logger.LogBreakoutAsync(
                            symbol,
                            cryptoId,
                            tf,
                            "SupportBreakdown",
                            -1,
                            nearestSupport,
                            0m,
                            nearestSupport,
                            r1, r2, r3, s1, s2, s3,
                            atr,
                            tolerance,
                            avgVol == 0 ? 0 : (last.Volume / avgVol),
                            Math.Abs(last.Close - last.Open),
                            last,
                            candlesAsc);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Support breakdown detection failed for {Symbol}-{Timeframe}", symbol, tf);
                    continue;
                }
            }
        }

        return results;
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

    private static List<decimal> GetSwingHighs(List<CandleBase> candlesAsc)
    {
        var highs = new List<decimal>();
        for (var i = 2; i < candlesAsc.Count - 2; i++)
        {
            var c = candlesAsc[i];
            if (c.High > candlesAsc[i - 1].High && c.High > candlesAsc[i + 1].High &&
                c.High > candlesAsc[i - 2].High && c.High > candlesAsc[i + 2].High)
            {
                highs.Add(c.High);
            }
        }
        return highs;
    }

    private static List<decimal> GetSwingLows(List<CandleBase> candlesAsc)
    {
        var lows = new List<decimal>();
        for (var i = 2; i < candlesAsc.Count - 2; i++)
        {
            var c = candlesAsc[i];
            if (c.Low < candlesAsc[i - 1].Low && c.Low < candlesAsc[i + 1].Low &&
                c.Low < candlesAsc[i - 2].Low && c.Low < candlesAsc[i + 2].Low)
            {
                lows.Add(c.Low);
            }
        }
        return lows;
    }

    private static List<(decimal Level, int Touches)> ClusterLevels(List<decimal> rawLevels, decimal tolerance)
    {
        var sorted = rawLevels.OrderBy(x => x).ToList();
        var clusters = new List<(decimal Level, int Touches)>();
        foreach (var lvl in sorted)
        {
            var found = false;
            for (var i = 0; i < clusters.Count; i++)
            {
                if (Math.Abs(clusters[i].Level - lvl) <= tolerance)
                {
                    var newLevel = (clusters[i].Level * clusters[i].Touches + lvl) / (clusters[i].Touches + 1);
                    clusters[i] = (newLevel, clusters[i].Touches + 1);
                    found = true;
                    break;
                }
            }
            if (!found)
                clusters.Add((lvl, 1));
        }

        // Prefer levels with multiple touches
        return clusters.Where(c => c.Touches >= 2).ToList();
    }
}