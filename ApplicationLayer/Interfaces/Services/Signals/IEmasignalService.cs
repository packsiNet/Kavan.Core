using ApplicationLayer.Dto.Signals;

namespace ApplicationLayer.Interfaces.Services.Signals
{
    public interface IEmasignalService
    {
        Task<List<BreakoutResult>> DetectPriceAboveEma10Async(List<string> symbols, List<string> timeframes, int lookbackPeriod);

        Task<List<BreakoutResult>> DetectPriceBelowEma10Async(List<string> symbols, List<string> timeframes, int lookbackPeriod);

        Task<List<BreakoutResult>> DetectPriceAboveEma50Async(List<string> symbols, List<string> timeframes, int lookbackPeriod);

        Task<List<BreakoutResult>> DetectPriceBelowEma50Async(List<string> symbols, List<string> timeframes, int lookbackPeriod);

        Task<List<BreakoutResult>> DetectPriceAboveEma200Async(List<string> symbols, List<string> timeframes, int lookbackPeriod);

        Task<List<BreakoutResult>> DetectPriceBelowEma200Async(List<string> symbols, List<string> timeframes, int lookbackPeriod);

        Task<List<BreakoutResult>> DetectEma20BreakoutUpAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod);

        Task<List<BreakoutResult>> DetectEma20BreakoutDownAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod);

        Task<List<BreakoutResult>> DetectEma100BreakoutUpAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod);

        Task<List<BreakoutResult>> DetectEma100BreakoutDownAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod);

        Task<List<BreakoutResult>> DetectEma50_200BullishCrossAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod);

        Task<List<BreakoutResult>> DetectEma50_200BearishCrossAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod);
    }
}