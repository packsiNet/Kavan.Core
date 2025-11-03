using ApplicationLayer.Interfaces;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;

namespace InfrastructureLayer.BusinessLogic.Services.Candles;

[InjectAsScoped]
public class CandleSeedService(
    IUnitOfWork unitOfWork,
    IRepository<Cryptocurrency> cryptoRepository,
    IRepository<Candle_1m> candle1mRepository
) : ICandleSeedService
{
    private readonly IUnitOfWork _uow = unitOfWork;
    private readonly IRepository<Cryptocurrency> _cryptoRepo = cryptoRepository;
    private readonly IRepository<Candle_1m> _candle1mRepo = candle1mRepository;

    public async Task<int> SeedBTCUSDT_1m_DoubleTopBreakoutAsync(int count = 100, DateTime? startTime = null, CancellationToken cancellationToken = default)
    {
        // Ensure cryptocurrency exists
        var crypto = _cryptoRepo.Query().FirstOrDefault(x => x.Symbol == "BTCUSDT");
        if (crypto is null)
        {
            crypto = new Cryptocurrency
            {
                Symbol = "BTCUSDT",
                BaseAsset = "BTC",
                QuoteAsset = "USDT"
            };
            await _cryptoRepo.AddAsync(crypto);
            await _uow.SaveChangesAsync(cancellationToken);
        }

        // Clear existing 1m candles for this symbol for determinism
        var toRemove = _candle1mRepo.GetDbSet().Where(c => c.CryptocurrencyId == crypto.Id).ToList();
        if (toRemove.Count > 0)
        {
            _candle1mRepo.RemoveRange(toRemove);
            await _uow.SaveChangesAsync(cancellationToken);
        }

        var start = startTime ?? DateTime.UtcNow.AddMinutes(-count);

        var candles = new List<Candle_1m>(count);

        // Pattern parameters
        decimal basePrice = 48000m; // starting close
        decimal resistance = 50000m; // equal highs (resistance)

        decimal close = basePrice;

        for (int i = 0; i < count; i++)
        {
            var openTime = start.AddMinutes(i);
            var closeTime = openTime.AddMinutes(1);

            decimal open;
            decimal high;
            decimal low;
            decimal volume;

            if (i < 30)
            {
                // Gradual uptrend towards resistance
                var step = 65m; // ~1.95k over 30 minutes
                open = close;
                close = Math.Round(basePrice + (i + 1) * step, 2);
                high = Math.Max(open, close) + 30m;
                low = Math.Min(open, close) - 30m;
                volume = 250m + i * 5m;
            }
            else if (i == 30)
            {
                // First equal peak, rejection below resistance
                open = close;
                high = resistance;
                close = resistance - 150m;
                low = close - 120m;
                volume = 900m;
            }
            else if (i > 30 && i < 46)
            {
                // Pullback
                open = close;
                close = Math.Round(close - 20m, 2);
                high = open + 20m;
                low = close - 35m;
                volume = 350m;
            }
            else if (i >= 46 && i < 60)
            {
                // Climb back up to resistance
                open = close;
                close = Math.Round(close + 50m, 2);
                high = close + 25m;
                low = open - 20m;
                volume = 400m + (i - 46) * 10m;
            }
            else if (i == 60)
            {
                // Second equal peak, rejection again
                open = close;
                high = resistance; // nearly equal high
                close = resistance - 120m;
                low = close - 100m;
                volume = 1000m;
            }
            else if (i > 60 && i < 89)
            {
                // Tight consolidation under resistance with higher lows
                open = close;
                var bias = 8m; // slow grind up
                close = Math.Round(close + bias, 2);
                high = close + 20m;
                low = open - 20m;
                volume = 500m + (i - 60) * 6m;
            }
            else if (i == 89)
            {
                // Breakout candle: strong bullish candle through resistance
                open = Math.Round(resistance - 80m, 2);
                high = Math.Round(50500m, 2);
                close = Math.Round(50350m, 2);
                low = Math.Round(open - 30m, 2);
                volume = 2500m; // spike volume
            }
            else
            {
                // Post-breakout follow-through
                open = close;
                close = Math.Round(close + 22m, 2);
                high = close + 30m;
                low = open - 25m;
                volume = 900m + (i - 89) * 20m;
            }

            // Ensure bounds are consistent
            var hi = Math.Max(Math.Max(open, close), high);
            var lo = Math.Min(Math.Min(open, close), low);
            high = Math.Round(hi, 2);
            low = Math.Round(lo, 2);

            candles.Add(new Candle_1m
            {
                CryptocurrencyId = crypto.Id,
                OpenTime = openTime,
                CloseTime = closeTime,
                Open = Math.Round(open, 2),
                High = high,
                Low = low,
                Close = Math.Round(close, 2),
                Volume = Math.Round(volume, 2)
            });
        }

        await _candle1mRepo.AddRangeAsync(candles);
        await _uow.SaveChangesAsync(cancellationToken);

        return candles.Count;
    }
}