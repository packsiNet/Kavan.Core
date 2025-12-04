using System.Threading;
using System.Threading.Tasks;
using ApplicationLayer.Dto.MarketData;

namespace ApplicationLayer.Interfaces.Services;

public interface IKlineStreamBroadcaster
{
    Task BroadcastAsync(KlineStreamDto dto, CancellationToken cancellationToken = default);
}
