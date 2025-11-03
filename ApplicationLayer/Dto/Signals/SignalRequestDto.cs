using System.Collections.Generic;

namespace ApplicationLayer.Dto.Signals
{
    public class SignalRequestDto
    {
        public string Market { get; set; } // e.g., "crypto"
        public List<string> Symbols { get; set; } = new();
        public List<string> Exclude { get; set; } = new();
        public List<string> Timeframes { get; set; } = new();

        public List<ConditionNodeDto> Conditions { get; set; } = new();
        public List<GroupNodeDto> Groups { get; set; } = new();

        public FilterOptionsDto Filters { get; set; }
        public UserPreferencesDto Preferences { get; set; }
    }
}