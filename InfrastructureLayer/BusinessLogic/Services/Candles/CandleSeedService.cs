using ApplicationLayer.Interfaces;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;

namespace InfrastructureLayer.BusinessLogic.Services.Candles
{
    [InjectAsScoped]
    public class CandleSeedService : ICandleSeedService
    {
        private readonly IRepository<Cryptocurrency> _cryptos;
        private readonly IRepository<Candle_1d> _c1d;
        private readonly IRepository<Candle_5m> _c5m;
        private readonly IRepository<Candle_1m> _c1m;
        private readonly IUnitOfWork _uow;

        public CandleSeedService(
            IRepository<Cryptocurrency> cryptos,
            IRepository<Candle_1d> c1d,
            IRepository<Candle_5m> c5m,
            IRepository<Candle_1m> c1m,
            IUnitOfWork uow)
        {
            _cryptos = cryptos;
            _c1d = c1d;
            _c5m = c5m;
            _c1m = c1m;
            _uow = uow;
        }

        public async Task SeedSymbolAsync()
        {
            var btc = _cryptos.Query().FirstOrDefault(c => c.Symbol == "BTCUSDT");
            if (btc == null)
            {
                btc = new Cryptocurrency
                {
                    Symbol = "BTCUSDT",
                    BaseAsset = "BTC",
                    QuoteAsset = "USDT"
                };
                await _cryptos.AddAsync(btc);
                await _uow.SaveChangesAsync();
            }

            var eth = _cryptos.Query().FirstOrDefault(c => c.Symbol == "ETHUSDT");
            if (eth == null)
            {
                eth = new Cryptocurrency
                {
                    Symbol = "ETHUSDT",
                    BaseAsset = "ETH",
                    QuoteAsset = "USDT"
                };
                await _cryptos.AddAsync(eth);
                await _uow.SaveChangesAsync();
            }

            await _uow.CommitAsync();
        }
    }
}