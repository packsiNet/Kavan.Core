using System.Threading;
using System.Threading.Tasks;

namespace ApplicationLayer.Interfaces.Services.Candles;

public interface ICandleAggregatorService
{
    Task ExecuteAggregationAsync(CancellationToken cancellationToken);
}
