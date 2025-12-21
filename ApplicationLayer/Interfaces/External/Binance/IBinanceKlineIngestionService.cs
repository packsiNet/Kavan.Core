namespace ApplicationLayer.Interfaces.External.Binance;

public interface IBinanceKlineIngestionService
{
    Task RunAsync(CancellationToken cancellationToken);
}

