using ApplicationLayer.Dto.Signals;

namespace ApplicationLayer.Interfaces.Services.Signals
{
    public interface IAdxSignalService
    {
        Task<List<BreakoutResult>> DetectAdxAboveAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod, int period, decimal threshold);

        Task<List<BreakoutResult>> DetectAdxBelowAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod, int period, decimal threshold);

        Task<List<BreakoutResult>> DetectDiPlusAboveDiMinusAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod, int period);

        Task<List<BreakoutResult>> DetectDiMinusAboveDiPlusAsync(List<string> symbols, List<string> timeframes, int lookbackPeriod, int period);
    }
}