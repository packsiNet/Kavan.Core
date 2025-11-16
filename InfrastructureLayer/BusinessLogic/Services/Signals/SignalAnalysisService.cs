using ApplicationLayer.Common.Enums;
using ApplicationLayer.Common.Utilities;
using ApplicationLayer.Dto.Signals;
using ApplicationLayer.Interfaces;
using ApplicationLayer.Interfaces.Services.Signals;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureLayer.BusinessLogic.Services.Signals;

[InjectAsScoped]
public class SignalAnalysisService(
    IRepository<Cryptocurrency> cryptoRepo,
    IRepository<Candle_1m> c1m,
    IRepository<Candle_5m> c5m,
    IRepository<Candle_1h> c1h,
    IRepository<Candle_4h> c4h,
    IRepository<Candle_1d> c1d,
    IConditionEvaluatorFactory factory,
    IStrategyTextParser strategyTextParser = null,
    SignalThresholds thresholds = null) : ISignalAnalysisService
{
    private readonly IRepository<Cryptocurrency> _cryptoRepo = cryptoRepo;
    private readonly IRepository<Candle_1m> _c1m = c1m;
    private readonly IRepository<Candle_5m> _c5m = c5m;
    private readonly IRepository<Candle_1h> _c1h = c1h;
    private readonly IRepository<Candle_4h> _c4h = c4h;
    private readonly IRepository<Candle_1d> _c1d = c1d;
    private readonly IConditionEvaluatorFactory _factory = factory;
    private readonly IStrategyTextParser _strategyTextParser = strategyTextParser;
    private readonly SignalThresholds _thresholds = thresholds ?? new SignalThresholds();

    public async Task<IReadOnlyList<SignalResultDto>> AnalyzeAsync(SignalRequestDto request, CancellationToken cancellationToken)
    {
        var results = new List<SignalResultDto>();
        var symbols = await ResolveSymbolsAsync(request, cancellationToken);
        var timeframes = request.Timeframes is { Count: > 0 } ? request.Timeframes : new List<string> { "1h" };

        var parsed = (!string.IsNullOrWhiteSpace(request.StrategyText) && _strategyTextParser != null)
            ? _strategyTextParser.Parse(request.StrategyText)
            : new StrategyParseResultDto();

        var conditions = (request.Conditions?.Count > 0 ? request.Conditions : parsed.Conditions) ?? new List<ConditionNodeDto>();
        var groups = (request.Groups?.Count > 0 ? request.Groups : parsed.Groups) ?? new List<GroupNodeDto>();

        timeframes = timeframes.OrderByDescending(GetTfRank).ToList();

        foreach (var symbol in symbols)
        {
            foreach (var tf in timeframes)
            {
                if (request.Filters?.ApplyBefore == true)
                {
                    var pass = await PassesFiltersAsync(symbol, tf, request.Filters, cancellationToken);
                    if (!pass) continue;
                }

                var evals = new List<ConditionEvaluationResult>();

                if (conditions is { Count: > 0 })
                {
                    foreach (var cond in conditions)
                    {
                        var ctf = string.IsNullOrWhiteSpace(cond.Timeframe) ? tf : cond.Timeframe;
                        cond.Parameters ??= new Dictionary<string, object>();
                        cond.Parameters["macro_context"] = macroContext.GetValueOrDefault(symbol);
                        var r = await EvaluateConditionAsync(symbol, ctf, cond, cancellationToken);
                        if (r.Matched) evals.Add(r);
                    }
                }

                if (groups is { Count: > 0 })
                {
                    foreach (var group in groups)
                    {
                        var gtf = string.IsNullOrWhiteSpace(group.Timeframe) ? tf : group.Timeframe;
                        var gr = await EvaluateGroupAsync(symbol, gtf, group, cancellationToken);
                        if (gr.Matched) evals.Add(gr);
                    }
                }

                if (evals.Count == 0) continue;

                var score = ComputeWeightedScore(evals);

                if (request.Filters != null && !request.Filters.ApplyBefore)
                {
                    var post = await PassesFiltersAsync(symbol, tf, request.Filters, cancellationToken);
                    if (!post) score -= _thresholds.FilterPenaltyWeight;
                }

                var last = (await GetRecentCandlesAsync(symbol, tf, 1, cancellationToken)).LastOrDefault();
                var explanation = BuildExplanation(evals);
                var attributes = MergeAttributes(evals);
                var detailed = ExtractDetailedTypes(evals);
                var signalType = InferSignalType(evals);
                var zones = ExtractZones(evals);

                var confidence = Math.Min(100.0, Math.Max(0.0, (score / _thresholds.MediumMax) * 100.0));
                var matchedTypes = evals.Select(e => e.Type).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList();
                var matchedConds = evals.SelectMany(e => e.MatchedConditions).ToList();
                var strength = SignalStrength.FromScore(score, _thresholds.WeakMax, _thresholds.MediumMax);

                if (!string.IsNullOrWhiteSpace(request?.Preferences?.Signal_Strength))
                {
                    var desired = request.Preferences.Signal_Strength.Trim().ToLowerInvariant();
                    var ok = desired switch
                    {
                        "strong" => strength.Equals(SignalStrength.Strong),
                        "medium" => strength.Equals(SignalStrength.Medium) || strength.Equals(SignalStrength.Strong),
                        "weak" => true,
                        _ => true
                    };
                    if (!ok) continue;
                }

                results.Add(new SignalResultDto
                {
                    Symbol = symbol,
                    TimeFrame = tf,
                    Timestamp = last?.CloseTime ?? DateTime.UtcNow,
                    SignalType = signalType,
                    Strength = strength,
                    Confidence = confidence,
                    Explanation = explanation,
                    MatchedConditions = matchedConds,
                    MatchedTypes = matchedTypes,
                    MatchedDetailedTypes = detailed,
                    Attributes = attributes,
                    Zones = zones
                });
            }
        }

        return results;
    }

    private static int GetTfRank(string tf)
        => tf switch { "1d" => 500, "4h" => 400, "1h" => 300, "15m" => 200, "5m" => 150, "1m" => 100, _ => 50 };

    private readonly Dictionary<string, List<ConditionEvaluationResult>> macroContext = new();

    private double ComputeWeightedScore(List<ConditionEvaluationResult> evals)
    {
        double score = 0.0;
        foreach (var e in evals)
        {
            var tf = e.Details != null && e.Details.TryGetValue("timeframe", out var tfv) ? tfv : "";
            var tfWeight = GetTfRank(tf) >= 300 ? _thresholds.MacroWeight : _thresholds.MicroWeight;
            var typeWeight = (e.Type?.Contains("fvg", StringComparison.OrdinalIgnoreCase) == true
                              || e.Type?.Contains("order", StringComparison.OrdinalIgnoreCase) == true
                              || e.Type?.Contains("bos", StringComparison.OrdinalIgnoreCase) == true
                              || e.Type?.Contains("choch", StringComparison.OrdinalIgnoreCase) == true)
                             ? _thresholds.PatternWeight : 1.0;
            score += (e.ScoreContribution + _thresholds.BaseConditionWeight) * tfWeight * typeWeight;
        }
        return score;
    }

    private static List<PoiZoneDto> ExtractZones(IEnumerable<ConditionEvaluationResult> evals)
    {
        var zones = new List<PoiZoneDto>();
        foreach (var e in evals)
        {
            if (e.Details == null || e.Details.Count == 0) continue;
            if (e.Details.TryGetValue("zone_low", out var zl) || e.Details.TryGetValue("zone_high", out var zh) || e.Details.TryGetValue("entry", out var en))
            {
                var z = new PoiZoneDto
                {
                    Name = e.Type,
                    Type = e.Details.GetValueOrDefault("zone_type"),
                    Timeframe = e.Details.GetValueOrDefault("timeframe")
                };
                if (e.Details.TryGetValue("zone_low", out var zlStr) && decimal.TryParse(zlStr, out var zlVal)) z.PriceLow = zlVal;
                if (e.Details.TryGetValue("zone_high", out var zhStr) && decimal.TryParse(zhStr, out var zhVal)) z.PriceHigh = zhVal;
                if (e.Details.TryGetValue("entry", out var enStr) && decimal.TryParse(enStr, out var enVal)) z.Entry = enVal;
                if (e.Details.TryGetValue("sl", out var slStr) && decimal.TryParse(slStr, out var slVal)) z.StopLoss = slVal;
                foreach (var tpKey in new[] { "tp1", "tp2", "tp3" })
                {
                    if (e.Details.TryGetValue(tpKey, out var tpStr) && decimal.TryParse(tpStr, out var tpVal)) z.TakeProfits.Add(tpVal);
                }
                foreach (var kv in e.Details)
                {
                    if (!z.Attributes.ContainsKey(kv.Key)) z.Attributes[kv.Key] = kv.Value;
                }
                zones.Add(z);
            }
        }
        return zones;
    }

    private async Task<List<string>> ResolveSymbolsAsync(SignalRequestDto request, CancellationToken ct)
    {
        var baseQuery = _cryptoRepo.Query();

        var symbols = request.Symbols is { Count: > 0 }
            ? await baseQuery.Where(c => request.Symbols.Contains(c.Symbol)).Select(c => c.Symbol).ToListAsync(ct)
            : await baseQuery.Select(c => c.Symbol).ToListAsync(ct);

        if (request.Exclude is { Count: > 0 })
            symbols = symbols.Where(s => !request.Exclude.Contains(s)).ToList();

        return symbols;
    }

    private async Task<bool> PassesFiltersAsync(string symbol, string timeframe, FilterOptionsDto filters, CancellationToken ct)
    {
        // Example: apply volume min filter against recent candles
        if (filters.Volume_Min.HasValue)
        {
            var candles = await GetRecentCandlesAsync(symbol, timeframe, 50, ct);
            if (candles.Count == 0) return false;
            var avgVol = candles.Average(c => (double)c.Volume);
            if (avgVol < filters.Volume_Min.Value) return false;
        }

        // Volatility filter: crude proxy via average true range percentage
        if (!string.IsNullOrWhiteSpace(filters.Volatility))
        {
            var candles = await GetRecentCandlesAsync(symbol, timeframe, 50, ct);
            if (candles.Count > 0)
            {
                var atrPct = candles.Average(c => (double)((c.High - c.Low) / c.Close * 100m));
                if (filters.Volatility.Equals("low", StringComparison.OrdinalIgnoreCase) && atrPct > 1.5) return false;
                if (filters.Volatility.Equals("medium", StringComparison.OrdinalIgnoreCase) && (atrPct < 0.8 || atrPct > 3.0)) return false;
                if (filters.Volatility.Equals("high", StringComparison.OrdinalIgnoreCase) && atrPct < 2.0) return false;
            }
        }

        // Liquidity and price ranges could be enforced here using market data; skipped if null
        if (filters.Price_Min.HasValue || filters.Price_Max.HasValue)
        {
            var last = (await GetRecentCandlesAsync(symbol, timeframe, 1, ct)).LastOrDefault();
            if (last != null)
            {
                if (filters.Price_Min.HasValue && last.Close < filters.Price_Min.Value) return false;
                if (filters.Price_Max.HasValue && last.Close > filters.Price_Max.Value) return false;
            }
        }

        return true;
    }

    private async Task<List<CandleBase>> GetRecentCandlesAsync(string symbol, string timeframe, int take, CancellationToken ct)
    {
        var cryptoId = await _cryptoRepo.Query().Where(c => c.Symbol == symbol).Select(c => c.Id).FirstOrDefaultAsync(ct);

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

    private async Task<ConditionEvaluationResult> EvaluateConditionAsync(string symbol, string tf, ConditionNodeDto cond, CancellationToken ct)
    {
        var evaluator = _factory.Resolve(cond.Type ?? string.Empty);
        if (evaluator == null)
            return new ConditionEvaluationResult { Matched = false, Explanation = $"No evaluator for type: {cond.Type}" };

        var result = await evaluator.EvaluateAsync(symbol, tf, cond, ct);
        result.Details ??= new Dictionary<string, string>();
        result.Details["timeframe"] = tf;

        // Confirmations handling
        if (cond.ConfirmationRequired && cond.Confirmation.Count > 0)
        {
            var confirmations = new List<bool>();
            foreach (var conf in cond.Confirmation)
            {
                var confEval = _factory.Resolve(conf.Type ?? string.Empty);
                if (confEval == null)
                {
                    confirmations.Add(!conf.Required); // treat as optional not failing if evaluator missing
                    continue;
                }
                var confResult = await confEval.EvaluateAsync(symbol, tf, new ConditionNodeDto { Type = conf.Type, Description = conf.Condition }, ct);
                confirmations.Add(confResult.Matched || !conf.Required);
            }

            var merged = cond.LogicalOperator?.Equals("OR", StringComparison.OrdinalIgnoreCase) == true
                ? confirmations.Any(x => x)
                : confirmations.All(x => x);

            result.Matched = result.Matched && merged;
            if (merged)
                result.ScoreContribution += _thresholds.ConfirmationBonusWeight;
        }

        return result;
    }

    private async Task<ConditionEvaluationResult> EvaluateGroupAsync(string symbol, string tf, GroupNodeDto group, CancellationToken ct)
    {
        var results = new List<ConditionEvaluationResult>();
        foreach (var c in group.Conditions)
            results.Add(await EvaluateConditionAsync(symbol, tf, c, ct));
        foreach (var g in group.Groups)
            results.Add(await EvaluateGroupAsync(symbol, tf, g, ct));

        var opIsOr = group.Operator?.Equals("OR", StringComparison.OrdinalIgnoreCase) == true;
        var matched = opIsOr ? results.Any(r => r.Matched) : results.All(r => r.Matched);
        var score = results.Sum(r => r.ScoreContribution);
        var explanation = BuildExplanation(results);

        return new ConditionEvaluationResult
        {
            Matched = matched,
            ScoreContribution = score,
            Explanation = explanation,
            MatchedConditions = results.SelectMany(r => r.MatchedConditions).ToList(),
            Type = string.IsNullOrWhiteSpace(group.Description) ? $"group:{(group.Operator ?? "AND")}" : group.Description,
            Details = new() { { "group_operator", group.Operator ?? "AND" }, { "group_timeframe", group.Timeframe ?? "" } }
        };
    }

    private static string BuildExplanation(IEnumerable<ConditionEvaluationResult> evals)
    {
        var matched = evals.Where(e => e.Matched).Select(e => e.Explanation).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
        return matched.Count == 0 ? "No conditions matched" : string.Join(" | ", matched);
    }

    private static string InferSignalType(List<ConditionEvaluationResult> matchedEvals)
    {
        var text = string.Join(" ", matchedEvals.Select(e => e.Explanation).Where(s => !string.IsNullOrWhiteSpace(s))).ToLowerInvariant();
        var bullish = new[] { "break", "up", "support", "floor", "mss" }.Count(k => text.Contains(k));
        var bearish = new[] { "down", "resistance", "ceiling", "reject" }.Count(k => text.Contains(k));
        if (bullish > bearish && bullish > 0) return "Buy";
        if (bearish > bullish && bearish > 0) return "Sell";
        return "None";
    }

    private static Dictionary<string, string> MergeAttributes(IEnumerable<ConditionEvaluationResult> evals)
    {
        var dict = new Dictionary<string, string>();
        foreach (var e in evals)
        {
            if (e.Details == null || e.Details.Count == 0) continue;
            var type = string.IsNullOrWhiteSpace(e.Type) ? "signal" : e.Type;
            foreach (var kv in e.Details)
            {
                var key = $"{type}.{kv.Key}";
                if (!dict.ContainsKey(key))
                    dict[key] = kv.Value;
            }
        }
        return dict;
    }

    private static List<int> ExtractDetailedTypes(IEnumerable<ConditionEvaluationResult> evals)
    {
        var set = new HashSet<int>();
        foreach (var e in evals)
        {
            if (e.Details == null || e.Details.Count == 0) continue;
            if (e.Details.TryGetValue("detail_code", out var codeStr) && int.TryParse(codeStr, out var code))
            {
                set.Add(code);
                continue;
            }
            if (e.Details.TryGetValue("detail_type", out var name) && !string.IsNullOrWhiteSpace(name))
            {
                if (DetailedSignalType.TryFromName(name, out var t) && t != null)
                    set.Add(t.Value);
            }
        }
        return set.ToList();
    }
}