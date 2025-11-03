using System.Collections.Generic;

namespace ApplicationLayer.Interfaces.Services.Signals
{
    public interface IConditionEvaluatorFactory
    {
        IConditionEvaluator? Resolve(string type);
        IReadOnlyCollection<IConditionEvaluator> All { get; }
    }
}