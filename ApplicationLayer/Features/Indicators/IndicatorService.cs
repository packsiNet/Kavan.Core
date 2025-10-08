using ApplicationLayer.Dto.Indicators;
using ApplicationLayer.Interfaces;
using ApplicationLayer.Interfaces.Indicators;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApplicationLayer.Features.Indicators;

public class IndicatorService(
    IUnitOfWork _unitOfWork,
    IRepository<Cryptocurrency> _cryptos,
    IRepository<Candle_1m> _candles_1m,
    IRepository<Candle_5m> _candles_5m,
    IRepository<Candle_1h> _candles_1h,
    IRepository<Candle_4h> _candles_4h,
    IRepository<Candle_1d> _candles_1d
) : IIndicatorService
{
    public async Task<IndicatorResult?> ComputeRsiAsync(string symbol, string timeFrame, int period, CancellationToken cancellationToken = default)
    {
        if (period < 2)
            throw new ArgumentException("RSI period must be >= 2", nameof(period));

        var (closes, lastTs) = await LoadClosesAsync(symbol, timeFrame, period + 1, cancellationToken);
        if (closes.Count < period + 1)
            return null;

        decimal gainSum = 0m;
        decimal lossSum = 0m;
        for (int i = 1; i < closes.Count; i++)
        {
            var change = closes[i] - closes[i - 1];
            if (change > 0) gainSum += change;
            else lossSum += -change;
        }

        var avgGain = gainSum / period;
        var avgLoss = lossSum / period;

        decimal rsi;
        if (avgLoss == 0)
        {
            rsi = 100m;
        }
        else
        {
            var rs = avgGain / avgLoss;
            rsi = 100m - (100m / (1m + rs));
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new IndicatorResult
        {
            Symbol = symbol,
            TimeFrame = timeFrame,
            Value = Math.Round(rsi, 4),
            Timestamp = lastTs
        };
    }

    public async Task<IndicatorResult?> ComputeEmaAsync(string symbol, string timeFrame, int period, CancellationToken cancellationToken = default)
    {
        if (period < 2)
            throw new ArgumentException("EMA period must be >= 2", nameof(period));

        var (closes, lastTs) = await LoadClosesAsync(symbol, timeFrame, period, cancellationToken);
        if (closes.Count < period)
            return null;

        var multiplier = 2m / (period + 1m);

        // SMA seed
        decimal ema = closes.Take(period).Average();

        // If we only have exactly 'period' values, EMA equals SMA for the last.
        // To be robust, we apply EMA across all items (including seed)
        for (int i = period; i < closes.Count; i++)
        {
            ema = ((closes[i] - ema) * multiplier) + ema;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new IndicatorResult
        {
            Symbol = symbol,
            TimeFrame = timeFrame,
            Value = Math.Round(ema, 4),
            Timestamp = lastTs
        };
    }

    public async Task<IndicatorResult?> ComputeMacdAsync(string symbol, string timeFrame, int fastPeriod, int slowPeriod, int signalPeriod, CancellationToken cancellationToken = default)
    {
        if (fastPeriod < 2 || slowPeriod <= fastPeriod || signalPeriod < 2)
            throw new ArgumentException("Invalid MACD parameters");

        // Minimum data to compute slow EMA and signal EMA on MACD line
        int minCount = slowPeriod + signalPeriod;
        var (closes, lastTs) = await LoadClosesAsync(symbol, timeFrame, minCount, cancellationToken);
        if (closes.Count < minCount)
            return null;

        decimal Ema(List<decimal> series, int period)
        {
            var m = 2m / (period + 1m);
            decimal ema = series.Take(period).Average();
            for (int i = period; i < series.Count; i++)
            {
                ema = ((series[i] - ema) * m) + ema;
            }
            return ema;
        }

        // Build MACD line values for signal EMA
        // For signal EMA we need a series of MACD values; approximate by computing
        // MACD at each step where both EMAs are defined.
        var macdSeries = new List<decimal>();
        for (int i = slowPeriod; i <= closes.Count; i++)
        {
            // Window up to i
            var window = closes.Take(i).ToList();
            var emaFast = Ema(window, fastPeriod);
            var emaSlow = Ema(window, slowPeriod);
            macdSeries.Add(emaFast - emaSlow);
        }

        if (macdSeries.Count < signalPeriod)
            return null;

        var macd = macdSeries.Last();
        var signal = Ema(macdSeries, signalPeriod);
        var histogram = macd - signal; // if needed later

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new IndicatorResult
        {
            Symbol = symbol,
            TimeFrame = timeFrame,
            Value = Math.Round(macd, 4),
            Timestamp = lastTs
        };
    }

    private async Task<(List<decimal> closes, DateTime lastTs)> LoadClosesAsync(string symbol, string timeFrame, int minCount, CancellationToken ct)
    {
        var crypto = await _cryptos.Query().FirstOrDefaultAsync(c => c.Symbol == symbol, ct);
        if (crypto == null)
            return (new List<decimal>(), default);

        async Task<(List<decimal> closes, DateTime lastTs)> FetchAsync<T>(IRepository<T> repo) where T : CandleBase
        {
            var rows = await repo.Query()
                .Where(c => c.CryptocurrencyId == crypto.Id)
                .OrderByDescending(c => c.OpenTime)
                .Take(minCount)
                .Select(c => new { c.Close, c.CloseTime })
                .ToListAsync(ct);

            if (rows.Count == 0)
                return (new List<decimal>(), default);

            rows.Reverse();
            return (rows.Select(r => r.Close).ToList(), rows.Last().CloseTime);
        }

        return timeFrame switch
        {
            "1m" => await FetchAsync(_candles_1m),
            "5m" => await FetchAsync(_candles_5m),
            "1h" => await FetchAsync(_candles_1h),
            "4h" => await FetchAsync(_candles_4h),
            "1d" => await FetchAsync(_candles_1d),
            _ => (new List<decimal>(), default)
        };
    }
}