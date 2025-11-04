#nullable enable
using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationLayer.Dto.Signals;
using Microsoft.EntityFrameworkCore;

namespace ApplicationLayer.Interfaces.Services.Signals
{
    public interface ISignalService
    {
        Task<List<BreakoutResult>> DetectBreakoutsAsync(
            List<string>? symbols,
            List<string>? timeframes,
            int lookbackPeriod,
            DbContext db);
    }
}