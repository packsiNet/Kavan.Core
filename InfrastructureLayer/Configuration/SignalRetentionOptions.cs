namespace InfrastructureLayer.Configuration
{
    public class SignalRetentionOptions
    {
        public bool Enabled { get; set; } = true;
        public int IntervalHours { get; set; } = 6;
        public int RetentionDays_1m { get; set; } = 7;
        public int RetentionDays_5m { get; set; } = 14;
        public int RetentionDays_1h { get; set; } = 90;
        public int RetentionDays_4h { get; set; } = 180;
        public int RetentionDays_1d { get; set; } = 365;
        public List<string> ExcludeCategories { get; set; } = new();
        public List<string> ExcludeSignalNames { get; set; } = new();
    }
}