using DomainLayer.Entities;

namespace ApplicationLayer.Interfaces.Binance;

public interface ICryptocurrencyRepository
{
    Task<Cryptocurrency> GetBySymbolAsync(string symbol, CancellationToken cancellationToken = default);
}