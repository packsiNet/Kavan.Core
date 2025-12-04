using System.Threading;
using System.Threading.Tasks;
using ApplicationLayer.Dto.MarketData;
using ApplicationLayer.Interfaces.Services;
using Kavan.Api.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Kavan.Api.Services.SignalR;

public class SignalRKlineBroadcaster(IHubContext<MarketDataHub> hubContext) : IKlineStreamBroadcaster
{
    public Task BroadcastAsync(KlineStreamDto dto, CancellationToken cancellationToken = default)
    {
        var group = $"{dto.Symbol.ToLowerInvariant()}@{dto.Interval.ToLowerInvariant()}";
        return hubContext.Clients.Group(group).SendAsync("klineUpdate", dto, cancellationToken);
    }
}
