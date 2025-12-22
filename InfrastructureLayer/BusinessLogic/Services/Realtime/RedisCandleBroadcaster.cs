using ApplicationLayer.Dto.Candle;
using ApplicationLayer.Interfaces.Services;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;

namespace InfrastructureLayer.BusinessLogic.Services.Realtime;

public class RedisCandleBroadcaster : ICandleBroadcaster
{
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<RedisCandleBroadcaster> _logger;
    private const string ChannelName = "candles-realtime";

    public RedisCandleBroadcaster(IConnectionMultiplexer redis, ILogger<RedisCandleBroadcaster> logger)
    {
        _redis = redis;
        _logger = logger;
    }

    public async Task BroadcastCandleAsync(string symbol, string timeframe, CandleDto candle, CancellationToken ct = default)
    {
        try
        {
            var message = new CandleBroadcastMessage(symbol, timeframe, candle);
            var json = JsonSerializer.Serialize(message);
            
            var db = _redis.GetDatabase();
            // Fire and forget to avoid blocking
            _ = db.PublishAsync(RedisChannel.Literal(ChannelName), json, CommandFlags.FireAndForget);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to broadcast candle via Redis for {Symbol}", symbol);
            // We don't throw to avoid disrupting the ingestion flow
        }
    }
}
