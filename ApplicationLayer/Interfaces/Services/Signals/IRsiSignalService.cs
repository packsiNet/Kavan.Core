using ApplicationLayer.Dto.Signals;

namespace ApplicationLayer.Interfaces.Services.Signals
{
    public interface IRsiSignalService
    {
        Task<List<BreakoutResult>> DetectRsiOverboughtAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod, int period);
        Task<List<BreakoutResult>> DetectRsiOversoldAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod, int period);
        Task<List<BreakoutResult>> DetectRsiCrossAbove50Async(List<string> symbols, List<string> timeframes, int lookbackPeriod, int period);
        Task<List<BreakoutResult>> DetectRsiCrossBelow50Async(List<string> symbols, List<string> timeframes, int lookbackPeriod, int period);
        Task<List<BreakoutResult>> DetectRsiBullishDivergenceAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod, int period);
        Task<List<BreakoutResult>> DetectRsiBearishDivergenceAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod, int period);
    }
}