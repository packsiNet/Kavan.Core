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
    }
}