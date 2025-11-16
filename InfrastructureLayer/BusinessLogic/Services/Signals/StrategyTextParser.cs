using ApplicationLayer.Dto.Signals;
using ApplicationLayer.Interfaces.Services.Signals;
using DomainLayer.Common.Attributes;

namespace InfrastructureLayer.BusinessLogic.Services.Signals
{
    [InjectAsScoped]
    public class StrategyTextParser : IStrategyTextParser
    {
        public StrategyParseResultDto Parse(string text)
        {
            var result = new StrategyParseResultDto();
            if (string.IsNullOrWhiteSpace(text)) return result;

            text = text.ToLowerInvariant();

            var hasH1Trend = text.Contains("h1") && (text.Contains("روند") || text.Contains("trend"));
            var hasBos = text.Contains("bos") || text.Contains("شکست ساختار") || text.Contains("break of structure");
            var hasFvg = text.Contains("fvg");
            var hasChoch = text.Contains("choch") || text.Contains("تغییر کاراکتر");
            var hasM1Sequence = text.Contains("m1") && (text.Contains("کف") && text.Contains("سقف") && (text.Contains("شکست") || text.Contains("break")));
            var hasOrderBlock = text.Contains("orderblock") || text.Contains("اردربلاک");
            var hasSupport = text.Contains("حمایت") || text.Contains("support");
            var hasResistance = text.Contains("مقاومت") || text.Contains("resistance");

            var macroGroup = new GroupNodeDto
            {
                Operator = "AND",
                Timeframe = hasH1Trend ? "1h" : (text.Contains("daily") || text.Contains("d1") || text.Contains("روزانه")) ? "1d" : "4h",
                Description = "macro_conditions"
            };

            if (hasH1Trend)
            {
                macroGroup.Conditions.Add(new ConditionNodeDto
                {
                    Type = "trend",
                    Description = "H1 trend detection",
                    Timeframe = "1h",
                    Parameters = new Dictionary<string, object> { { "mode", hasSupport ? "bull" : hasResistance ? "bear" : "auto" } }
                });
            }

            if (hasBos)
            {
                macroGroup.Conditions.Add(new ConditionNodeDto
                {
                    Type = "bos",
                    Description = "Break of structure",
                    Timeframe = macroGroup.Timeframe,
                    Parameters = new Dictionary<string, object> { { "direction", hasSupport ? "up" : hasResistance ? "down" : "auto" } }
                });
            }

            if (hasFvg || hasOrderBlock || hasSupport || hasResistance)
            {
                macroGroup.Conditions.Add(new ConditionNodeDto
                {
                    Type = hasFvg ? "fvg_zone" : hasOrderBlock ? "order_block" : hasSupport ? "support_zone" : "resistance_zone",
                    Description = "Entry zone identification",
                    Timeframe = macroGroup.Timeframe
                });
            }

            if (macroGroup.Conditions.Count > 0)
                result.Groups.Add(macroGroup);

            var confirmGroup = new GroupNodeDto
            {
                Operator = "AND",
                Timeframe = text.Contains("m15") ? "15m" : "5m",
                Description = "confirmation_conditions"
            };

            if (hasChoch)
            {
                confirmGroup.Conditions.Add(new ConditionNodeDto
                {
                    Type = "choch",
                    Description = "CHOCH confirmation",
                    Timeframe = confirmGroup.Timeframe,
                    ConfirmationRequired = false
                });
            }
            if (hasFvg)
            {
                confirmGroup.Conditions.Add(new ConditionNodeDto
                {
                    Type = "fvg_confirm",
                    Description = "FVG confirmation",
                    Timeframe = confirmGroup.Timeframe
                });
            }

            if (confirmGroup.Conditions.Count > 0)
                result.Groups.Add(confirmGroup);

            if (hasM1Sequence)
            {
                result.Groups.Add(new GroupNodeDto
                {
                    Operator = "AND",
                    Timeframe = "1m",
                    Description = "entry_sequence",
                    Conditions =
                    {
                        new ConditionNodeDto { Type = "sequence_low_high_pullback_break", Description = "M1 sequence", Timeframe = "1m" },
                        new ConditionNodeDto { Type = "fvg_internal_entry", Description = "M1 internal FVG entry", Timeframe = "1m" }
                    }
                });
            }

            return result;
        }
    }
}