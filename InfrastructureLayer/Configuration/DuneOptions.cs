namespace InfrastructureLayer.Configuration
{
    public class DuneOptions
    {
        public bool Enabled { get; set; } = true;
        public int IntervalHours { get; set; } = 6;
        public string BaseUrl { get; set; } = "https://api.dune.com/api/v1";
        public string ApiKey { get; set; } = string.Empty;
        public int QueryId { get; set; } = 6344827;
        public int Limit { get; set; } = 1;
    }
}
