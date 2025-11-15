namespace ApplicationLayer.Dto.Signals
{
    public class SignalDto
    {
        public int Id { get; set; }

        public string Symbol { get; set; }

        public string Timeframe { get; set; }

        public DateTime SignalTime { get; set; }

        public string SignalCategory { get; set; }

        public string SignalName { get; set; }

        public int Direction { get; set; }

        public decimal BreakoutLevel { get; set; }

        public decimal NearestResistance { get; set; }

        public decimal NearestSupport { get; set; }

        public decimal PivotR1 { get; set; }

        public decimal PivotR2 { get; set; }

        public decimal PivotR3 { get; set; }

        public decimal PivotS1 { get; set; }

        public decimal PivotS2 { get; set; }

        public decimal PivotS3 { get; set; }

        public decimal Atr { get; set; }

        public decimal Tolerance { get; set; }

        public decimal VolumeRatio { get; set; }

        public decimal BodySize { get; set; }

        public CandleDto LastCandle { get; set; }

        public List<CandleDto> Candles { get; set; } = new();
    }

    public class CandleDto
    {
        public int Index { get; set; }

        public DateTime OpenTime { get; set; }

        public DateTime CloseTime { get; set; }

        public decimal Open { get; set; }

        public decimal High { get; set; }

        public decimal Low { get; set; }

        public decimal Close { get; set; }

        public decimal Volume { get; set; }
    }
}