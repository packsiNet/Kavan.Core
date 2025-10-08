using ApplicationLayer.Dto.Signals;

namespace ApplicationLayer.Interfaces.Signals;

public interface ISignalService
{
    Task<List<SignalDto>> GenerateSignalsAsync(string symbol, CancellationToken cancellationToken = default);

    Task<List<SignalDto>> GetSignalsAsync(string? symbol, string? timeFrame, int? limit, CancellationToken cancellationToken = default);
}