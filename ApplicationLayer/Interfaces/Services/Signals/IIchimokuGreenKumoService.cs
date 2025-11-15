using ApplicationLayer.Dto.Signals;

namespace ApplicationLayer.Interfaces.Services.Signals
{
    public interface IIchimokuGreenKumoService
    {
        Task<List<BreakoutResult>> DetectGreenKumoAsync(
            List<string> symbols,
            List<string> timeframes,
            int lookbackPeriod);
    }
}