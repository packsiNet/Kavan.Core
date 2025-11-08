using ApplicationLayer.Dto.Signals;

namespace ApplicationLayer.Interfaces.Services.Signals
{
    public interface IIchimokuAboveCloudService
    {
        Task<List<BreakoutResult>> DetectPriceAboveCloudAsync(
            List<string> symbols,
            List<string> timeframes,
            int lookbackPeriod);
    }
}