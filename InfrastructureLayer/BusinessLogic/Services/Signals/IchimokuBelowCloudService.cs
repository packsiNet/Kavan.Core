using ApplicationLayer.Dto.Signals;
using ApplicationLayer.Interfaces.Services.Signals;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using InfrastructureLayer.Context;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureLayer.BusinessLogic.Services.Signals;

[InjectAsScoped]
public class IchimokuBelowCloudService(ApplicationDbContext db, ISignalLoggingService logger) : IIchimokuBelowCloudService
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

    private static decimal Mid(IEnumerable<CandleBase> src)
        => (src.Max(c => c.High) + src.Min(c => c.Low)) / 2m;

    private static (decimal A, decimal B) ComputeCloud(List<CandleBase> candlesAsc)
    {
        var tenkan = Mid(candlesAsc.TakeLast(9));
        var kijun = Mid(candlesAsc.TakeLast(26));
        var spanA = (tenkan + kijun) / 2m;
        var spanB = Mid(candlesAsc.TakeLast(52));
        return (spanA, spanB);
    }

    public async Task<List<BreakoutResult>> DetectPriceBelowCloudAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod)
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
                var candlesAsc = await FetchCandlesAsync(tf, cryptoId, lookback);
                if (candlesAsc.Count < 60) continue;

                var atr = ComputeAtr(candlesAsc, 14);
                var last = candlesAsc[^1];
                var prev = candlesAsc[^2];
                var tol = Math.Max(last.Close * 0.0025m, atr * 0.25m);
                var avgVol = candlesAsc.TakeLast(20).Average(c => c.Volume);

                var cloud = ComputeCloud(candlesAsc);
                var bottom = Math.Min(cloud.A, cloud.B);
                var bodySize = Math.Abs(last.Close - last.Open);
                var bodyOk = bodySize >= (atr * 0.5m) && last.Close < last.Open;
                var wickOk = (last.Close - last.Low) <= (atr * 0.6m);
                var below = last.Close <= bottom - Math.Max(tol, atr * 0.2m);

                if (below && bodyOk && wickOk)
                {
                    results.Add(new BreakoutResult
                    {
                        Symbol = symbol,
                        Timeframe = tf,
                        IsBreakout = true,
                        Direction = BreakoutDirection.Down,
                        BreakoutLevel = bottom,
                        CandleTime = last.CloseTime,
                        VolumeRatio = avgVol == 0 ? 0 : (last.Volume / avgVol)
                    });

                    var p = (prev.High + prev.Low + prev.Close) / 3m;
                    var r1 = 2m * p - prev.Low;
                    var s1 = 2m * p - prev.High;
                    var r2 = p + (prev.High - prev.Low);
                    var s2 = p - (prev.High - prev.Low);
                    var r3 = prev.High + 2m * (p - prev.Low);
                    var s3 = prev.Low - 2m * (prev.High - p);

                    await _logger.LogSignalAsync(
                        category: "Ichimoku",
                        symbol: symbol,
                        cryptocurrencyId: cryptoId,
                        timeframe: tf,
                        signalName: "IchimokuPriceBelowCloud",
                        direction: -1,
                        breakoutLevel: bottom,
                        nearestResistance: 0m,
                        nearestSupport: bottom,
                        pivotR1: r1, pivotR2: r2, pivotR3: r3,
                        pivotS1: s1, pivotS2: s2, pivotS3: s3,
                        atr: atr,
                        tolerance: tol,
                        volumeRatio: avgVol == 0 ? 0 : (last.Volume / avgVol),
                        bodySize: bodySize,
                        lastCandle: last,
                        snapshotCandles: candlesAsc);
                }
            }
        }

        return results;
    }
}