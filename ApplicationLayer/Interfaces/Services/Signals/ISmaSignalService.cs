using ApplicationLayer.Dto.Signals;

namespace ApplicationLayer.Interfaces.Services.Signals
{
    public interface ISmaSignalService
    {
        Task<List<BreakoutResult>> DetectPriceAboveSma10Async(List<string> symbols, List<string> timeframes, int lookbackPeriod);
        Task<List<BreakoutResult>> DetectPriceBelowSma10Async(List<string> symbols, List<string> timeframes, int lookbackPeriod);
        Task<List<BreakoutResult>> DetectPriceAboveSma50Async(List<string> symbols, List<string> timeframes, int lookbackPeriod);
        Task<List<BreakoutResult>> DetectPriceBelowSma50Async(List<string> symbols, List<string> timeframes, int lookbackPeriod);
        Task<List<BreakoutResult>> DetectPriceAboveSma200Async(List<string> symbols, List<string> timeframes, int lookbackPeriod);
        Task<List<BreakoutResult>> DetectPriceBelowSma200Async(List<string> symbols, List<string> timeframes, int lookbackPeriod);
        Task<List<BreakoutResult>> DetectSma20BreakoutUpAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod);
        Task<List<BreakoutResult>> DetectSma20BreakoutDownAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod);
        Task<List<BreakoutResult>> DetectSma100BreakoutUpAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod);
        Task<List<BreakoutResult>> DetectSma100BreakoutDownAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod);
        Task<List<BreakoutResult>> DetectSma50_200BullishCrossAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod);
        Task<List<BreakoutResult>> DetectSma50_200BearishCrossAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod);
    }
}