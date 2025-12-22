using ApplicationLayer.Dto.Candle;
using Microsoft.AspNetCore.SignalR;

namespace Kavan.Api.Hubs;

public interface ICandleClient
{
    Task CandleUpdated(CandleDto candle);
}

public class CandleHub : Hub<ICandleClient>
{
    public static string BuildGroupName(string symbol, string timeframe) 
        => $"candle:{symbol.ToUpper()}:{timeframe}";

    public async Task Subscribe(string symbol, string timeframe)
    {
        var groupName = BuildGroupName(symbol, timeframe);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
    }

    public async Task Unsubscribe(string symbol, string timeframe)
    {
        var groupName = BuildGroupName(symbol, timeframe);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
    }
}
