using ApplicationLayer.Dto.Signals;
using ApplicationLayer.Interfaces;
using ApplicationLayer.Interfaces.Services.Signals;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureLayer.BusinessLogic.Services.Signals.Evaluators;

[InjectAsScoped]
public class FvgEntryEvaluator(
    IRepository<Cryptocurrency> crypto,
    IRepository<Candle_1m> c1m,
    IRepository<Candle_5m> c5m,
    IRepository<Candle_1h> c1h,
    IRepository<Candle_4h> c4h,
    IRepository<Candle_1d> c1d) : IConditionEvaluator
{
    private readonly IRepository<Cryptocurrency> _crypto = crypto;
    private readonly IRepository<Candle_1m> _c1m = c1m;
    private readonly IRepository<Candle_5m> _c5m = c5m;
    private readonly IRepository<Candle_1h> _c1h = c1h;
    private readonly IRepository<Candle_4h> _c4h = c4h;
    private readonly IRepository<Candle_1d> _c1d = c1d;

    public string Type => "fvg_entry";

    public async Task<ConditionEvaluationResult> EvaluateAsync(string symbol, string timeframe, ConditionNodeDto condition, CancellationToken ct)
    {
        var candles = await GetRecentCandlesAsync(symbol, timeframe, 10, ct);
        if (candles.Count < 3)
            return new ConditionEvaluationResult { Matched = false, Explanation = "Insufficient data for FVG" };

        // Simple FVG: High[1] < Low[3] for bearish gap OR Low[1] > High[3] for bullish gap
        var c0 = candles[0]; // latest
        var c1 = candles[1];
        var c2 = candles[2];

        bool bullishGap = c1.Low > c2.High;
        bool bearishGap = c1.High < c2.Low;

        // Entry: price enters the gap (current close is inside the gap range)
        bool entersBullishGap = bullishGap && c0.Close >= c2.High && c0.Close <= c1.Low;
        bool entersBearishGap = bearishGap && c0.Close <= c2.Low && c0.Close >= c1.High;

        var matched = entersBullishGap || entersBearishGap;
        var dir = entersBullishGap ? "bullish FVG entry" : entersBearishGap ? "bearish FVG entry" : "no FVG entry";

        return new ConditionEvaluationResult
        {
            Matched = matched,
            ScoreContribution = matched ? 0.8 : 0.0,
            Explanation = dir,
            MatchedConditions = matched ? new() { condition.Description ?? Type } : new()
        };
    }

    private async Task<List<CandleBase>> GetRecentCandlesAsync(string symbol, string timeframe, int take, CancellationToken ct)
    {
        var cryptoId = await _crypto.Query().Where(c => c.Symbol == symbol).Select(c => c.Id).FirstOrDefaultAsync(ct);
        IQueryable<CandleBase> query = timeframe switch
        {
            "1m" => _c1m.Query().Where(c => c.CryptocurrencyId == cryptoId).OrderByDescending(c => c.CloseTime),
            "5m" => _c5m.Query().Where(c => c.CryptocurrencyId == cryptoId).OrderByDescending(c => c.CloseTime),
            "1h" => _c1h.Query().Where(c => c.CryptocurrencyId == cryptoId).OrderByDescending(c => c.CloseTime),
            "4h" => _c4h.Query().Where(c => c.CryptocurrencyId == cryptoId).OrderByDescending(c => c.CloseTime),
            "1d" => _c1d.Query().Where(c => c.CryptocurrencyId == cryptoId).OrderByDescending(c => c.CloseTime),
            _ => _c1h.Query().Where(c => c.CryptocurrencyId == cryptoId).OrderByDescending(c => c.CloseTime)
        };
        return await query.Take(take).Cast<CandleBase>().ToListAsync(ct);
    }
}