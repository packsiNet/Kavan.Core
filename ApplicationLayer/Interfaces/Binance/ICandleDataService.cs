using ApplicationLayer.Dto.Cabdles;
using DomainLayer.Entities;

namespace ApplicationLayer.Interfaces.Binance;

public interface ICandleDataService
{
    Task<IEnumerable<Candle_1m>> GetCandlesAsync(string symbol, string interval, int limit = 500);
}