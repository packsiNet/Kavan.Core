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