using ApplicationLayer.Dto.MarketData;
using ApplicationLayer.Interfaces.Services;
using System.Threading;
using System.Threading.Tasks;

namespace Kavan.Worker.Services;

public class NoOpKlineStreamBroadcaster : IKlineStreamBroadcaster
{
    public Task BroadcastAsync(KlineStreamDto dto, CancellationToken cancellationToken = default)
    {
        // No-op for Worker
        return Task.CompletedTask;
    }
}
