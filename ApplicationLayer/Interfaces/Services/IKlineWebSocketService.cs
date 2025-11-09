using DomainLayer.Entities;

namespace ApplicationLayer.Interfaces.Services;

public interface IKlineWebSocketService
{
    /// <summary>
    /// Connects to Binance combined kline streams for the given symbol across
    /// 1m, 5m, 1h, 4h, and 1d intervals and persists only final (closed) candles.
    /// The method is resilient to disconnects and will auto-reconnect until canceled.
    /// </summary>
    /// <param name="cryptocurrency">The cryptocurrency entity (Id and Symbol are used).</param>
    /// <param name="cancellationToken">Cancellation token to stop streaming.</param>
    Task RunStreamsForSymbolAsync(Cryptocurrency cryptocurrency, CancellationToken cancellationToken);
}