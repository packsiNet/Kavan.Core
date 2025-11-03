using System.Collections.Generic;

namespace ApplicationLayer.Dto.Signals
{
    public class ConditionNodeDto
    {
        public string? Type { get; set; } // e.g., structure, fvg_entry, mss_break
        public string? Indicator { get; set; }
        public string? Description { get; set; }
        public Dictionary<string, object>? Parameters { get; set; }
        public string? Timeframe { get; set; }
        public string? LogicalOperator { get; set; } // AND/OR used when combining confirmations

        public List<ConfirmationDto> Confirmation { get; set; } = new();
        public bool ConfirmationRequired { get; set; }
    }

    public class ConfirmationDto
    {
        public string? Type { get; set; }
        public string? Condition { get; set; }
        public bool Required { get; set; }
    }

    public class GroupNodeDto
    {
        public string? Operator { get; set; } // AND/OR
        public string? Timeframe { get; set; }
        public string? Description { get; set; }
        public List<ConditionNodeDto> Conditions { get; set; } = new();
        public List<GroupNodeDto> Groups { get; set; } = new();
    }
}