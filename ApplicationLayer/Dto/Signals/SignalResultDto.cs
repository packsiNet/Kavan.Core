using ApplicationLayer.Common.Enums;

namespace ApplicationLayer.Dto.Signals
{
    public class SignalResultDto
    {
        public Guid SignalId { get; set; } = Guid.NewGuid();

        public string Symbol { get; set; } = string.Empty;

        public string TimeFrame { get; set; } = string.Empty;

        public DateTime Timestamp { get; set; }

        public string SignalType { get; set; } = string.Empty; // Buy/Sell

        public SignalStrength Strength { get; set; } = SignalStrength.Weak;

        public double Confidence { get; set; } // 0-100

        public string Explanation { get; set; } = string.Empty;

        public List<string> MatchedConditions { get; set; } = new();

        // Types of matched evaluators (e.g., structure, mss_break)
        public List<string> MatchedTypes { get; set; } = new();

        // Codes of matched detailed signals (e.g., 1001 for IchimokuCloudBreakoutUp)
        public List<int> MatchedDetailedTypes { get; set; } = new();

        // Arbitrary attributes aggregated from matched evaluators
        public Dictionary<string, string> Attributes { get; set; } = new();
    }
}