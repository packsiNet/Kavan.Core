namespace ApplicationLayer.Dto.Signals
{
    public class UserPreferencesDto
    {
        public string Risk_Level { get; set; } // low/medium/high

        public string Strategy_Type { get; set; } // price_action, etc.

        public string Signal_Strength { get; set; } // desired: weak/medium/strong
    }
}