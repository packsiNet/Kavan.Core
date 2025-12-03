namespace ApplicationLayer.Interfaces.External;

public interface IAiTranslationService
{
    Task<string> TranslateAsync(string inputText, CancellationToken cancellationToken = default);
}

