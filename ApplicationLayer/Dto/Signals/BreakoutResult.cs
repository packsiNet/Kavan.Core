using System;

namespace ApplicationLayer.Dto.Signals
{
    public class BreakoutResult
    {
        public string Symbol { get; set; } = string.Empty;
        public string Timeframe { get; set; } = string.Empty;
        public bool IsBreakout { get; set; }
        public BreakoutDirection Direction { get; set; }
        public decimal BreakoutLevel { get; set; }
        public DateTime CandleTime { get; set; }
        public decimal VolumeRatio { get; set; }
    }

    public enum BreakoutDirection
    {
        Up,
        Down
    }
}