using ApplicationLayer.Dto.Signals;

namespace ApplicationLayer.Interfaces.Services.Signals
{
    public interface IIchimokuChikouBelowPriceService
    {
        Task<List<BreakoutResult>> DetectChikouBelowPriceAsync(
            List<string> symbols,
            List<string> timeframes,
            int lookbackPeriod);
    }
}