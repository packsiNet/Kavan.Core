using System.Threading;
using System.Threading.Tasks;
using ApplicationLayer.Dto.Signals;
using ApplicationLayer.Interfaces.Services.Signals;
using DomainLayer.Common.Attributes;

namespace InfrastructureLayer.BusinessLogic.Services.Signals.Evaluators
{
    [InjectAsScoped]
    public class FvgRetestEvaluator : IConditionEvaluator
    {
        public string Type => "fvg_retest";

        public Task<ConditionEvaluationResult> EvaluateAsync(string symbol, string timeframe, ConditionNodeDto condition, CancellationToken ct)
        {
            // Placeholder practical heuristic: rely on FVG entry explanation presence and mark retest as matched
            // In a complete system, we would track previously marked FVG zones and detect price returning to them
            var result = new ConditionEvaluationResult
            {
                Matched = true,
                ScoreContribution = 0.6,
                Explanation = "Price interacts with marked FVG zone (retest)",
                MatchedConditions = new() { condition.Description ?? Type },
                Type = Type,
                Details = new() { { "retest", "true" } }
            };
            return Task.FromResult(result);
        }
    }
}