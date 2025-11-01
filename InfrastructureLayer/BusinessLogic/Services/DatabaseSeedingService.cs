using ApplicationLayer.Interfaces;
using ApplicationLayer.Interfaces.Binance;
using ApplicationLayer.Interfaces.Services;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureLayer.BusinessLogic.Services;

[InjectAsScoped]
public class DatabaseSeedingService(
    IRepository<Cryptocurrency> _cryptocurrencyRepository,
    IRepository<TimeFrame> _timeFrameRepository,
    IUnitOfWork _unitOfWork,
    IBinanceExchangeInfoService _binanceExchangeInfoService) : IDatabaseSeedingService
{
    public async Task SeedCryptocurrenciesAsync()
    {
        
        // Check if cryptocurrencies already exist
        var existingCount = await _cryptocurrencyRepository.Query().CountAsync();
        if (existingCount > 0)
        {
            return; // Already seeded
        }

        try
        {
            // Fetch USDT trading pairs from Binance API
            var binanceSymbols = await _binanceExchangeInfoService.GetUsdtTradingPairsAsync();
            
            if (!binanceSymbols.Any())
            {
                // Fallback to essential cryptocurrencies if API fails
                await SeedFallbackCryptocurrenciesAsync();
                return;
            }

            // Filter to get major cryptocurrencies (top 50 by market cap typically)
            var majorCryptos = GetMajorCryptocurrencies(binanceSymbols);

            var cryptocurrencies = majorCryptos.Select(symbol => new Cryptocurrency
            {
                Symbol = symbol.Symbol,
                BaseAsset = symbol.BaseAsset,
                QuoteAsset = symbol.QuoteAsset,
                IsActive = true
            }).ToList();

            foreach (var crypto in cryptocurrencies)
            {
                await _cryptocurrencyRepository.AddAsync(crypto);
            }

            await _unitOfWork.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Log error and use fallback
            Console.WriteLine($"Error seeding cryptocurrencies from Binance API: {ex.Message}");
            await SeedFallbackCryptocurrenciesAsync();
        }
    }

    private async Task SeedFallbackCryptocurrenciesAsync()
    {
        // Essential cryptocurrencies as fallback
        var fallbackCryptocurrencies = new List<Cryptocurrency>
        {
            new() { Symbol = "BTCUSDT", BaseAsset = "BTC", QuoteAsset = "USDT", IsActive = true },
            new() { Symbol = "ETHUSDT", BaseAsset = "ETH", QuoteAsset = "USDT", IsActive = true },
            new() { Symbol = "BNBUSDT", BaseAsset = "BNB", QuoteAsset = "USDT", IsActive = true },
            new() { Symbol = "ADAUSDT", BaseAsset = "ADA", QuoteAsset = "USDT", IsActive = true },
            new() { Symbol = "SOLUSDT", BaseAsset = "SOL", QuoteAsset = "USDT", IsActive = true }
        };

        foreach (var crypto in fallbackCryptocurrencies)
        {
            await _cryptocurrencyRepository.AddAsync(crypto);
        }

        await _unitOfWork.SaveChangesAsync();
    }

    private List<ApplicationLayer.Dto.Binance.BinanceSymbolDto> GetMajorCryptocurrencies(
        List<ApplicationLayer.Dto.Binance.BinanceSymbolDto> allSymbols)
    {
        // List of major cryptocurrency base assets
        var majorCryptoAssets = new HashSet<string>
        {
            "BTC", "ETH", "BNB", "ADA", "SOL", "XRP", "DOT", "AVAX", "MATIC", "LINK",
            "UNI", "LTC", "BCH", "ALGO", "VET", "ICP", "FIL", "TRX", "ETC", "XLM",
            "ATOM", "THETA", "HBAR", "NEAR", "MANA", "SAND", "CRV", "AAVE", "MKR", "COMP"
        };

        return allSymbols
            .Where(s => majorCryptoAssets.Contains(s.BaseAsset))
            .Take(30) // Limit to top 30 major cryptocurrencies
            .ToList();
    }

    private string GetCryptocurrencyName(string baseAsset)
    {
        var cryptoNames = new Dictionary<string, string>
        {
            { "BTC", "Bitcoin" },
            { "ETH", "Ethereum" },
            { "BNB", "Binance Coin" },
            { "ADA", "Cardano" },
            { "SOL", "Solana" },
            { "XRP", "Ripple" },
            { "DOT", "Polkadot" },
            { "AVAX", "Avalanche" },
            { "MATIC", "Polygon" },
            { "LINK", "Chainlink" },
            { "UNI", "Uniswap" },
            { "LTC", "Litecoin" },
            { "BCH", "Bitcoin Cash" },
            { "ALGO", "Algorand" },
            { "VET", "VeChain" },
            { "ICP", "Internet Computer" },
            { "FIL", "Filecoin" },
            { "TRX", "TRON" },
            { "ETC", "Ethereum Classic" },
            { "XLM", "Stellar" },
            { "ATOM", "Cosmos" },
            { "THETA", "Theta Network" },
            { "HBAR", "Hedera" },
            { "NEAR", "NEAR Protocol" },
            { "MANA", "Decentraland" },
            { "SAND", "The Sandbox" },
            { "CRV", "Curve DAO Token" },
            { "AAVE", "Aave" },
            { "MKR", "Maker" },
            { "COMP", "Compound" }
        };

        return cryptoNames.TryGetValue(baseAsset, out var name) ? name : baseAsset;
    }

    public async Task SeedTimeFramesAsync()
    {
        // Check if timeframes already exist
        var existingCount = await _timeFrameRepository.Query().CountAsync();
        if (existingCount > 0)
        {
            return; // Already seeded
        }

        var timeFrames = new List<TimeFrame>
        {
            new() { Code = "1m", NameEnglish = "1 Minute", NamePersian = "1 دقیقه", DurationInMinutes = 1, DisplayOrder = 1, IsActive = true },
            new() { Code = "5m", NameEnglish = "5 Minutes", NamePersian = "5 دقیقه", DurationInMinutes = 5, DisplayOrder = 2, IsActive = true },
            new() { Code = "1h", NameEnglish = "1 Hour", NamePersian = "1 ساعت", DurationInMinutes = 60, DisplayOrder = 3, IsActive = true },
            new() { Code = "4h", NameEnglish = "4 Hours", NamePersian = "4 ساعت", DurationInMinutes = 240, DisplayOrder = 4, IsActive = true },
            new() { Code = "1d", NameEnglish = "1 Day", NamePersian = "1 روز", DurationInMinutes = 1440, DisplayOrder = 5, IsActive = true }
        };

        foreach (var timeFrame in timeFrames)
        {
            await _timeFrameRepository.AddAsync(timeFrame);
        }

        await _unitOfWork.SaveChangesAsync();
    }
}