using ApplicationLayer.Dto.Signals;

namespace ApplicationLayer.Interfaces.Services.Signals
{
    public interface ITenkanKijunBullishCrossService
    {
        Task<List<BreakoutResult>> DetectBullishCrossAsync(
            List<string> symbols,
            List<string> timeframes,
            int lookbackPeriod);
    }
}