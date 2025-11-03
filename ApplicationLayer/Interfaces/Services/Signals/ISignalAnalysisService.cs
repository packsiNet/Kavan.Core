using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ApplicationLayer.Dto.Signals;

namespace ApplicationLayer.Interfaces.Services.Signals
{
    public interface ISignalAnalysisService
    {
        Task<IReadOnlyList<SignalResultDto>> AnalyzeAsync(SignalRequestDto request, CancellationToken cancellationToken);
    }
}