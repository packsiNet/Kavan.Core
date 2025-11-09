using ApplicationLayer.Interfaces;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;

namespace InfrastructureLayer.BusinessLogic.Services.Candles;

[InjectAsScoped]
public class CandleSeedService(
    IRepository<Cryptocurrency> cryptos,
    IUnitOfWork uow) : ICandleSeedService
{
    public async Task SeedSymbolAsync()
    {
        var btc = cryptos.Query().FirstOrDefault(c => c.Symbol == "BTCUSDT");
        if (btc == null)
        {
            btc = new Cryptocurrency
            {
                Symbol = "BTCUSDT",
                BaseAsset = "BTC",
                QuoteAsset = "USDT"
            };
            await cryptos.AddAsync(btc);
            await uow.SaveChangesAsync();
        }

        var eth = cryptos.Query().FirstOrDefault(c => c.Symbol == "ETHUSDT");
        if (eth == null)
        {
            eth = new Cryptocurrency
            {
                Symbol = "ETHUSDT",
                BaseAsset = "ETH",
                QuoteAsset = "USDT"
            };
            await cryptos.AddAsync(eth);
            await uow.SaveChangesAsync();
        }

        await uow.CommitAsync();
    }
}