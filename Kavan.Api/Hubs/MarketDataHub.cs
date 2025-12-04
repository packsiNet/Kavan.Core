using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Kavan.Api.Hubs;

[Authorize]
public class MarketDataHub : Hub
{
    public async Task SubscribeKlines(string symbol, string interval)
    {
        var group = BuildGroup(symbol, interval);
        await Groups.AddToGroupAsync(Context.ConnectionId, group);
    }

    public async Task UnsubscribeKlines(string symbol, string interval)
    {
        var group = BuildGroup(symbol, interval);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, group);
    }

    private static string BuildGroup(string symbol, string interval)
        => $"{symbol.ToLowerInvariant()}@{interval.ToLowerInvariant()}";
}
