namespace ApplicationLayer.Interfaces.Services.External;

public interface IDuneSyncService
{
    Task<int> SyncAsync(CancellationToken cancellationToken);
}
