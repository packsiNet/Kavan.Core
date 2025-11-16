namespace ApplicationLayer.Dto.Signals
{
    public class StrategyParseResultDto
    {
        public List<ConditionNodeDto> Conditions { get; set; } = new();

        public List<GroupNodeDto> Groups { get; set; } = new();
    }
}