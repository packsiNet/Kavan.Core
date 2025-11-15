namespace ApplicationLayer.Dto.Breakouts
{
    public class BreakoutScanRequestDto
    {
        public List<string> Symbols { get; set; } = new();

        public List<string> Timeframes { get; set; } = new();

        public int LookbackPeriod { get; set; } = 100;

        public bool AutoWindow { get; set; } = false;
    }
}