using ApplicationLayer.Dto.Signals;

namespace ApplicationLayer.Interfaces.Services.Signals
{
    public interface ICciSignalService
    {
        Task<List<BreakoutResult>> DetectCciAbove100Async(List<string> symbols, List<string> timeframes, int lookbackPeriod, int period);

        Task<List<BreakoutResult>> DetectCciBelowMinus100Async(List<string> symbols, List<string> timeframes, int lookbackPeriod, int period);

        Task<List<BreakoutResult>> DetectCciCrossAboveZeroAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod, int period);

        Task<List<BreakoutResult>> DetectCciCrossBelowZeroAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod, int period);
    }
}