using ApplicationLayer.Dto.Candle;
using Kavan.Api.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;

namespace Kavan.Api.Services.Realtime;

public class CandleRedisConsumer : BackgroundService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IHubContext<CandleHub, ICandleClient> _hubContext;
    private readonly ILogger<CandleRedisConsumer> _logger;
    private const string ChannelName = "candles-realtime";

    public CandleRedisConsumer(
        IConnectionMultiplexer redis,
        IHubContext<CandleHub, ICandleClient> hubContext,
        ILogger<CandleRedisConsumer> logger)
    {
        _redis = redis;
        _hubContext = hubContext;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting Candle Redis Consumer...");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (!_redis.IsConnected)
                {
                    _logger.LogWarning("Redis is not connected. Retrying in 5s...");
                    await Task.Delay(5000, stoppingToken);
                    continue;
                }

                var subscriber = _redis.GetSubscriber();

                await subscriber.SubscribeAsync(RedisChannel.Literal(ChannelName), async (channel, message) =>
                {
                    try
                    {
                        var data = JsonSerializer.Deserialize<CandleBroadcastMessage>(message);
                        if (data == null) return;

                        _logger.LogDebug("Received candle for {Symbol} {Timeframe}: Close={Close}", data.Symbol, data.Timeframe, data.Candle.Close);

                        var groupName = CandleHub.BuildGroupName(data.Symbol, data.Timeframe);
                        await _hubContext.Clients.Group(groupName).CandleUpdated(data.Candle);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing Redis candle message");
                    }
                });

                _logger.LogInformation("Successfully subscribed to Redis channel: {Channel}", ChannelName);
                
                // If we get here, we are subscribed. Now just wait until cancelled or connection lost.
                // Note: StackExchange.Redis handles reconnection automatically, so we don't need to re-subscribe manually usually.
                // But if the initial subscription failed, we need this loop.
                // If connection drops and reconnects, SE.Redis restores subscriptions.
                
                // Block here to keep the service alive
                await Task.Delay(-1, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to subscribe to Redis channel. Retrying in 5s...");
                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}
