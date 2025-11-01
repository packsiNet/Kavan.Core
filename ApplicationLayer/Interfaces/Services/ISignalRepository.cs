using DomainLayer.Entities;

namespace ApplicationLayer.Interfaces.Services;

public interface ISignalRepository
{
    Task<List<TradingSignal>> GetActiveSignalsAsync(
        string? symbol = null,
        string? timeframe = null,
        string? signalType = null,
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default);

    Task<TradingSignal?> GetSignalByIdAsync(string signalId, CancellationToken cancellationToken = default);
    Task<List<TradingSignal>> GetSignalsBySymbolAsync(string symbol, CancellationToken cancellationToken = default);
    Task SaveSignalAsync(TradingSignal signal, CancellationToken cancellationToken = default);
    Task UpdateSignalAsync(TradingSignal signal, CancellationToken cancellationToken = default);
    Task DeactivateSignalAsync(string signalId, CancellationToken cancellationToken = default);
    Task<bool> SignalExistsAsync(string symbol, string signalType, string timeframe, CancellationToken cancellationToken = default);
}