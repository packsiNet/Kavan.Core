using ApplicationLayer.Dto.Signals;
using ApplicationLayer.Interfaces.Services.Signals;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using InfrastructureLayer.Context;
using Microsoft.EntityFrameworkCore;
using ApplicationLayer.Common.Utilities;

namespace InfrastructureLayer.BusinessLogic.Services.Signals;

[InjectAsScoped]
public class CandlestickPatternService(ApplicationDbContext db, ISignalLoggingService logger) : ICandlestickPatternService
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

    private static bool DownTrend(List<CandleBase> c)
        => c[^1].Close < c[^2].Close && c[^2].Close < c[^3].Close;

    private static bool UpTrend(List<CandleBase> c)
        => c[^1].Close > c[^2].Close && c[^2].Close > c[^3].Close;

    private static bool Bullish(CandleBase x) => x.Close > x.Open;

    private static bool Bearish(CandleBase x) => x.Close < x.Open;

    private static decimal Body(CandleBase x) => Math.Abs(x.Close - x.Open);

    private static decimal UpperWick(CandleBase x) => x.High - Math.Max(x.Open, x.Close);

    private static decimal LowerWick(CandleBase x) => Math.Min(x.Open, x.Close) - x.Low;

    private async Task<List<BreakoutResult>> DetectBullishHammerAsyncInternal(List<string> symbols, List<string> timeframes, int lookbackPeriod)
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
                var lookback = Math.Max(lookbackPeriod > 0 ? lookbackPeriod : 90, 60);
                var candlesAsc = await FetchCandlesAsync(tf, cryptoId, lookback);
                if (candlesAsc.Count < 5) continue;
                var atr = ComputeAtr(candlesAsc, 14);
                var last = candlesAsc[^1];
                var prev = candlesAsc[^2];
                var avgVol = candlesAsc.TakeLast(20).Average(c => c.Volume);
                var b = Body(last);
                var lw = LowerWick(last);
                var uw = UpperWick(last);
                var cond = DownTrend(candlesAsc) && lw >= b * 2m && uw <= b * 0.6m && Bullish(last) && Bearish(prev);
                var volOk = avgVol == 0 ? false : (last.Volume / avgVol) >= 1m;
                if (cond && volOk)
                {
                    results.Add(new BreakoutResult { Symbol = symbol, Timeframe = tf, IsBreakout = true, Direction = BreakoutDirection.Up, BreakoutLevel = last.Close, CandleTime = last.CloseTime, VolumeRatio = avgVol == 0 ? 0 : (last.Volume / avgVol) });
                    var p = (prev.High + prev.Low + prev.Close) / 3m;
                    var r1 = 2m * p - prev.Low;
                    var s1 = 2m * p - prev.High;
                    var r2 = p + (prev.High - prev.Low);
                    var s2 = p - (prev.High - prev.Low);
                    var r3 = prev.High + 2m * (p - prev.Low);
                    var s3 = prev.Low - 2m * (prev.High - p);
                    await _logger.LogSignalAsync("Candlestick", symbol, cryptoId, tf, "BullishHammer", 1, last.Close, last.High, last.Low, r1, r2, r3, s1, s2, s3, atr, SignalThresholdsExtensions.ComputeTolerance(last.Close, atr, tf), avgVol == 0 ? 0 : (last.Volume / avgVol), b, last, candlesAsc);
                }
            }
        }
        return results;
    }

    private async Task<List<BreakoutResult>> DetectBearishShootingStarAsyncInternal(List<string> symbols, List<string> timeframes, int lookbackPeriod)
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
                var lookback = Math.Max(lookbackPeriod > 0 ? lookbackPeriod : 90, 60);
                var candlesAsc = await FetchCandlesAsync(tf, cryptoId, lookback);
                if (candlesAsc.Count < 5) continue;
                var atr = ComputeAtr(candlesAsc, 14);
                var last = candlesAsc[^1];
                var prev = candlesAsc[^2];
                var avgVol = candlesAsc.TakeLast(20).Average(c => c.Volume);
                var b = Body(last);
                var lw = LowerWick(last);
                var uw = UpperWick(last);
                var cond = UpTrend(candlesAsc) && uw >= b * 2m && lw <= b * 0.6m && Bearish(last) && Bullish(prev);
                var volOk = avgVol == 0 ? false : (last.Volume / avgVol) >= 1m;
                if (cond && volOk)
                {
                    results.Add(new BreakoutResult { Symbol = symbol, Timeframe = tf, IsBreakout = true, Direction = BreakoutDirection.Down, BreakoutLevel = last.Close, CandleTime = last.CloseTime, VolumeRatio = avgVol == 0 ? 0 : (last.Volume / avgVol) });
                    var p = (prev.High + prev.Low + prev.Close) / 3m;
                    var r1 = 2m * p - prev.Low;
                    var s1 = 2m * p - prev.High;
                    var r2 = p + (prev.High - prev.Low);
                    var s2 = p - (prev.High - prev.Low);
                    var r3 = prev.High + 2m * (p - prev.Low);
                    var s3 = prev.Low - 2m * (prev.High - p);
                    await _logger.LogSignalAsync("Candlestick", symbol, cryptoId, tf, "ShootingStar", -1, last.Close, last.High, last.Low, r1, r2, r3, s1, s2, s3, atr, SignalThresholdsExtensions.ComputeTolerance(last.Close, atr, tf), avgVol == 0 ? 0 : (last.Volume / avgVol), b, last, candlesAsc);
                }
            }
        }
        return results;
    }

    public Task<List<BreakoutResult>> DetectBullishHammerAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod)
        => DetectBullishHammerAsyncInternal(symbols, timeframes, lookbackPeriod);

    public Task<List<BreakoutResult>> DetectBearishShootingStarAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod)
        => DetectBearishShootingStarAsyncInternal(symbols, timeframes, lookbackPeriod);

    public async Task<List<BreakoutResult>> DetectBullishEngulfingAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod)
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
                var lookback = Math.Max(lookbackPeriod > 0 ? lookbackPeriod : 90, 60);
                var c = await FetchCandlesAsync(tf, cryptoId, lookback);
                if (c.Count < 3) continue;
                var atr = ComputeAtr(c, 14);
                var last = c[^1];
                var prev = c[^2];
                var avgVol = c.TakeLast(20).Average(x => x.Volume);
                var cond = Bearish(prev) && Bullish(last) && last.Open <= prev.Close && last.Close >= prev.Open;
                var volOk = avgVol == 0 ? false : (last.Volume / avgVol) >= 1m;
                if (cond && volOk)
                {
                    var b = Body(last);
                    results.Add(new BreakoutResult { Symbol = symbol, Timeframe = tf, IsBreakout = true, Direction = BreakoutDirection.Up, BreakoutLevel = last.Close, CandleTime = last.CloseTime, VolumeRatio = avgVol == 0 ? 0 : (last.Volume / avgVol) });
                    var p = (prev.High + prev.Low + prev.Close) / 3m;
                    var r1 = 2m * p - prev.Low;
                    var s1 = 2m * p - prev.High;
                    var r2 = p + (prev.High - prev.Low);
                    var s2 = p - (prev.High - prev.Low);
                    var r3 = prev.High + 2m * (p - prev.Low);
                    var s3 = prev.Low - 2m * (prev.High - p);
                    await _logger.LogSignalAsync("Candlestick", symbol, cryptoId, tf, "BullishEngulfing", 1, last.Close, last.High, last.Low, r1, r2, r3, s1, s2, s3, atr, SignalThresholdsExtensions.ComputeTolerance(last.Close, atr, tf), avgVol == 0 ? 0 : (last.Volume / avgVol), b, last, c);
                }
            }
        }
        return results;
    }

    public async Task<List<BreakoutResult>> DetectBearishEngulfingAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod)
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
                var lookback = Math.Max(lookbackPeriod > 0 ? lookbackPeriod : 90, 60);
                var c = await FetchCandlesAsync(tf, cryptoId, lookback);
                if (c.Count < 3) continue;
                var atr = ComputeAtr(c, 14);
                var last = c[^1];
                var prev = c[^2];
                var avgVol = c.TakeLast(20).Average(x => x.Volume);
                var cond = Bullish(prev) && Bearish(last) && last.Open >= prev.Close && last.Close <= prev.Open;
                var volOk = avgVol == 0 ? false : (last.Volume / avgVol) >= 1m;
                if (cond && volOk)
                {
                    var b = Body(last);
                    results.Add(new BreakoutResult { Symbol = symbol, Timeframe = tf, IsBreakout = true, Direction = BreakoutDirection.Down, BreakoutLevel = last.Close, CandleTime = last.CloseTime, VolumeRatio = avgVol == 0 ? 0 : (last.Volume / avgVol) });
                    var p = (prev.High + prev.Low + prev.Close) / 3m;
                    var r1 = 2m * p - prev.Low;
                    var s1 = 2m * p - prev.High;
                    var r2 = p + (prev.High - prev.Low);
                    var s2 = p - (prev.High - prev.Low);
                    var r3 = prev.High + 2m * (p - prev.Low);
                    var s3 = prev.Low - 2m * (prev.High - p);
                    await _logger.LogSignalAsync("Candlestick", symbol, cryptoId, tf, "BearishEngulfing", -1, last.Close, last.High, last.Low, r1, r2, r3, s1, s2, s3, atr, SignalThresholdsExtensions.ComputeTolerance(last.Close, atr, tf), avgVol == 0 ? 0 : (last.Volume / avgVol), b, last, c);
                }
            }
        }
        return results;
    }

    public async Task<List<BreakoutResult>> DetectThreeWhiteSoldiersAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod)
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
                if (c.Count < 4) continue;
                var atr = ComputeAtr(c, 14);
                var a = c[^3]; var b = c[^2]; var d = c[^1];
                var avgVol = c.TakeLast(20).Average(x => x.Volume);
                var cond = Bullish(a) && Bullish(b) && Bullish(d) && a.Close < b.Close && b.Close < d.Close && Body(a) >= atr * 0.4m && Body(b) >= atr * 0.4m && Body(d) >= atr * 0.4m && UpperWick(a) <= Body(a) * 0.6m && UpperWick(b) <= Body(b) * 0.6m && UpperWick(d) <= Body(d) * 0.6m;
                var volOk = avgVol == 0 ? false : (d.Volume / avgVol) >= 1m;
                if (cond && volOk)
                {
                    results.Add(new BreakoutResult { Symbol = symbol, Timeframe = tf, IsBreakout = true, Direction = BreakoutDirection.Up, BreakoutLevel = d.Close, CandleTime = d.CloseTime, VolumeRatio = avgVol == 0 ? 0 : (d.Volume / avgVol) });
                    var p = (b.High + b.Low + b.Close) / 3m;
                    var r1 = 2m * p - b.Low;
                    var s1 = 2m * p - b.High;
                    var r2 = p + (b.High - b.Low);
                    var s2 = p - (b.High - b.Low);
                    var r3 = b.High + 2m * (p - b.Low);
                    var s3 = b.Low - 2m * (b.High - p);
                    await _logger.LogSignalAsync("Candlestick", symbol, cryptoId, tf, "ThreeWhiteSoldiers", 1, d.Close, d.High, d.Low, r1, r2, r3, s1, s2, s3, atr, SignalThresholdsExtensions.ComputeTolerance(d.Close, atr, tf), avgVol == 0 ? 0 : (d.Volume / avgVol), Body(d), d, c);
                }
            }
        }
        return results;
    }

    public async Task<List<BreakoutResult>> DetectThreeBlackCrowsAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod)
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
                if (c.Count < 4) continue;
                var atr = ComputeAtr(c, 14);
                var a = c[^3]; var b = c[^2]; var d = c[^1];
                var avgVol = c.TakeLast(20).Average(x => x.Volume);
                var cond = Bearish(a) && Bearish(b) && Bearish(d) && a.Close > b.Close && b.Close > d.Close && Body(a) >= atr * 0.4m && Body(b) >= atr * 0.4m && Body(d) >= atr * 0.4m && LowerWick(a) <= Body(a) * 0.6m && LowerWick(b) <= Body(b) * 0.6m && LowerWick(d) <= Body(d) * 0.6m;
                var volOk = avgVol == 0 ? false : (d.Volume / avgVol) >= 1m;
                if (cond && volOk)
                {
                    results.Add(new BreakoutResult { Symbol = symbol, Timeframe = tf, IsBreakout = true, Direction = BreakoutDirection.Down, BreakoutLevel = d.Close, CandleTime = d.CloseTime, VolumeRatio = avgVol == 0 ? 0 : (d.Volume / avgVol) });
                    var p = (b.High + b.Low + b.Close) / 3m;
                    var r1 = 2m * p - b.Low;
                    var s1 = 2m * p - b.High;
                    var r2 = p + (b.High - b.Low);
                    var s2 = p - (b.High - b.Low);
                    var r3 = b.High + 2m * (p - b.Low);
                    var s3 = b.Low - 2m * (b.High - p);
                    await _logger.LogSignalAsync("Candlestick", symbol, cryptoId, tf, "ThreeBlackCrows", -1, d.Close, d.High, d.Low, r1, r2, r3, s1, s2, s3, atr, SignalThresholdsExtensions.ComputeTolerance(d.Close, atr, tf), avgVol == 0 ? 0 : (d.Volume / avgVol), Body(d), d, c);
                }
            }
        }
        return results;
    }

    public async Task<List<BreakoutResult>> DetectMorningStarAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod)
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
                if (c.Count < 4) continue;
                var atr = ComputeAtr(c, 14);
                var a = c[^3]; var b = c[^2]; var d = c[^1];
                var avgVol = c.TakeLast(20).Average(x => x.Volume);
                var cond = Bearish(a) && Body(a) >= atr * 0.5m && Body(b) <= atr * 0.25m && Bullish(d) && Body(d) >= atr * 0.5m && d.Close >= a.Open - (Body(a) * 0.5m);
                var volOk = avgVol == 0 ? false : (d.Volume / avgVol) >= 1m;
                if (cond && volOk)
                {
                    results.Add(new BreakoutResult { Symbol = symbol, Timeframe = tf, IsBreakout = true, Direction = BreakoutDirection.Up, BreakoutLevel = d.Close, CandleTime = d.CloseTime, VolumeRatio = avgVol == 0 ? 0 : (d.Volume / avgVol) });
                    var p = (b.High + b.Low + b.Close) / 3m;
                    var r1 = 2m * p - b.Low;
                    var s1 = 2m * p - b.High;
                    var r2 = p + (b.High - b.Low);
                    var s2 = p - (b.High - b.Low);
                    var r3 = b.High + 2m * (p - b.Low);
                    var s3 = b.Low - 2m * (b.High - p);
                    await _logger.LogSignalAsync("Candlestick", symbol, cryptoId, tf, "MorningStar", 1, d.Close, d.High, d.Low, r1, r2, r3, s1, s2, s3, atr, SignalThresholdsExtensions.ComputeTolerance(d.Close, atr, tf), avgVol == 0 ? 0 : (d.Volume / avgVol), Body(d), d, c);
                }
            }
        }
        return results;
    }

    public async Task<List<BreakoutResult>> DetectEveningStarAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod)
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
                if (c.Count < 4) continue;
                var atr = ComputeAtr(c, 14);
                var a = c[^3]; var b = c[^2]; var d = c[^1];
                var avgVol = c.TakeLast(20).Average(x => x.Volume);
                var cond = Bullish(a) && Body(a) >= atr * 0.5m && Body(b) <= atr * 0.25m && Bearish(d) && Body(d) >= atr * 0.5m && d.Close <= a.Open + (Body(a) * -0.5m);
                var volOk = avgVol == 0 ? false : (d.Volume / avgVol) >= 1m;
                if (cond && volOk)
                {
                    results.Add(new BreakoutResult { Symbol = symbol, Timeframe = tf, IsBreakout = true, Direction = BreakoutDirection.Down, BreakoutLevel = d.Close, CandleTime = d.CloseTime, VolumeRatio = avgVol == 0 ? 0 : (d.Volume / avgVol) });
                    var p = (b.High + b.Low + b.Close) / 3m;
                    var r1 = 2m * p - b.Low;
                    var s1 = 2m * p - b.High;
                    var r2 = p + (b.High - b.Low);
                    var s2 = p - (b.High - b.Low);
                    var r3 = b.High + 2m * (p - b.Low);
                    var s3 = b.Low - 2m * (b.High - p);
                    await _logger.LogSignalAsync("Candlestick", symbol, cryptoId, tf, "EveningStar", -1, d.Close, d.High, d.Low, r1, r2, r3, s1, s2, s3, atr, SignalThresholdsExtensions.ComputeTolerance(d.Close, atr, tf), avgVol == 0 ? 0 : (d.Volume / avgVol), Body(d), d, c);
                }
            }
        }
        return results;
    }

    public async Task<List<BreakoutResult>> DetectHangingManAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod)
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
                var lookback = Math.Max(lookbackPeriod > 0 ? lookbackPeriod : 90, 60);
                var c = await FetchCandlesAsync(tf, cryptoId, lookback);
                if (c.Count < 5) continue;
                var atr = ComputeAtr(c, 14);
                var last = c[^1];
                var prev = c[^2];
                var avgVol = c.TakeLast(20).Average(x => x.Volume);
                var b = Body(last);
                var lw = LowerWick(last);
                var uw = UpperWick(last);
                var cond = UpTrend(c) && lw >= b * 2m && uw <= b * 0.6m && Bearish(last) && Bullish(prev);
                var volOk = avgVol == 0 ? false : (last.Volume / avgVol) >= 1m;
                if (cond && volOk)
                {
                    results.Add(new BreakoutResult { Symbol = symbol, Timeframe = tf, IsBreakout = true, Direction = BreakoutDirection.Down, BreakoutLevel = last.Close, CandleTime = last.CloseTime, VolumeRatio = avgVol == 0 ? 0 : (last.Volume / avgVol) });
                    var p = (prev.High + prev.Low + prev.Close) / 3m;
                    var r1 = 2m * p - prev.Low;
                    var s1 = 2m * p - prev.High;
                    var r2 = p + (prev.High - prev.Low);
                    var s2 = p - (prev.High - prev.Low);
                    var r3 = prev.High + 2m * (p - prev.Low);
                    var s3 = prev.Low - 2m * (prev.High - p);
                    await _logger.LogSignalAsync("Candlestick", symbol, cryptoId, tf, "HangingMan", -1, last.Close, last.High, last.Low, r1, r2, r3, s1, s2, s3, atr, SignalThresholdsExtensions.ComputeTolerance(last.Close, atr, tf), avgVol == 0 ? 0 : (last.Volume / avgVol), b, last, c);
                }
            }
        }
        return results;
    }

    public async Task<List<BreakoutResult>> DetectDojiAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod)
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
                var lookback = Math.Max(lookbackPeriod > 0 ? lookbackPeriod : 60, 60);
                var c = await FetchCandlesAsync(tf, cryptoId, lookback);
                if (c.Count < 3) continue;
                var atr = ComputeAtr(c, 14);
                var last = c[^1];
                var avgVol = c.TakeLast(20).Average(x => x.Volume);
                var rng = last.High - last.Low;
                var b = Body(last);
                var cond = b <= Math.Max(atr * 0.15m, rng * 0.1m);
                if (cond)
                {
                    await _logger.LogSignalAsync("Candlestick", symbol, cryptoId, tf, "Doji", 0, last.Close, last.High, last.Low, 0m, 0m, 0m, 0m, 0m, 0m, atr, SignalThresholdsExtensions.ComputeTolerance(last.Close, atr, tf), avgVol == 0 ? 0 : (last.Volume / avgVol), b, last, c);
                }
            }
        }
        return results;
    }
}