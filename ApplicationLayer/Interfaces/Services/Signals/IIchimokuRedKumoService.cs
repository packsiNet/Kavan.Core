using ApplicationLayer.Dto.Signals;

namespace ApplicationLayer.Interfaces.Services.Signals
{
    public interface IIchimokuRedKumoService
    {
        Task<List<BreakoutResult>> DetectRedKumoAsync(
            List<string> symbols,
            List<string> timeframes,
            int lookbackPeriod);
    }
}