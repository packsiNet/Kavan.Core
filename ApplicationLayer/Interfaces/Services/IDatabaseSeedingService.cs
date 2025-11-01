namespace ApplicationLayer.Interfaces.Services;

public interface IDatabaseSeedingService
{
    Task SeedCryptocurrenciesAsync();
}