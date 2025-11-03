using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ApplicationLayer.Dto.Signals;
using ApplicationLayer.Interfaces;
using ApplicationLayer.Interfaces.Services.Signals;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureLayer.BusinessLogic.Services.Signals.Evaluators
{
    [InjectAsScoped]
    public class SupportLevelEvaluator : IConditionEvaluator
    {
        private readonly IRepository<Cryptocurrency> _crypto;
        private readonly IRepository<Candle_1h> _c1h;
        private readonly IRepository<Candle_4h> _c4h;

        public SupportLevelEvaluator(
            IRepository<Cryptocurrency> crypto,
            IRepository<Candle_1h> c1h,
            IRepository<Candle_4h> c4h)
        { _crypto = crypto; _c1h = c1h; _c4h = c4h; }

        public string Type => "support_level";

        public async Task<ConditionEvaluationResult> EvaluateAsync(string symbol, string timeframe, ConditionNodeDto condition, CancellationToken ct)
        {
            var cryptoId = await _crypto.Query().Where(c => c.Symbol == symbol).Select(c => c.Id).FirstOrDefaultAsync(ct);
            IQueryable<CandleBase> baseQuery = timeframe == "4h"
                ? _c4h.Query().Cast<CandleBase>()
                : _c1h.Query().Cast<CandleBase>();

            baseQuery = baseQuery.Where(c => c.CryptocurrencyId == cryptoId).OrderByDescending(c => c.CloseTime);
            var candles = await baseQuery.Take(7).ToListAsync(ct);
            if (candles.Count < 5)
                return new ConditionEvaluationResult { Matched = false, Explanation = "Insufficient data for support" };

            var c2 = candles.ElementAtOrDefault(2);
            var isFloor = c2 != null && c2.Low < candles[1].Low && c2.Low < candles[3].Low;

            return new ConditionEvaluationResult
            {
                Matched = isFloor,
                ScoreContribution = isFloor ? 0.7 : 0.0,
                Explanation = isFloor ? "Local floor formed" : "No floor",
                MatchedConditions = isFloor ? new() { condition.Description ?? Type } : new(),
                Type = Type,
                Details = isFloor ? new() { { "pivot_index", "2" } } : new()
            };
        }
    }
}