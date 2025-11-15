using ApplicationLayer.Dto.Signals;

namespace ApplicationLayer.Interfaces.Services.Signals
{
    public interface IIchimokuChikouAbovePriceService
    {
        Task<List<BreakoutResult>> DetectChikouAbovePriceAsync(
            List<string> symbols,
            List<string> timeframes,
            int lookbackPeriod);
    }
}