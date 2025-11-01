using ApplicationLayer.Interfaces.Services;
using DomainLayer.Common.Attributes;

namespace InfrastructureLayer.BusinessLogic.Services;

[InjectAsScoped]
public class SignalEvaluatorFactory : ISignalEvaluatorFactory
{
    private readonly IEnumerable<ISignalEvaluator> _evaluators;

    public SignalEvaluatorFactory(IEnumerable<ISignalEvaluator> evaluators)
    {
        _evaluators = evaluators;
    }

    public IEnumerable<ISignalEvaluator> GetAllEvaluators()
    {
        return _evaluators;
    }

    public IEnumerable<ISignalEvaluator> GetEvaluatorsByCategory(string category)
    {
        return _evaluators.Where(e => e.IndicatorCategory.Equals(category, StringComparison.OrdinalIgnoreCase));
    }

    public ISignalEvaluator? GetEvaluator(string category, string name)
    {
        return _evaluators.FirstOrDefault(e => 
            e.IndicatorCategory.Equals(category, StringComparison.OrdinalIgnoreCase) &&
            e.IndicatorName.Equals(name, StringComparison.OrdinalIgnoreCase));
    }
}