using DomainLayer.Entities;

namespace ApplicationLayer.Interfaces.Services.Signals
{
    public interface ISignalLoggingService
    {
        Task<int> LogBreakoutAsync(
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
            List<CandleBase> snapshotCandles);

        Task<int> LogSignalAsync(
            string category,
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
            List<CandleBase> snapshotCandles);
    }
}