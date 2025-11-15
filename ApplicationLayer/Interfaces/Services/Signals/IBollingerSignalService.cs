using ApplicationLayer.Dto.Signals;

namespace ApplicationLayer.Interfaces.Services.Signals
{
    public interface IBollingerSignalService
    {
        Task<List<BreakoutResult>> DetectPriceAboveUpperBandAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod);
        Task<List<BreakoutResult>> DetectPriceBelowLowerBandAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod);
        Task<List<BreakoutResult>> DetectUpperBandBreakoutAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod);
        Task<List<BreakoutResult>> DetectLowerBandBreakoutAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod);
        Task<List<BreakoutResult>> DetectBandSqueezeAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod);
    }
}