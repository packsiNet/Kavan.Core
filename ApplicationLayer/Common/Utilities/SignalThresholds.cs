namespace ApplicationLayer.Common.Utilities
{
    public class SignalThresholds
    {
        // Defaults per prompt, configurable via DI/options later
        public double WeakMax { get; set; } = 40.0;

        public double MediumMax { get; set; } = 70.0;

        // Weights can be adjusted based on strategy/user preferences
        public double BaseConditionWeight { get; set; } = 1.0;

        public double ConfirmationBonusWeight { get; set; } = 0.5;

        public double FilterPenaltyWeight { get; set; } = 1.0;

        public double MacroWeight { get; set; } = 2.0;

        public double MicroWeight { get; set; } = 1.0;

        public double PatternWeight { get; set; } = 1.5;
    }

    public static class SignalThresholdsExtensions
    {
        public static decimal ComputeTolerance(decimal lastClose, decimal atr, string timeframe)
        {
            var pct = 0.0025m;
            var atrFactor = 0.25m;
            return Math.Max(lastClose * pct, atr * atrFactor);
        }
    }
}