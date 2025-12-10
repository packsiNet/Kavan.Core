using ApplicationLayer.Dto.News;

namespace ApplicationLayer.Interfaces.External;

public interface ICryptoPanicClient
{
    Task<IReadOnlyList<CryptoPanicPost>> GetPostsAsync(CryptoPanicQuery query, CancellationToken cancellationToken);
}
