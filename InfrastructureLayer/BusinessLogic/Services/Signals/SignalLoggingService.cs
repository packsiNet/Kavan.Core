using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationLayer.Interfaces.Services.Signals;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using InfrastructureLayer.Context;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureLayer.BusinessLogic.Services.Signals
{
    [InjectAsScoped]
    public class SignalLoggingService(ApplicationDbContext db) : ISignalLoggingService
    {
        private readonly ApplicationDbContext _db = db;

        private async Task<bool> IsDuplicateAsync(
            string symbol,
            string timeframe,
            string signalName,
            int direction,
            decimal breakoutLevel,
            decimal tolerance,
            DateTime candidateCloseTime)
        {
            var last = await _db.Set<Signal>()
                .Where(x => x.Symbol == symbol
                            && x.Timeframe == timeframe
                            && x.SignalCategory == "Technical"
                            && x.SignalName == signalName
                            && x.Direction == direction)
                .OrderByDescending(x => x.SignalTime)
                .FirstOrDefaultAsync();

            if (last == null)
                return false;

            var nearSameLevel = Math.Abs(last.BreakoutLevel - breakoutLevel) <= tolerance;
            if (!nearSameLevel)
                return false;

            // Check invalidation after last signal: if price moved back across level, then a new breakout can be recorded
            // For Up breakout: invalidated if any candle closes <= level - tolerance
            // For Down breakout: invalidated if any candle closes >= level + tolerance

            var candles = await FetchCandlesRangeAsync(timeframe, last.CryptocurrencyId, last.SignalTime, candidateCloseTime);
            if (candles.Count == 0)
                return true; // no new candles, treat as duplicate

            if (direction > 0)
            {
                var invalidated = candles.Any(c => c.Close <= breakoutLevel - tolerance);
                return !invalidated;
            }
            else if (direction < 0)
            {
                var invalidated = candles.Any(c => c.Close >= breakoutLevel + tolerance);
                return !invalidated;
            }
            return true;
        }

        private async Task<List<CandleBase>> FetchCandlesRangeAsync(string timeframe, int cryptoId, DateTime from, DateTime to)
        {
            switch (timeframe)
            {
                case "1m":
                {
                    var list = await _db.Set<Candle_1m>()
                        .Where(c => c.CryptocurrencyId == cryptoId && c.OpenTime > from && c.CloseTime <= to)
                        .OrderBy(c => c.OpenTime)
                        .ToListAsync();
                    return list.Cast<CandleBase>().ToList();
                }
                case "5m":
                {
                    var list = await _db.Set<Candle_5m>()
                        .Where(c => c.CryptocurrencyId == cryptoId && c.OpenTime > from && c.CloseTime <= to)
                        .OrderBy(c => c.OpenTime)
                        .ToListAsync();
                    return list.Cast<CandleBase>().ToList();
                }
                case "1h":
                {
                    var list = await _db.Set<Candle_1h>()
                        .Where(c => c.CryptocurrencyId == cryptoId && c.OpenTime > from && c.CloseTime <= to)
                        .OrderBy(c => c.OpenTime)
                        .ToListAsync();
                    return list.Cast<CandleBase>().ToList();
                }
                case "4h":
                {
                    var list = await _db.Set<Candle_4h>()
                        .Where(c => c.CryptocurrencyId == cryptoId && c.OpenTime > from && c.CloseTime <= to)
                        .OrderBy(c => c.OpenTime)
                        .ToListAsync();
                    return list.Cast<CandleBase>().ToList();
                }
                case "1d":
                {
                    var list = await _db.Set<Candle_1d>()
                        .Where(c => c.CryptocurrencyId == cryptoId && c.OpenTime > from && c.CloseTime <= to)
                        .OrderBy(c => c.OpenTime)
                        .ToListAsync();
                    return list.Cast<CandleBase>().ToList();
                }
                default:
                {
                    var list = await _db.Set<Candle_1h>()
                        .Where(c => c.CryptocurrencyId == cryptoId && c.OpenTime > from && c.CloseTime <= to)
                        .OrderBy(c => c.OpenTime)
                        .ToListAsync();
                    return list.Cast<CandleBase>().ToList();
                }
            }
        }

        public async Task<int> LogBreakoutAsync(
            string symbol,
            int cryptocurrencyId,
            string timeframe,
            string signalName,
            int direction,
            decimal breakoutLevel,
            decimal nearestResistance,
            decimal nearestSupport,
            decimal pivotR1,
            decimal pivotR2,
            decimal pivotR3,
            decimal pivotS1,
            decimal pivotS2,
            decimal pivotS3,
            decimal atr,
            decimal tolerance,
            decimal volumeRatio,
            decimal bodySize,
            CandleBase lastCandle,
            List<CandleBase> snapshotCandles)
        {
            // Deduplication gate: avoid recording the same breakout continuation
            var isDup = await IsDuplicateAsync(symbol, timeframe, signalName, direction, breakoutLevel, tolerance, lastCandle.CloseTime);
            if (isDup)
                return 0; // skip duplicate

            var log = new Signal
            {
                CryptocurrencyId = cryptocurrencyId,
                Symbol = symbol,
                Timeframe = timeframe,
                SignalTime = lastCandle.CloseTime,
                SignalCategory = "Technical",
                SignalName = signalName,
                Direction = direction,
                BreakoutLevel = breakoutLevel,
                NearestResistance = nearestResistance,
                NearestSupport = nearestSupport,
                PivotR1 = pivotR1,
                PivotR2 = pivotR2,
                PivotR3 = pivotR3,
                PivotS1 = pivotS1,
                PivotS2 = pivotS2,
                PivotS3 = pivotS3,
                Atr = atr,
                Tolerance = tolerance,
                VolumeRatio = volumeRatio,
                BodySize = bodySize,
                CandleOpenTime = lastCandle.OpenTime,
                CandleCloseTime = lastCandle.CloseTime,
                CandleOpen = lastCandle.Open,
                CandleHigh = lastCandle.High,
                CandleLow = lastCandle.Low,
                CandleClose = lastCandle.Close,
                CandleVolume = lastCandle.Volume
            };

            _db.Add(log);
            await _db.SaveChangesAsync();

            if (snapshotCandles is { Count: > 0 })
            {
                var ordered = snapshotCandles.OrderBy(c => c.OpenTime).ToList();
                var list = new List<SignalCandle>();
                for (int i = 0; i < ordered.Count; i++)
                {
                    var c = ordered[i];
                    list.Add(new SignalCandle
                    {
                        SignalId = log.Id,
                        Index = i,
                        Timeframe = timeframe,
                        OpenTime = c.OpenTime,
                        CloseTime = c.CloseTime,
                        Open = c.Open,
                        High = c.High,
                        Low = c.Low,
                        Close = c.Close,
                        Volume = c.Volume
                    });
                }

                _db.AddRange(list);
                await _db.SaveChangesAsync();
            }

            return log.Id;
        }
    }
}