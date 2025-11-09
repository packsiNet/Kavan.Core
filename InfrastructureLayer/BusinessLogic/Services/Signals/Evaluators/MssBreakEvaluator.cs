using ApplicationLayer.Dto.Signals;
using ApplicationLayer.Interfaces;
using ApplicationLayer.Interfaces.Services.Signals;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureLayer.BusinessLogic.Services.Signals.Evaluators;

[InjectAsScoped]
public class MssBreakEvaluator(
    IRepository<Cryptocurrency> crypto,
    IRepository<Candle_1h> c1h,
    IRepository<Candle_4h> c4h) : IConditionEvaluator
{
    public string Type => "mss_break";

    public async Task<ConditionEvaluationResult> EvaluateAsync(string symbol, string timeframe, ConditionNodeDto condition, CancellationToken ct)
    {
        var cryptoId = await crypto.Query().Where(c => c.Symbol == symbol).Select(c => c.Id).FirstOrDefaultAsync(ct);
        IQueryable<CandleBase> baseQuery = timeframe == "4h"
            ? c4h.Query().Cast<CandleBase>()
            : c1h.Query().Cast<CandleBase>();

        baseQuery = baseQuery.Where(c => c.CryptocurrencyId == cryptoId).OrderByDescending(c => c.CloseTime);
        var candles = await baseQuery.Take(20).ToListAsync(ct);
        if (candles.Count < 10)
            return new ConditionEvaluationResult { Matched = false, Explanation = "Insufficient data for MSS" };

        // Simple MSS: consecutive higher lows and higher highs then a break above previous swing high
        var swingHigh = candles.Skip(1).Take(8).Max(c => c.High);
        var last = candles[0];
        var matched = last.Close > swingHigh;

        return new ConditionEvaluationResult
        {
            Matched = matched,
            ScoreContribution = matched ? 1.2 : 0.0,
            Explanation = matched ? "MSS confirmed with break" : "No MSS break",
            MatchedConditions = matched ? new() { condition.Description ?? Type } : new(),
            Type = Type,
            Details = new() { { "swing_high", swingHigh.ToString() }, { "last_close", last.Close.ToString() } }
        };
    }
}