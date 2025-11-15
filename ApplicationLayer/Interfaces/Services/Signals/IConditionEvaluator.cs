using ApplicationLayer.Dto.Signals;

namespace ApplicationLayer.Interfaces.Services.Signals
{
    public interface IConditionEvaluator
    {
        string Type { get; }

        Task<ConditionEvaluationResult> EvaluateAsync(string symbol, string timeframe, ConditionNodeDto condition, CancellationToken ct);
    }

    public sealed class ConditionEvaluationResult
    {
        public bool Matched { get; set; }

        public double ScoreContribution { get; set; }

        public string Explanation { get; set; } = string.Empty;

        public List<string> MatchedConditions { get; set; } = new();

        // Enables carrying evaluator-specific information to be surfaced in SignalResult
        public string Type { get; set; } = string.Empty;

        public Dictionary<string, string> Details { get; set; } = new();
    }
}