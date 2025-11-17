using DomainLayer.Entities;

namespace ApplicationLayer.Interfaces.Services.Candles;

public interface ICandlesQueryService
{
    Task<List<CandleBase>> GetBeforeAsync(
        int cryptocurrencyId,
        string timeframe,
        DateTime pivotCloseTime,
        int count,
        CancellationToken ct = default);

    Task<List<CandleBase>> GetAfterAsync(
        int cryptocurrencyId,
        string timeframe,
        DateTime pivotCloseTime,
        int count,
        CancellationToken ct = default);
}