namespace ApplicationLayer.Interfaces.Services;

public interface ICryptoBootstrapService
{
    Task EnsureSeededAsync(CancellationToken cancellationToken = default);
}

