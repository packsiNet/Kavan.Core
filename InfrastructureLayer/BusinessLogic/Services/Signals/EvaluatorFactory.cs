using System.Collections.Generic;
using System.Linq;
using ApplicationLayer.Interfaces.Services.Signals;
using DomainLayer.Common.Attributes;

namespace InfrastructureLayer.BusinessLogic.Services.Signals
{
    [InjectAsScoped]
    public class EvaluatorFactory : IConditionEvaluatorFactory
    {
        private readonly Dictionary<string, IConditionEvaluator> _map;

        public EvaluatorFactory(IEnumerable<IConditionEvaluator> evaluators)
        {
            _map = evaluators.ToDictionary(e => e.Type.ToLowerInvariant());
        }

        public IConditionEvaluator? Resolve(string type)
        {
            if (string.IsNullOrWhiteSpace(type)) return null;
            var key = type.ToLowerInvariant();
            return _map.TryGetValue(key, out var eval) ? eval : null;
        }

        public IReadOnlyCollection<IConditionEvaluator> All => _map.Values.ToList();
    }
}