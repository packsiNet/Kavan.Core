namespace ApplicationLayer.Interfaces.External;

public interface IAiProvider
{
    Task<string> TranslateAsync(string inputText, CancellationToken cancellationToken = default);
}

