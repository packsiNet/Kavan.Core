namespace ApplicationLayer.Interfaces.Services.External;

public interface IDuneGasPriceSyncService
{
    Task<int> SyncAsync(CancellationToken cancellationToken);
}
