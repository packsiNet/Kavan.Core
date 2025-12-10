using ApplicationLayer.Dto.News;

namespace ApplicationLayer.Interfaces.Services.News;

public interface INewsSyncService
{
    Task<int> SyncLatestAsync(CryptoPanicQuery query, CancellationToken cancellationToken);
}
