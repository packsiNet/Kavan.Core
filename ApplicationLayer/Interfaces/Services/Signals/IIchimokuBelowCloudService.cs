using ApplicationLayer.Dto.Signals;

namespace ApplicationLayer.Interfaces.Services.Signals
{
    public interface IIchimokuBelowCloudService
    {
        Task<List<BreakoutResult>> DetectPriceBelowCloudAsync(
            List<string> symbols,
            List<string> timeframes,
            int lookbackPeriod);
    }
}