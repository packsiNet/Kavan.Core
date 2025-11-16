using ApplicationLayer.Dto.Signals;

namespace ApplicationLayer.Interfaces.Services.Signals
{
    public interface IStrategyTextParser
    {
        StrategyParseResultDto Parse(string text);
    }
}