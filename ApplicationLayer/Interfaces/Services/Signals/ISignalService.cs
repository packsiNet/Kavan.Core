using ApplicationLayer.Dto.Signals;

namespace ApplicationLayer.Interfaces.Services.Signals
{
    public interface ISignalService
    {
        Task<List<BreakoutResult>> DetectBreakoutsAsync(
            List<string> symbols,
            List<string> timeframes,
            int lookbackPeriod);

        Task<List<BreakoutResult>> DetectResistanceBreakoutsAsync(
            List<string> symbols,
            List<string> timeframes,
            int lookbackPeriod);

        Task<List<BreakoutResult>> DetectSupportBreakdownsAsync(
            List<string> symbols,
            List<string> timeframes,
            int lookbackPeriod);
    }
}