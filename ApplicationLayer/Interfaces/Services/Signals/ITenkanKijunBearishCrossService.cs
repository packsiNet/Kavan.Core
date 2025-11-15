using ApplicationLayer.Dto.Signals;

namespace ApplicationLayer.Interfaces.Services.Signals
{
    public interface ITenkanKijunBearishCrossService
    {
        Task<List<BreakoutResult>> DetectBearishCrossAsync(
            List<string> symbols,
            List<string> timeframes,
            int lookbackPeriod);
    }
}