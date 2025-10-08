using ApplicationLayer.Dto.Indicators;

namespace ApplicationLayer.Interfaces.Indicators;

public interface IIndicatorService
{
    Task<IndicatorResult?> ComputeRsiAsync(string symbol, string timeFrame, int period, CancellationToken cancellationToken = default);
    Task<IndicatorResult?> ComputeEmaAsync(string symbol, string timeFrame, int period, CancellationToken cancellationToken = default);
    Task<IndicatorResult?> ComputeMacdAsync(string symbol, string timeFrame, int fastPeriod, int slowPeriod, int signalPeriod, CancellationToken cancellationToken = default);
}