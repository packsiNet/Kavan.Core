namespace ApplicationLayer.Dto.Signals
{
    public class FilterOptionsDto
    {
        public double? Volume_Min { get; set; }
        public string Volatility { get; set; } // e.g., low/medium/high
        public double? Liquidity { get; set; }
        public decimal? Price_Min { get; set; }
        public decimal? Price_Max { get; set; }

        // Control applying filters before/after condition evaluation
        public bool ApplyBefore { get; set; } = true;
    }
}