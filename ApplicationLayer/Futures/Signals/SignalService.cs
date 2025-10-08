using ApplicationLayer.Dto.Signals;
using ApplicationLayer.Interfaces;
using ApplicationLayer.Interfaces.Indicators;
using ApplicationLayer.Interfaces.Signals;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ApplicationLayer.Futures.Signals;

public class SignalService(
    IUnitOfWork _unitOfWork,
    IIndicatorService _indicatorService,
    IRepository<Signal> _signals,
    IRepository<Cryptocurrency> _cryptos,
    IRepository<Candle_1m> _candles_1m,
    IRepository<Candle_5m> _candles_5m,
    IRepository<Candle_1h> _candles_1h,
    IRepository<Candle_4h> _candles_4h,
    IRepository<Candle_1d> _candles_1d,
    IConfiguration _configuration,
    ILogger<SignalService> _logger
) : ISignalService
{
    private readonly ILogger<SignalService> _log = _logger;
    private readonly string[] TimeFrames =
        (_configuration.GetSection("Trading:TimeFrames").Get<string[]>() ?? new[] { "1m", "5m", "1h", "4h", "1d" });

    private readonly int BreakoutLookback =
        int.TryParse(_configuration["Trading:BreakoutLookback"], out var bl) ? bl : 20;
    private readonly int EmaFastPeriod =
        int.TryParse(_configuration["Trading:EmaFastPeriod"], out var ef) ? ef : 20;
    private readonly int EmaSlowPeriod =
        int.TryParse(_configuration["Trading:EmaSlowPeriod"], out var es) ? es : 50;
    private readonly decimal BreakTrendThreshold =
        decimal.TryParse(_configuration["Trading:BreakTrendThreshold"], out var bt) ? bt : 0.0005m;

    public async Task<List<SignalDto>> GenerateSignalsAsync(string symbol, CancellationToken cancellationToken = default)
    {
        var crypto = await _cryptos.Query().FirstOrDefaultAsync(c => c.Symbol == symbol, cancellationToken);
        if (crypto == null) return new List<SignalDto>();

        var results = new List<SignalDto>();

        foreach (var tf in TimeFrames)
        {
            try
            {
                // محاسبه اندیکاتورها
                var rsi = await _indicatorService.ComputeRsiAsync(symbol, tf, 14, cancellationToken);
                var ema20 = await _indicatorService.ComputeEmaAsync(symbol, tf, EmaFastPeriod, cancellationToken);
                var ema50 = await _indicatorService.ComputeEmaAsync(symbol, tf, EmaSlowPeriod, cancellationToken);
                var macd = await _indicatorService.ComputeMacdAsync(symbol, tf, 12, 26, 9, cancellationToken);

                if (rsi == null || ema20 == null || ema50 == null || macd == null)
                    continue;

                var (lastClose, lastTs) = await LoadLastCloseAsync(crypto.Id, tf, cancellationToken);
                if (lastClose == null || lastTs == default) continue;

                // --- Pivot-based Trend Breakout ---
                var breakoutSignal = await DetectTrendBreakoutAsync(crypto.Id, tf, lastClose.Value, cancellationToken);

                // --- BreakTrend ---
                var breakTrendSignal = await DetectBreakTrendAsync(crypto.Id, tf, lastClose.Value, cancellationToken);

                // --- EMA Cross ---
                var emaCrossSignal = await DetectEmaCrossAsync(crypto.Id, tf, cancellationToken);

                // --- MACD Cross ---
                var macdCrossSignal = await DetectMacdCrossAsync(crypto.Id, tf, 12, 26, 9, cancellationToken);

                // --- RSI ---
                string? rsiSignal = null;
                if (rsi.Value <= 30m) rsiSignal = "BUY";
                else if (rsi.Value >= 70m) rsiSignal = "SELL";

                // --- Compose final signal with priority ---
                string? signalType = breakoutSignal
                    ?? breakTrendSignal
                    ?? emaCrossSignal
                    ?? macdCrossSignal
                    ?? rsiSignal;

                string strategy =
                    breakoutSignal != null ? "Breakout" :
                    breakTrendSignal != null ? "TrendLine" :
                    emaCrossSignal != null ? "EMA" :
                    macdCrossSignal != null ? "MACD" :
                    rsiSignal != null ? "RSI" : string.Empty;

                if (signalType == null) continue;

                // جلوگیری از تکراری بودن سیگنال
                var exists = await _signals.Query()
                    .AnyAsync(s => s.CryptocurrencyId == crypto.Id && s.TimeFrame == tf && s.Timestamp == lastTs, cancellationToken);
                if (exists) continue;

                var entity = new Signal
                {
                    CryptocurrencyId = crypto.Id,
                    Symbol = symbol,
                    TimeFrame = tf,
                    SignalType = signalType,
                    Strategy = strategy,
                    Timestamp = lastTs,
                    Rsi = rsi.Value,
                    Ema = ema20.Value,
                    Macd = macd.Value
                };

                await _signals.AddAsync(entity);
                results.Add(new SignalDto
                {
                    Symbol = symbol,
                    TimeFrame = tf,
                    SignalType = signalType,
                    Strategy = strategy,
                    Timestamp = lastTs,
                    Rsi = rsi.Value,
                    Ema = ema20.Value,
                    Macd = macd.Value
                });
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error generating signal for {Symbol} {TF}.", symbol, tf);
            }
        }

        if (results.Count > 0)
            await _unitOfWork.SaveChangesAsync(cancellationToken);

        return results;
    }

    public async Task<List<SignalDto>> GetSignalsAsync(string? symbol, string? timeFrame, int? limit, CancellationToken cancellationToken = default)
    {
        var query = _signals.Query().AsQueryable();

        if (!string.IsNullOrWhiteSpace(symbol))
            query = query.Where(s => s.Symbol == symbol);

        if (!string.IsNullOrWhiteSpace(timeFrame))
            query = query.Where(s => s.TimeFrame == timeFrame);

        query = query.OrderByDescending(s => s.Timestamp);
        if (limit.HasValue && limit.Value > 0)
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
        var query = _signals.Query().AsQueryable();

        if (!string.IsNullOrWhiteSpace(symbol))
            query = query.Where(s => s.Symbol == symbol);

        if (!string.IsNullOrWhiteSpace(timeFrame))
            query = query.Where(s => s.TimeFrame == timeFrame);

        if (!string.IsNullOrWhiteSpace(strategy))
            query = query.Where(s => s.Strategy == strategy);

        query = query.OrderByDescending(s => s.Timestamp);
        if (limit.HasValue && limit.Value > 0)
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

    // =========================
    // Pivot-based Trend Breakout
    // =========================
    // شناسایی Pivot High/Low با Lookback مشخص
    private static (List<int> swingHighs, List<int> swingLows) DetectPivots(
        List<(decimal high, decimal low, decimal close)> candles,
        int lookback)
    {
        var swingHighs = new List<int>();
        var swingLows = new List<int>();

        for (int i = lookback; i < candles.Count - lookback; i++)
        {
            var mid = candles[i];
            bool isHigh = true;
            bool isLow = true;
            for (int j = i - lookback; j <= i + lookback; j++)
            {
                if (candles[j].high > mid.high) isHigh = false;
                if (candles[j].low < mid.low) isLow = false;
            }
            if (isHigh) swingHighs.Add(i);
            if (isLow) swingLows.Add(i);
        }

        return (swingHighs, swingLows);
    }

    // رسم خط روند با استفاده از Pivotها (Regression در صورت وجود >=3 Pivot، وگرنه با دو Pivot آخر)
    private static (decimal slope, decimal intercept)? BuildTrendLine(
        List<int> pivots,
        List<(decimal high, decimal low, decimal close)> candles,
        bool useHighs)
    {
        if (pivots.Count < 2) return null;

        // اگر بیش از دو Pivot داریم، از رگرسیون خطی استفاده می‌کنیم
        if (pivots.Count >= 3)
        {
            var xs = pivots.Select(i => (decimal)i).ToList();
            var ys = pivots.Select(i => useHighs ? candles[i].high : candles[i].low).ToList();

            var xMean = xs.Average();
            var yMean = ys.Average();
            decimal cov = 0m, varX = 0m;
            for (int k = 0; k < xs.Count; k++)
            {
                var dx = xs[k] - xMean;
                var dy = ys[k] - yMean;
                cov += dx * dy;
                varX += dx * dx;
            }
            var slope = varX == 0m ? 0m : cov / varX;
            var intercept = yMean - slope * xMean;
            return (slope, intercept);
        }
        else
        {
            var i1 = pivots[^2];
            var i2 = pivots[^1];
            var y1 = useHighs ? candles[i1].high : candles[i1].low;
            var y2 = useHighs ? candles[i2].high : candles[i2].low;
            var slope = (y2 - y1) / (i2 - i1);
            var intercept = y2 - slope * i2;
            return (slope, intercept);
        }
    }

    // شمارش برخوردهای قبلی به خط روند با تلرانس مشخص
    private static int CountLineTouches(
        (decimal slope, decimal intercept) line,
        List<(decimal high, decimal low, decimal close)> candles,
        int startIndex,
        int endIndex,
        bool isResistance,
        decimal tolerance)
    {
        int touches = 0;
        for (int i = Math.Max(0, startIndex); i <= Math.Min(endIndex, candles.Count - 1); i++)
        {
            var y = line.slope * i + line.intercept;
            var price = isResistance ? candles[i].high : candles[i].low;
            if (Math.Abs(price - y) <= tolerance)
            {
                touches++;
            }
        }
        return touches;
    }

    private async Task<string?> DetectTrendBreakoutAsync(int cryptoId, string tf, decimal lastClose, CancellationToken ct)
    {
        var candles = await LoadRecentCandlesAsync(cryptoId, tf, BreakoutLookback * 3, ct);
        if (candles.Count < BreakoutLookback * 3) return null;

        int pivotLookback = BreakoutLookback;
        var (swingHighs, swingLows) = DetectPivots(candles, pivotLookback);

        // حداقل دو Pivot برای هر سوی روند
        var highLine = BuildTrendLine(swingHighs, candles, useHighs: true);
        var lowLine = BuildTrendLine(swingLows, candles, useHighs: false);

        int lastIndex = candles.Count - 1;
        var tolerance = Math.Max(BreakTrendThreshold * lastClose, lastClose * 0.0005m);

        bool validBreakHigh = false;
        if (highLine.HasValue)
        {
            var y = highLine.Value.slope * lastIndex + highLine.Value.intercept;
            var touches = CountLineTouches(highLine.Value, candles, 0, lastIndex - 1, isResistance: true, tolerance);
            validBreakHigh = lastClose > y && touches >= 2;
        }

        bool validBreakLow = false;
        if (lowLine.HasValue)
        {
            var y = lowLine.Value.slope * lastIndex + lowLine.Value.intercept;
            var touches = CountLineTouches(lowLine.Value, candles, 0, lastIndex - 1, isResistance: false, tolerance);
            validBreakLow = lastClose < y && touches >= 2;
        }

        if (validBreakHigh) return "BUY";
        if (validBreakLow) return "SELL";
        return null;
    }

    // =========================
    // BreakTrend Logic
    // =========================
    private async Task<string?> DetectBreakTrendAsync(int cryptoId, string tf, decimal lastClose, CancellationToken ct)
    {
        var closes = await LoadClosesSeriesAsync(cryptoId, tf, EmaSlowPeriod + 2, ct);
        if (closes.Count < EmaSlowPeriod + 2) return null;

        var ema20_prev = ComputeEmaLocal(closes.Take(closes.Count - 1).ToList(), EmaFastPeriod);
        var ema50_prev = ComputeEmaLocal(closes.Take(closes.Count - 1).ToList(), EmaSlowPeriod);
        var ema20_now = ComputeEmaLocal(closes, EmaFastPeriod);
        var ema50_now = ComputeEmaLocal(closes, EmaSlowPeriod);

        var slope20 = ema20_now - ema20_prev;
        var slope50 = ema50_now - ema50_prev;
        var threshold = Math.Max(BreakTrendThreshold * lastClose, 0.0001m);

        if (slope20 > threshold && ema20_now > ema50_now && slope50 >= -threshold) return "BUY";
        if (slope20 < -threshold && ema20_now < ema50_now && slope50 <= threshold) return "SELL";

        return null;
    }

    // =========================
    // EMA Cross Logic (20/50)
    // =========================
    private async Task<string?> DetectEmaCrossAsync(int cryptoId, string tf, CancellationToken ct)
    {
        var count = Math.Max(EmaSlowPeriod + 1, EmaFastPeriod + 1);
        var closes = await LoadClosesSeriesAsync(cryptoId, tf, count, ct);
        if (closes.Count < count) return null;

        var prevSeries = closes.Take(closes.Count - 1).ToList();
        var ema20_prev = ComputeEmaLocal(prevSeries, EmaFastPeriod);
        var ema50_prev = ComputeEmaLocal(prevSeries, EmaSlowPeriod);
        var ema20_now = ComputeEmaLocal(closes, EmaFastPeriod);
        var ema50_now = ComputeEmaLocal(closes, EmaSlowPeriod);

        if (ema20_prev <= ema50_prev && ema20_now > ema50_now) return "BUY";
        if (ema20_prev >= ema50_prev && ema20_now < ema50_now) return "SELL";
        return null;
    }

    // =========================
    // MACD Cross Logic (MACD vs Signal)
    // =========================
    private async Task<string?> DetectMacdCrossAsync(int cryptoId, string tf, int fastPeriod, int slowPeriod, int signalPeriod, CancellationToken ct)
    {
        int minCount = slowPeriod + signalPeriod;
        var closes = await LoadClosesSeriesAsync(cryptoId, tf, minCount, ct);
        if (closes.Count < minCount) return null;

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

        var macdSeries = new List<decimal>();
        for (int i = slowPeriod; i <= closes.Count; i++)
        {
            var window = closes.Take(i).ToList();
            var emaFast = Ema(window, fastPeriod);
            var emaSlow = Ema(window, slowPeriod);
            macdSeries.Add(emaFast - emaSlow);
        }

        if (macdSeries.Count < signalPeriod + 1) return null;

        var macd_now = macdSeries.Last();
        var macd_prev = macdSeries[macdSeries.Count - 2];
        var signal_now = Ema(macdSeries, signalPeriod);
        var signal_prev = Ema(macdSeries.Take(macdSeries.Count - 1).ToList(), signalPeriod);

        if (macd_prev <= signal_prev && macd_now > signal_now) return "BUY";
        if (macd_prev >= signal_prev && macd_now < signal_now) return "SELL";
        return null;
    }

    // =========================
    // متدهای کمکی EMA/MACD و بارگذاری کندل‌ها
    // =========================
    private static decimal ComputeEmaLocal(List<decimal> series, int period)
    {
        if (series.Count < period) return series.LastOrDefault();

        var m = 2m / (period + 1m);
        decimal ema = series.Take(period).Average();
        for (int i = period; i < series.Count; i++)
        {
            ema = ((series[i] - ema) * m) + ema;
        }
        return ema;
    }

    private async Task<List<decimal>> LoadClosesSeriesAsync(int cryptoId, string tf, int count, CancellationToken ct)
    {
        async Task<List<decimal>> FetchAsync<T>(IRepository<T> repo) where T : CandleBase
        {
            var rows = await repo.Query()
                .Where(c => c.CryptocurrencyId == cryptoId)
                .OrderByDescending(c => c.OpenTime)
                .Take(count)
                .Select(c => c.Close)
                .ToListAsync(ct);
            rows.Reverse();
            return rows;
        }

        return tf switch
        {
            "1m" => await FetchAsync(_candles_1m),
            "5m" => await FetchAsync(_candles_5m),
            "1h" => await FetchAsync(_candles_1h),
            "4h" => await FetchAsync(_candles_4h),
            "1d" => await FetchAsync(_candles_1d),
            _ => new List<decimal>()
        };
    }

    private async Task<List<(decimal high, decimal low, decimal close)>> LoadRecentCandlesAsync(int cryptoId, string tf, int count, CancellationToken ct)
    {
        async Task<List<(decimal high, decimal low, decimal close)>> FetchAsync<T>(IRepository<T> repo) where T : CandleBase
        {
            var rows = await repo.Query()
                .Where(c => c.CryptocurrencyId == cryptoId)
                .OrderByDescending(c => c.OpenTime)
                .Take(count)
                .Select(c => new { c.High, c.Low, c.Close })
                .ToListAsync(ct);
            rows.Reverse();
            return rows.Select(r => (r.High, r.Low, r.Close)).ToList();
        }

        return tf switch
        {
            "1m" => await FetchAsync(_candles_1m),
            "5m" => await FetchAsync(_candles_5m),
            "1h" => await FetchAsync(_candles_1h),
            "4h" => await FetchAsync(_candles_4h),
            "1d" => await FetchAsync(_candles_1d),
            _ => new List<(decimal high, decimal low, decimal close)>()
        };
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
}