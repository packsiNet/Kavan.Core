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

        public async Task<int> SeedBTCUSDT_1m_DoubleTopBreakoutAsync(int count = 100, DateTime? startTime = null, CancellationToken cancellationToken = default)
        {
            if (count < 10) count = 10;

            var anchor = startTime?.ToUniversalTime() ?? DateTime.UtcNow;

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
            var cryptoId = btc.Id;

            var start = anchor.AddMinutes(-count);
            var end = anchor;

            var exists = _c1m.Query().Any(x => x.CryptocurrencyId == cryptoId && x.OpenTime >= start && x.OpenTime < end);
            if (exists)
                return 0;

            var candles = new List<Candle_1m>();
            var rnd = new Random(7);
            decimal price = 68000m;
            for (int i = 0; i < count; i++)
            {
                var openTime = start.AddMinutes(i);
                var closeTime = openTime.AddMinutes(1);
                var open = price + rnd.Next(-30, 30);
                var close = price + rnd.Next(-30, 30);
                var high = Math.Max(open, close) + rnd.Next(10, 40);
                var low = Math.Min(open, close) - rnd.Next(10, 40);
                price = close;

                candles.Add(new Candle_1m
                {
                    CryptocurrencyId = cryptoId,
                    OpenTime = openTime,
                    CloseTime = closeTime,
                    Open = open,
                    High = high,
                    Low = low,
                    Close = close,
                    Volume = rnd.Next(1000, 5000)
                });
            }

            await _uow.BeginTransactionAsync();
            await _c1m.AddRangeAsync(candles);
            await _uow.SaveChangesAsync();
            await _uow.CommitAsync();

            return candles.Count;
        }

        public async Task<int> SeedETHUSDT_MTF_FVG_StructureAsync(int days = 12, int m5Bars = 60, int m1Bars = 120, DateTime? startTime = null, CancellationToken cancellationToken = default)
        {
            if (days < 3) days = 3;
            if (m5Bars < 10) m5Bars = 10;
            if (m1Bars < 10) m1Bars = 10;

            var now = startTime?.ToUniversalTime() ?? DateTime.UtcNow;

            // Ensure ETHUSDT exists
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
            var cryptoId = eth.Id;

            // Seed 1d: create a down-move into support then bullish bounce
            var dStart = now.Date.AddDays(-days);
            var dEnd = dStart.AddDays(days);
            var daily = new List<Candle_1d>();
            var has1d = _c1d.Query().Any(c => c.CryptocurrencyId == cryptoId && c.OpenTime >= dStart && c.OpenTime < dEnd);
            decimal basePrice = 1800m;
            var rnd = new Random(42);
            if (!has1d)
            {
                for (int i = 0; i < days; i++)
                {
                    var openTime = dStart.AddDays(i);
                    var closeTime = openTime.AddDays(1);
                    var drift = i < days - 3 ? -rnd.Next(1, 10) : rnd.Next(5, 25);
                    basePrice = Math.Max(1400m, basePrice + drift);
                    var open = basePrice + rnd.Next(-5, 5);
                    var close = basePrice + rnd.Next(-5, 5);
                    var high = Math.Max(open, close) + rnd.Next(3, 12);
                    var low = Math.Min(open, close) - rnd.Next(3, 12);
                    daily.Add(new Candle_1d
                    {
                        CryptocurrencyId = cryptoId,
                        OpenTime = openTime,
                        CloseTime = closeTime,
                        Open = open,
                        High = high,
                        Low = low,
                        Close = close,
                        Volume = rnd.Next(1000, 5000)
                    });
                }
            }

            // Seed 5m: craft FVG and structure breakout with wick while latest close re-enters gap
            var m5Start = now.AddMinutes(-m5Bars * 5);
            var m5End = m5Start.AddMinutes(m5Bars * 5);
            var m5 = new List<Candle_5m>();
            var has5m = _c5m.Query().Any(c => c.CryptocurrencyId == cryptoId && c.OpenTime >= m5Start && c.OpenTime < m5End);
            decimal p5 = 1500m;
            if (!has5m)
            {
                for (int i = 0; i < m5Bars; i++)
                {
                    var openTime = m5Start.AddMinutes(i * 5);
                    var closeTime = openTime.AddMinutes(5);
                    var open = p5 + rnd.Next(-2, 2);
                    var close = p5 + rnd.Next(-2, 2);
                    var high = Math.Max(open, close) + rnd.Next(1, 4);
                    var low = Math.Min(open, close) - rnd.Next(1, 4);
                    p5 = close;
                    m5.Add(new Candle_5m
                    {
                        CryptocurrencyId = cryptoId,
                        OpenTime = openTime,
                        CloseTime = closeTime,
                        Open = open,
                        High = high,
                        Low = low,
                        Close = close,
                        Volume = rnd.Next(500, 1500)
                    });
                }
            }
            // Overwrite last 3 bars to enforce bullish FVG and breakout-on-wick + entry
            // c2: older bar
            var idx2 = m5Bars - 3; var idx1 = m5Bars - 2; var idx0 = m5Bars - 1;
            var c2High = 1500m; var c2Low = 1494m; var c2Open = 1497m; var c2Close = 1498m;
            if (!has5m)
            {
                m5[idx2] = new Candle_5m
                {
                    CryptocurrencyId = cryptoId,
                    OpenTime = m5[idx2].OpenTime,
                    CloseTime = m5[idx2].CloseTime,
                    Open = c2Open,
                    High = c2High,
                    Low = c2Low,
                    Close = c2Close,
                    Volume = 900
                };
                // c1: breakout candle creating FVG, low > c2.High; moderate high so prevHigh isn’t too large
                var c1Low = 1505m; var c1High = 1514m; var c1Open = 1506m; var c1Close = 1510m;
                m5[idx1] = new Candle_5m
                {
                    CryptocurrencyId = cryptoId,
                    OpenTime = m5[idx1].OpenTime,
                    CloseTime = m5[idx1].CloseTime,
                    Open = c1Open,
                    High = c1High,
                    Low = c1Low,
                    Close = c1Close,
                    Volume = 1800
                };
                // c0: retest/entry candle closing inside gap [c2.High, c1.Low] and wicking above prevHigh
                var c0Close = 1502m; var c0High = 1516m; var c0Low = 1499m; var c0Open = 1501m;
                m5[idx0] = new Candle_5m
                {
                    CryptocurrencyId = cryptoId,
                    OpenTime = m5[idx0].OpenTime,
                    CloseTime = m5[idx0].CloseTime,
                    Open = c0Open,
                    High = c0High,
                    Low = c0Low,
                    Close = c0Close,
                    Volume = 1600
                };
            }

            // Seed 1m: similar micro FVG and breakout-on-wick + entry
            var m1Start = now.AddMinutes(-m1Bars);
            var m1End = m1Start.AddMinutes(m1Bars);
            var m1 = new List<Candle_1m>();
            var has1m = _c1m.Query().Any(c => c.CryptocurrencyId == cryptoId && c.OpenTime >= m1Start && c.OpenTime < m1End);
            decimal p1 = 1510m;
            if (!has1m)
            {
                for (int i = 0; i < m1Bars; i++)
                {
                    var openTime = m1Start.AddMinutes(i);
                    var closeTime = openTime.AddMinutes(1);
                    var open = p1 + rnd.Next(-1, 1);
                    var close = p1 + rnd.Next(-1, 1);
                    var high = Math.Max(open, close) + rnd.Next(1, 3);
                    var low = Math.Min(open, close) - rnd.Next(1, 3);
                    p1 = close;
                    m1.Add(new Candle_1m
                    {
                        CryptocurrencyId = cryptoId,
                        OpenTime = openTime,
                        CloseTime = closeTime,
                        Open = open,
                        High = high,
                        Low = low,
                        Close = close,
                        Volume = rnd.Next(200, 800)
                    });
                }
            }
            // Enforce last 3 bars: c2, c1 create micro FVG, c0 enters and wicks above prevHigh
            var i2 = m1Bars - 3; var i1 = m1Bars - 2; var i0 = m1Bars - 1;
            var m1c2High = 1512m; var m1c2Low = 1508m; var m1c2Open = 1509m; var m1c2Close = 1511m;
            if (!has1m)
            {
                m1[i2] = new Candle_1m
                {
                    CryptocurrencyId = cryptoId,
                    OpenTime = m1[i2].OpenTime,
                    CloseTime = m1[i2].CloseTime,
                    Open = m1c2Open,
                    High = m1c2High,
                    Low = m1c2Low,
                    Close = m1c2Close,
                    Volume = 500
                };
                var m1c1Low = 1515m; var m1c1High = 1522m; var m1c1Open = 1516m; var m1c1Close = 1519m;
                m1[i1] = new Candle_1m
                {
                    CryptocurrencyId = cryptoId,
                    OpenTime = m1[i1].OpenTime,
                    CloseTime = m1[i1].CloseTime,
                    Open = m1c1Open,
                    High = m1c1High,
                    Low = m1c1Low,
                    Close = m1c1Close,
                    Volume = 900
                };
                var m1c0Close = 1517m; var m1c0High = 1524m; var m1c0Low = 1513m; var m1c0Open = 1516m;
                m1[i0] = new Candle_1m
                {
                    CryptocurrencyId = cryptoId,
                    OpenTime = m1[i0].OpenTime,
                    CloseTime = m1[i0].CloseTime,
                    Open = m1c0Open,
                    High = m1c0High,
                    Low = m1c0Low,
                    Close = m1c0Close,
                    Volume = 800
                };
            }

            var inserted = (daily?.Count ?? 0) + (m5?.Count ?? 0) + (m1?.Count ?? 0);
            if (inserted == 0)
                return 0;

            await _uow.BeginTransactionAsync();
            if (daily.Count > 0) await _c1d.AddRangeAsync(daily);
            if (m5.Count > 0) await _c5m.AddRangeAsync(m5);
            if (m1.Count > 0) await _c1m.AddRangeAsync(m1);
            await _uow.SaveChangesAsync();
            await _uow.CommitAsync();

            return inserted;
        }
    }
}