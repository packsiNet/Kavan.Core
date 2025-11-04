using ApplicationLayer.Dto.Signals;
using ApplicationLayer.Interfaces.Services.Signals;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace InfrastructureLayer.BusinessLogic.Services.Signals
{
    [InjectAsScoped]
    public class SignalService : ISignalService
    {
        public async Task<List<BreakoutResult>> DetectBreakoutsAsync(
            List<string> symbols,
            List<string> timeframes,
            int lookbackPeriod,
            DbContext db)
        {
            var results = new List<BreakoutResult>();

            var tfList = (timeframes != null && timeframes.Count > 0)
                ? timeframes
                : ["1m", "5m", "1h", "4h", "1d"];

            List<string> symbolList;
            if (symbols != null && symbols.Count > 0)
            {
                symbolList = symbols;
            }
            else
            {
                symbolList = await db.Set<Cryptocurrency>()
                    .Select(c => c.Symbol)
                    .ToListAsync();
            }

            foreach (var symbol in symbolList)
            {
                int cryptoId;
                try
                {
                    cryptoId = await db.Set<Cryptocurrency>()
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
                        var candlesAsc = await FetchCandlesAsync(db, tf, cryptoId, lookbackPeriod);

                        if (candlesAsc.Count < lookbackPeriod)
                        {
                            Log.Information("Skipped {Symbol}-{Timeframe}: insufficient candles ({Count}/{Lookback})", symbol, tf, candlesAsc.Count, lookbackPeriod);
                            continue;
                        }

                        var highestHigh = candlesAsc.Max(c => c.High);
                        var lowestLow = candlesAsc.Min(c => c.Low);
                        var avgVolume = candlesAsc.Average(c => c.Volume);

                        var lastCandle = candlesAsc[^1];
                        var bodySize = Math.Abs(lastCandle.Close - lastCandle.Open);

                        var lastFive = candlesAsc.Skip(Math.Max(0, candlesAsc.Count - 5)).Take(5).ToList();
                        var avgBody = lastFive.Count == 0 ? 0m : lastFive.Average(c => Math.Abs(c.Close - c.Open));

                        var bullish = lastCandle.Close > highestHigh && lastCandle.Volume > avgVolume && bodySize > avgBody;
                        var bearish = lastCandle.Close < lowestLow && lastCandle.Volume > avgVolume && bodySize > avgBody;

                        if (bullish || bearish)
                        {
                            results.Add(new BreakoutResult
                            {
                                Symbol = symbol,
                                Timeframe = tf,
                                IsBreakout = true,
                                Direction = bullish ? BreakoutDirection.Up : BreakoutDirection.Down,
                                BreakoutLevel = bullish ? highestHigh : lowestLow,
                                CandleTime = lastCandle.CloseTime,
                                VolumeRatio = avgVolume == 0 ? 0 : (lastCandle.Volume / avgVolume)
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Breakout detection failed for {Symbol}-{Timeframe}", symbol, tf);
                        continue;
                    }
                }
            }

            return results;
        }

        private static async Task<List<CandleBase>> FetchCandlesAsync(DbContext db, string timeframe, int cryptoId, int lookback)
        {
            switch (timeframe)
            {
                case "1m":
                {
                    var list = await db.Set<Candle_1m>()
                        .Where(c => c.CryptocurrencyId == cryptoId)
                        .OrderByDescending(c => c.OpenTime)
                        .Take(lookback)
                        .ToListAsync();
                    return list.OrderBy(c => c.OpenTime).Cast<CandleBase>().ToList();
                }
                case "5m":
                {
                    var list = await db.Set<Candle_5m>()
                        .Where(c => c.CryptocurrencyId == cryptoId)
                        .OrderByDescending(c => c.OpenTime)
                        .Take(lookback)
                        .ToListAsync();
                    return list.OrderBy(c => c.OpenTime).Cast<CandleBase>().ToList();
                }
                case "1h":
                {
                    var list = await db.Set<Candle_1h>()
                        .Where(c => c.CryptocurrencyId == cryptoId)
                        .OrderByDescending(c => c.OpenTime)
                        .Take(lookback)
                        .ToListAsync();
                    return list.OrderBy(c => c.OpenTime).Cast<CandleBase>().ToList();
                }
                case "4h":
                {
                    var list = await db.Set<Candle_4h>()
                        .Where(c => c.CryptocurrencyId == cryptoId)
                        .OrderByDescending(c => c.OpenTime)
                        .Take(lookback)
                        .ToListAsync();
                    return list.OrderBy(c => c.OpenTime).Cast<CandleBase>().ToList();
                }
                case "1d":
                {
                    var list = await db.Set<Candle_1d>()
                        .Where(c => c.CryptocurrencyId == cryptoId)
                        .OrderByDescending(c => c.OpenTime)
                        .Take(lookback)
                        .ToListAsync();
                    return list.OrderBy(c => c.OpenTime).Cast<CandleBase>().ToList();
                }
                default:
                {
                    var list = await db.Set<Candle_1h>()
                        .Where(c => c.CryptocurrencyId == cryptoId)
                        .OrderByDescending(c => c.OpenTime)
                        .Take(lookback)
                        .ToListAsync();
                    return list.OrderBy(c => c.OpenTime).Cast<CandleBase>().ToList();
                }
            }
        }
    }
}