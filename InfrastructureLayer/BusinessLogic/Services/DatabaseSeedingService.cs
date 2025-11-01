using ApplicationLayer.Interfaces;
using ApplicationLayer.Interfaces.Services;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Microsoft.Extensions.Logging;

namespace InfrastructureLayer.BusinessLogic.Services;

[InjectAsScoped]
public class DatabaseSeedingService(
    IUnitOfWork _unitOfWork,
    IRepository<Cryptocurrency> _cryptocurrencyRepo,
    ILogger<DatabaseSeedingService> _logger
) : IDatabaseSeedingService
{
    public async Task SeedCryptocurrenciesAsync()
    {
        try
        {           
            // Check if we already have cryptocurrencies
            var existingCryptos = _cryptocurrencyRepo.GetAll().Where(c => c.IsActive && !c.IsDeleted).ToList();
            
            if (existingCryptos.Any())
            {
                _logger.LogInformation("Cryptocurrencies already exist in database. Skipping seeding.");
                return;
            }

            _logger.LogInformation("Seeding cryptocurrency data...");

            var cryptocurrencies = new List<Cryptocurrency>
            {
                new Cryptocurrency
                {
                    Symbol = "BTCUSDT",
                    BaseAsset = "BTC",
                    QuoteAsset = "USDT",
                    IsActive = true,
                    IsDeleted = false
                },
                new Cryptocurrency
                {
                    Symbol = "ETHUSDT",
                    BaseAsset = "ETH",
                    QuoteAsset = "USDT",
                    IsActive = true,
                    IsDeleted = false
                },
                new Cryptocurrency
                {
                    Symbol = "BNBUSDT",
                    BaseAsset = "BNB",
                    QuoteAsset = "USDT",
                    IsActive = true,
                    IsDeleted = false
                },
                new Cryptocurrency
                {
                    Symbol = "ADAUSDT",
                    BaseAsset = "ADA",
                    QuoteAsset = "USDT",
                    IsActive = true,
                    IsDeleted = false
                },
                new Cryptocurrency
                {
                    Symbol = "SOLUSDT",
                    BaseAsset = "SOL",
                    QuoteAsset = "USDT",
                    IsActive = true,
                    IsDeleted = false
                }
            };

            foreach (var crypto in cryptocurrencies)
            {
                await _cryptocurrencyRepo.AddAsync(crypto);
            }

            await _unitOfWork.SaveChangesAsync();
            
            _logger.LogInformation($"Successfully seeded {cryptocurrencies.Count} cryptocurrencies.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while seeding cryptocurrency data.");
            throw;
        }
    }
}