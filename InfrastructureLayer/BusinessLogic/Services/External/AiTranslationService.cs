using ApplicationLayer.Interfaces.External;
using DomainLayer.Common.Attributes;

namespace InfrastructureLayer.BusinessLogic.Services.External;

[InjectAsScoped]
public class AiTranslationService : IAiTranslationService
{
    private readonly IAiProvider _provider;

    public AiTranslationService(IAiProvider provider)
    {
        _provider = provider;
    }

    public Task<string> TranslateAsync(string inputText, CancellationToken cancellationToken = default)
    {
        return _provider.TranslateAsync(inputText, cancellationToken);
    }
}

