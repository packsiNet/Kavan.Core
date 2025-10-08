using ApplicationLayer.Interfaces;
using ApplicationLayer.Interfaces.Binance;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureLayer.BusinessLogic.Services.Binance;

[InjectAsScoped]
public class BinanceSymbolSyncService(IRepository<Cryptocurrency> _repository) : IBinanceSymbolSyncService
{
    public async Task<List<string>> GetActiveSymbolsAsync()
    {
        return await _repository.Query()
            .Where(x => x.IsActive)
            .Select(x => x.Symbol)
            .ToListAsync();
    }
}