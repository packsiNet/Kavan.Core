namespace ApplicationLayer.Dto.Signals
{
    public class SignalRequestDto
    {
        public string Market { get; set; } // e.g., "crypto"

        public List<string> Symbols { get; set; } = [];

        public List<string> Exclude { get; set; } = [];

        public List<string> Timeframes { get; set; } = [];

        public List<ConditionNodeDto> Conditions { get; set; } = [];

        public List<GroupNodeDto> Groups { get; set; } = [];

        public FilterOptionsDto Filters { get; set; }

        public UserPreferencesDto Preferences { get; set; }

        public string StrategyText { get; set; }
    }
}