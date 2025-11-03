using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ApplicationLayer.Common.Enums;
using ApplicationLayer.Dto.Signals;
using ApplicationLayer.Interfaces;
using ApplicationLayer.Interfaces.Services.Signals;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureLayer.BusinessLogic.Services.Signals.Evaluators
{
    [InjectAsScoped]
    public class StructureEvaluator : IConditionEvaluator
    {
        private readonly IRepository<Cryptocurrency> _crypto;
        private readonly IRepository<Candle_1m> _c1m;
        private readonly IRepository<Candle_5m> _c5m;
        private readonly IRepository<Candle_1h> _c1h;
        private readonly IRepository<Candle_4h> _c4h;
        private readonly IRepository<Candle_1d> _c1d;

        public StructureEvaluator(
            IRepository<Cryptocurrency> crypto,
            IRepository<Candle_1m> c1m,
            IRepository<Candle_5m> c5m,
            IRepository<Candle_1h> c1h,
            IRepository<Candle_4h> c4h,
            IRepository<Candle_1d> c1d)
        {
            _crypto = crypto; _c1m = c1m; _c5m = c5m; _c1h = c1h; _c4h = c4h; _c1d = c1d;
        }

        public string Type => "structure";

        public async Task<ConditionEvaluationResult> EvaluateAsync(string symbol, string timeframe, ConditionNodeDto condition, CancellationToken ct)
        {
            var candles = await GetRecentCandlesAsync(symbol, timeframe, 30, ct);
            if (candles.Count < 5)
                return new ConditionEvaluationResult { Matched = false, Explanation = "Insufficient data for structure" };

            var last = candles[0];
            // Optional parameters
            var lookbackBars = 5;
            if (condition.Parameters != null && condition.Parameters.TryGetValue("lookback_bars", out var lbObj))
            {
                if (int.TryParse(lbObj?.ToString(), out var lb) && lb > 1 && lb <= 30)
                    lookbackBars = lb;
            }

            var priceSource = "close";
            if (condition.Parameters != null && condition.Parameters.TryGetValue("break_source", out var srcObj))
            {
                var src = srcObj?.ToString()?.ToLowerInvariant();
                if (src == "high" || src == "close") priceSource = src;
            }

            var prevHigh = candles.Skip(1).Take(lookbackBars).Max(c => c.High);
            var prevLow = candles.Skip(1).Take(lookbackBars).Min(c => c.Low);

            var direction = condition.Parameters != null && condition.Parameters.TryGetValue("break_direction", out var dirObj)
                ? dirObj?.ToString()?.ToLowerInvariant()
                : null;

            bool upBreak = priceSource == "high" ? last.High > prevHigh : last.Close > prevHigh;
            bool downBreak = priceSource == "high" ? last.Low < prevLow : last.Close < prevLow;

            var matched = direction switch
            {
                "up" => upBreak,
                "down" => downBreak,
                _ => upBreak || downBreak
            };

            var desc = matched
                ? (upBreak ? "Resistance broken" : "Support broken")
                : "No structure break";

            var details = new System.Collections.Generic.Dictionary<string, string>();
            if (matched)
            {
                details["break_direction"] = upBreak ? "up" : "down";
                // Provide a detailed signal code for downstream consumers
                details["detail_code"] = (upBreak ? DetailedSignalType.ResistanceBreakout.Value : DetailedSignalType.SupportBreakdown.Value).ToString();
                details["prev_high"] = prevHigh.ToString();
                details["prev_low"] = prevLow.ToString();
                details["last_close"] = last.Close.ToString();
                details["last_high"] = last.High.ToString();
                details["last_low"] = last.Low.ToString();
                details["break_source"] = priceSource;
                details["lookback_bars"] = lookbackBars.ToString();
            }

            return new ConditionEvaluationResult
            {
                Matched = matched,
                ScoreContribution = matched ? 1.0 : 0.0,
                Explanation = desc,
                MatchedConditions = matched ? new() { condition.Description ?? Type } : new(),
                Type = Type,
                Details = details
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
}