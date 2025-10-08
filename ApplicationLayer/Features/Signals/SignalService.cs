using ApplicationLayer.Dto.Signals;
using ApplicationLayer.Interfaces;
using ApplicationLayer.Interfaces.Indicators;
using ApplicationLayer.Interfaces.Signals;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApplicationLayer.Features.Signals;

public class SignalService(
    IUnitOfWork _unitOfWork,
    IIndicatorService _indicatorService,
    IRepository<Signal> _signals,
    IRepository<Cryptocurrency> _cryptos,
    IRepository<Candle_1m> _candles_1m,
    IRepository<Candle_5m> _candles_5m,
    IRepository<Candle_1h> _candles_1h,
    IRepository<Candle_4h> _candles_4h,
    IRepository<Candle_1d> _candles_1d
) : ISignalService
{
    private static readonly string[] TimeFrames = ["1m", "5m", "1h", "4h", "1d"];

    public async Task<List<SignalDto>> GenerateSignalsAsync(string symbol, CancellationToken cancellationToken = default)
    {
        var crypto = await _cryptos.Query().FirstOrDefaultAsync(c => c.Symbol == symbol, cancellationToken);
        if (crypto == null) return [];

        var results = new List<SignalDto>();

        foreach (var tf in TimeFrames)
        {
            // Default parameters
            int rsiPeriod = 14;
            int emaPeriod = 20;
            int macdFast = 12, macdSlow = 26, macdSignal = 9;

            var rsi = await _indicatorService.ComputeRsiAsync(symbol, tf, rsiPeriod, cancellationToken);
            var ema = await _indicatorService.ComputeEmaAsync(symbol, tf, emaPeriod, cancellationToken);
            var macd = await _indicatorService.ComputeMacdAsync(symbol, tf, macdFast, macdSlow, macdSignal, cancellationToken);

            if (rsi == null || ema == null || macd == null)
                continue;

            var (lastClose, lastTs) = await LoadLastCloseAsync(crypto.Id, tf, cancellationToken);
            if (lastClose == null || lastTs == default)
                continue;

            string? signalType = DecideSignalType(rsi.Value, ema.Value, macd.Value, lastClose.Value);
            if (signalType == null)
                continue; // no signal

            // Avoid duplicate per timeframe/timestamp
            var exists = await _signals.AnyAsync(s => s.CryptocurrencyId == crypto.Id && s.TimeFrame == tf && s.Timestamp == lastTs);
            if (exists) continue;

            var entity = new Signal
            {
                CryptocurrencyId = crypto.Id,
                Symbol = symbol,
                TimeFrame = tf,
                SignalType = signalType,
                Strategy = "RSI",
                Timestamp = lastTs,
                Rsi = rsi.Value,
                Ema = ema.Value,
                Macd = macd.Value
            };

            await _signals.AddAsync(entity);
            results.Add(new SignalDto
            {
                Symbol = symbol,
                TimeFrame = tf,
                SignalType = signalType,
                Strategy = "RSI",
                Timestamp = lastTs,
                Rsi = rsi.Value,
                Ema = ema.Value,
                Macd = macd.Value
            });
        }

        if (results.Count > 0)
            await _unitOfWork.SaveChangesAsync(cancellationToken);

        return results;
    }

    public async Task<List<SignalDto>> GetSignalsAsync(string? symbol, string? timeFrame, int? limit, CancellationToken cancellationToken = default)
    {
        var query = _signals.Query();

        if (!string.IsNullOrWhiteSpace(symbol))
            query = query.Where(s => s.Symbol == symbol);

        if (!string.IsNullOrWhiteSpace(timeFrame))
            query = query.Where(s => s.TimeFrame == timeFrame);

        query = query.OrderByDescending(s => s.Timestamp);

        if (limit.HasValue)
            query = query.Take(limit.Value);

        var rows = await query.Select(s => new SignalDto
        {
            Symbol = s.Symbol,
            TimeFrame = s.TimeFrame,
            SignalType = s.SignalType,
            Strategy = s.Strategy,
            Timestamp = s.Timestamp,
            Rsi = s.Rsi,
            Ema = s.Ema,
            Macd = s.Macd
        }).ToListAsync(cancellationToken);

        return rows;
    }

    public async Task<List<SignalDto>> GetSignalsAsync(string? symbol, string? timeFrame, string? strategy, int? limit, CancellationToken cancellationToken = default)
    {
        var query = _signals.Query();

        if (!string.IsNullOrWhiteSpace(symbol))
            query = query.Where(s => s.Symbol == symbol);

        if (!string.IsNullOrWhiteSpace(timeFrame))
            query = query.Where(s => s.TimeFrame == timeFrame);

        if (!string.IsNullOrWhiteSpace(strategy))
            query = query.Where(s => s.Strategy == strategy);

        query = query.OrderByDescending(s => s.Timestamp);

        if (limit.HasValue)
            query = query.Take(limit.Value);

        var rows = await query.Select(s => new SignalDto
        {
            Symbol = s.Symbol,
            TimeFrame = s.TimeFrame,
            SignalType = s.SignalType,
            Strategy = s.Strategy,
            Timestamp = s.Timestamp,
            Rsi = s.Rsi,
            Ema = s.Ema,
            Macd = s.Macd
        }).ToListAsync(cancellationToken);

        return rows;
    }

    private async Task<(decimal? close, DateTime ts)> LoadLastCloseAsync(int cryptoId, string tf, CancellationToken ct)
    {
        async Task<(decimal? close, DateTime ts)> FetchAsync<T>(IRepository<T> repo) where T : CandleBase
        {
            var row = await repo.Query()
                .Where(c => c.CryptocurrencyId == cryptoId)
                .OrderByDescending(c => c.OpenTime)
                .Select(c => new { c.Close, c.CloseTime })
                .FirstOrDefaultAsync(ct);

            if (row == null) return (null, default);
            return (row.Close, row.CloseTime);
        }

        return tf switch
        {
            "1m" => await FetchAsync(_candles_1m),
            "5m" => await FetchAsync(_candles_5m),
            "1h" => await FetchAsync(_candles_1h),
            "4h" => await FetchAsync(_candles_4h),
            "1d" => await FetchAsync(_candles_1d),
            _ => (null, default)
        };
    }

    private static string? DecideSignalType(decimal rsi, decimal ema, decimal macd, decimal lastClose)
    {
        // Simple heuristic combining indicators
        if (rsi <= 30m && lastClose > ema && macd > 0m)
            return "BUY";

        if (rsi >= 70m && lastClose < ema && macd < 0m)
            return "SELL";

        return null; // no clear signal
    }
}