using System.Threading;
using System.Threading.Tasks;

using ApplicationLayer.Dto.External.Dune;

namespace ApplicationLayer.Interfaces.External
{
    public interface IDuneClient
    {
        Task<DuneQueryResultDto<T>> GetQueryResultsAsync<T>(int queryId, int limit, CancellationToken cancellationToken);
    }
}
