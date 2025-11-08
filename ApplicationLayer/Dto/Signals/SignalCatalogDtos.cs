using System;
using System.Collections.Generic;

namespace ApplicationLayer.Dto.Signals
{
    public class SignalCategoryDto
    {
        public string Category { get; set; }
        public List<SignalTypeDto> Types { get; set; } = new();
    }

    public class SignalTypeDto
    {
        public string Name { get; set; }
        public int Count { get; set; }
    }

    public class SignalSummaryDto
    {
        public int Id { get; set; }
        public string Symbol { get; set; }
        public string Timeframe { get; set; }
        public DateTime SignalTime { get; set; }
        public string SignalName { get; set; }
        public int Direction { get; set; }
        public decimal BreakoutLevel { get; set; }
    }
}