using ApplicationLayer.Interfaces;
using ApplicationLayer.Interfaces.Services;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Microsoft.Extensions.Logging;

namespace InfrastructureLayer.BusinessLogic.Services.Crypto;

[InjectAsScoped]
public class CryptoBootstrapService(
    IRepository<Cryptocurrency> cryptocurrencyRepository,
    IUnitOfWork unitOfWork,
    ILogger<CryptoBootstrapService> logger
) : ICryptoBootstrapService
{
    private readonly IRepository<Cryptocurrency> _cryptocurrencies = cryptocurrencyRepository;
    private readonly IUnitOfWork _uow = unitOfWork;
    private readonly ILogger<CryptoBootstrapService> _logger = logger;

    private readonly IReadOnlyList<CryptoSeedItem> _defaults = new List<CryptoSeedItem>
    {
        new("BTC", "Bitcoin", "Layer1"),
        new("ETH", "Ethereum", "Layer1"),
        new("BNB", "BNB", "Layer1"),
        new("SOL", "Solana", "Layer1"),
        new("ADA", "Cardano", "Layer1"),
        new("XRP", "XRP", "Payments"),
        new("DOGE", "Dogecoin", "Meme"),
        new("TRX", "TRON", "Layer1"),
        new("MATIC", "Polygon", "Layer2"),
        new("DOT", "Polkadot", "Layer1")
    };

    public async Task EnsureSeededAsync(CancellationToken cancellationToken = default)
    {
        var hasAny = await _cryptocurrencies.AnyAsync(x => true);
        if (hasAny)
            return;

        var entities = _defaults.Select(a => new Cryptocurrency
        {
            Category = a.Category,
            Name = a.Name,
            Symbol = a.Symbol + "USDT",
            BaseAsset = a.Symbol,
            QuoteAsset = "USDT"
        }).ToList();

        await _cryptocurrencies.AddRangeAsync(entities);
        await _uow.SaveChangesAsync(cancellationToken);

        if (_logger.IsEnabled(LogLevel.Information))
            _logger.LogInformation("Seeded {Count} default cryptocurrencies.", entities.Count);
    }

    private readonly record struct CryptoSeedItem(string Symbol, string Name, string Category);
}

