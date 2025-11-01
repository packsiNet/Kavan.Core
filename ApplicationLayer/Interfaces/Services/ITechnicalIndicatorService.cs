using DomainLayer.Entities;

namespace ApplicationLayer.Interfaces.Services;

public interface ITechnicalIndicatorService
{
    Task<List<FairValueGap>> DetectFairValueGapsAsync(string symbol, string timeframe, CancellationToken cancellationToken = default);
    Task<List<MarketStructureShift>> DetectMarketStructureShiftsAsync(string symbol, string timeframe, CancellationToken cancellationToken = default);
    Task<List<SupportResistanceLevel>> DetectSupportResistanceLevelsAsync(string symbol, string timeframe, CancellationToken cancellationToken = default);
    Task<List<CandlestickPattern>> DetectCandlestickPatternsAsync(string symbol, string timeframe, CancellationToken cancellationToken = default);
    Task<bool> ValidateBreakoutAsync(string symbol, string timeframe, decimal breakPrice, string direction, CancellationToken cancellationToken = default);
    Task<bool> IsPriceInFVGZoneAsync(string symbol, decimal currentPrice, CancellationToken cancellationToken = default);
}