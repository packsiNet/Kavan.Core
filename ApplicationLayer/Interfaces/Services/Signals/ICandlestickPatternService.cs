using ApplicationLayer.Dto.Signals;

namespace ApplicationLayer.Interfaces.Services.Signals
{
    public interface ICandlestickPatternService
    {
        Task<List<BreakoutResult>> DetectBullishHammerAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod);
        Task<List<BreakoutResult>> DetectBullishEngulfingAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod);
        Task<List<BreakoutResult>> DetectThreeWhiteSoldiersAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod);
        Task<List<BreakoutResult>> DetectMorningStarAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod);
        Task<List<BreakoutResult>> DetectBearishShootingStarAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod);
        Task<List<BreakoutResult>> DetectBearishEngulfingAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod);
        Task<List<BreakoutResult>> DetectThreeBlackCrowsAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod);
        Task<List<BreakoutResult>> DetectHangingManAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod);
        Task<List<BreakoutResult>> DetectEveningStarAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod);
        Task<List<BreakoutResult>> DetectDojiAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod);
    }
}