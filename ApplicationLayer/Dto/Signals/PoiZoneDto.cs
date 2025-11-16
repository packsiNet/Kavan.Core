namespace ApplicationLayer.Dto.Signals
{
    public class PoiZoneDto
    {
        public string Name { get; set; }

        public string Type { get; set; } // e.g., FVG, OrderBlock, Demand, Supply

        public string Timeframe { get; set; }

        public decimal? PriceLow { get; set; }

        public decimal? PriceHigh { get; set; }

        public decimal? Entry { get; set; }

        public decimal? StopLoss { get; set; }

        public List<decimal> TakeProfits { get; set; } = new();

        public DateTime? ValidUntil { get; set; }

        public Dictionary<string, string> Attributes { get; set; } = new();
    }
}