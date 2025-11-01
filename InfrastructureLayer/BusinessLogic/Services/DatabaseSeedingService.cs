using ApplicationLayer.Interfaces;
using ApplicationLayer.Interfaces.Binance;
using ApplicationLayer.Interfaces.Services;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureLayer.BusinessLogic.Services;

[InjectAsScoped]
public class DatabaseSeedingService(
    //IBinanceExchangeInfoService _binanceExchangeInfoService
    IRepository<Cryptocurrency> _cryptocurrencyRepository,
    IRepository<TimeFrame> _timeFrameRepository,
    IUnitOfWork _unitOfWork) : IDatabaseSeedingService
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
            await SeedFallbackCryptocurrenciesAsync();
            // var binanceSymbols = await _binanceExchangeInfoService.GetUsdtTradingPairsAsync();
            
            // if (!binanceSymbols.Any())
            // {
            //     // Fallback to essential cryptocurrencies if API fails
            //     await SeedFallbackCryptocurrenciesAsync();
            //     return;
            // }

            // // Filter to get major cryptocurrencies (top 50 by market cap typically)
            // var majorCryptos = GetMajorCryptocurrencies(binanceSymbols);

            // var cryptocurrencies = majorCryptos.Select(symbol => new Cryptocurrency
            // {
            //     Symbol = symbol.Symbol,
            //     BaseAsset = symbol.BaseAsset,
            //     QuoteAsset = symbol.QuoteAsset,
            //     IsActive = true
            // }).ToList();

            // foreach (var crypto in cryptocurrencies)
            // {
            //     await _cryptocurrencyRepository.AddAsync(crypto);
            // }

            // await _unitOfWork.SaveChangesAsync();
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
            new() { Symbol = "SOLUSDT", BaseAsset = "SOL", QuoteAsset = "USDT", IsActive = true },
            new() { Symbol = "XRPUSDT", BaseAsset = "XRP", QuoteAsset = "USDT", IsActive = true },
            new() { Symbol = "ADAUSDT", BaseAsset = "ADA", QuoteAsset = "USDT", IsActive = true },
            new() { Symbol = "DOGEUSDT", BaseAsset = "DOGE", QuoteAsset = "USDT", IsActive = true },
            new() { Symbol = "AVAXUSDT", BaseAsset = "AVAX", QuoteAsset = "USDT", IsActive = true },
            new() { Symbol = "DOTUSDT", BaseAsset = "DOT", QuoteAsset = "USDT", IsActive = true },
            new() { Symbol = "LINKUSDT", BaseAsset = "LINK", QuoteAsset = "USDT", IsActive = true },
            new() { Symbol = "MATICUSDT", BaseAsset = "MATIC", QuoteAsset = "USDT", IsActive = true },
            new() { Symbol = "LTCUSDT", BaseAsset = "LTC", QuoteAsset = "USDT", IsActive = true },
            new() { Symbol = "TRXUSDT", BaseAsset = "TRX", QuoteAsset = "USDT", IsActive = true },
            new() { Symbol = "ICPUSDT", BaseAsset = "ICP", QuoteAsset = "USDT", IsActive = true },
            new() { Symbol = "ATOMUSDT", BaseAsset = "ATOM", QuoteAsset = "USDT", IsActive = true },
            new() { Symbol = "APTUSDT", BaseAsset = "APT", QuoteAsset = "USDT", IsActive = true },
            new() { Symbol = "FILUSDT", BaseAsset = "FIL", QuoteAsset = "USDT", IsActive = true },
            new() { Symbol = "NEARUSDT", BaseAsset = "NEAR", QuoteAsset = "USDT", IsActive = true },
            new() { Symbol = "ARBUSDT", BaseAsset = "ARB", QuoteAsset = "USDT", IsActive = true },
            new() { Symbol = "ETCUSDT", BaseAsset = "ETC", QuoteAsset = "USDT", IsActive = true },
            new() { Symbol = "OPUSDT", BaseAsset = "OP", QuoteAsset = "USDT", IsActive = true },
            new() { Symbol = "VETUSDT", BaseAsset = "VET", QuoteAsset = "USDT", IsActive = true },
            new() { Symbol = "AAVEUSDT", BaseAsset = "AAVE", QuoteAsset = "USDT", IsActive = true },
            new() { Symbol = "GRTUSDT", BaseAsset = "GRT", QuoteAsset = "USDT", IsActive = true },
            new() { Symbol = "RUNEUSDT", BaseAsset = "RUNE", QuoteAsset = "USDT", IsActive = true },
            new() { Symbol = "SANDUSDT", BaseAsset = "SAND", QuoteAsset = "USDT", IsActive = true },
            new() { Symbol = "MANAUSDT", BaseAsset = "MANA", QuoteAsset = "USDT", IsActive = true },
            new() { Symbol = "EGLDUSDT", BaseAsset = "EGLD", QuoteAsset = "USDT", IsActive = true },
            new() { Symbol = "AXSUSDT", BaseAsset = "AXS", QuoteAsset = "USDT", IsActive = true },
            new() { Symbol = "FLOWUSDT", BaseAsset = "FLOW", QuoteAsset = "USDT", IsActive = true },
            new() { Symbol = "XTZUSDT", BaseAsset = "XTZ", QuoteAsset = "USDT", IsActive = true },
            new() { Symbol = "IMXUSDT", BaseAsset = "IMX", QuoteAsset = "USDT", IsActive = true },
            new() { Symbol = "ALGOUSDT", BaseAsset = "ALGO", QuoteAsset = "USDT", IsActive = true },
            new() { Symbol = "STXUSDT", BaseAsset = "STX", QuoteAsset = "USDT", IsActive = true },
            new() { Symbol = "SUIUSDT", BaseAsset = "SUI", QuoteAsset = "USDT", IsActive = true },
            new() { Symbol = "INJUSDT", BaseAsset = "INJ", QuoteAsset = "USDT", IsActive = true },
            new() { Symbol = "FTMUSDT", BaseAsset = "FTM", QuoteAsset = "USDT", IsActive = true },
            new() { Symbol = "HBARUSDT", BaseAsset = "HBAR", QuoteAsset = "USDT", IsActive = true },
            new() { Symbol = "CFXUSDT", BaseAsset = "CFX", QuoteAsset = "USDT", IsActive = true },
            new() { Symbol = "GALAUSDT", BaseAsset = "GALA", QuoteAsset = "USDT", IsActive = true }
            // new() { Symbol = "THETAUSDT", BaseAsset = "THETA", QuoteAsset = "USDT", IsActive = true },
            // new() { Symbol = "ZILUSDT", BaseAsset = "ZIL", QuoteAsset = "USDT", IsActive = true },
            // new() { Symbol = "KAVAUSDT", BaseAsset = "KAVA", QuoteAsset = "USDT", IsActive = true },
            // new() { Symbol = "DASHUSDT", BaseAsset = "DASH", QuoteAsset = "USDT", IsActive = true },
            // new() { Symbol = "NEOUSDT", BaseAsset = "NEO", QuoteAsset = "USDT", IsActive = true },
            // new() { Symbol = "CHZUSDT", BaseAsset = "CHZ", QuoteAsset = "USDT", IsActive = true },
            // new() { Symbol = "XMRUSDT", BaseAsset = "XMR", QuoteAsset = "USDT", IsActive = true },
            // new() { Symbol = "EOSUSDT", BaseAsset = "EOS", QuoteAsset = "USDT", IsActive = true },
            // new() { Symbol = "KSMUSDT", BaseAsset = "KSM", QuoteAsset = "USDT", IsActive = true },
            // new() { Symbol = "CAKEUSDT", BaseAsset = "CAKE", QuoteAsset = "USDT", IsActive = true },
            // new() { Symbol = "MINAUSDT", BaseAsset = "MINA", QuoteAsset = "USDT", IsActive = true },
            // new() { Symbol = "PEPEUSDT", BaseAsset = "PEPE", QuoteAsset = "USDT", IsActive = true },
            // new() { Symbol = "ENSUSDT", BaseAsset = "ENS", QuoteAsset = "USDT", IsActive = true },
            // new() { Symbol = "RNDRUSDT", BaseAsset = "RNDR", QuoteAsset = "USDT", IsActive = true },
            // new() { Symbol = "FXSUSDT", BaseAsset = "FXS", QuoteAsset = "USDT", IsActive = true },
            // new() { Symbol = "GMXUSDT", BaseAsset = "GMX", QuoteAsset = "USDT", IsActive = true },
            // new() { Symbol = "DYDXUSDT", BaseAsset = "DYDX", QuoteAsset = "USDT", IsActive = true },
            // new() { Symbol = "WOOUSDT", BaseAsset = "WOO", QuoteAsset = "USDT", IsActive = true },
            // new() { Symbol = "LDOUSDT", BaseAsset = "LDO", QuoteAsset = "USDT", IsActive = true },
            // new() { Symbol = "SEIUSDT", BaseAsset = "SEI", QuoteAsset = "USDT", IsActive = true },
            // new() { Symbol = "BLURUSDT", BaseAsset = "BLUR", QuoteAsset = "USDT", IsActive = true },
            // new() { Symbol = "ZRXUSDT", BaseAsset = "ZRX", QuoteAsset = "USDT", IsActive = true },
            // new() { Symbol = "IOTAUSDT", BaseAsset = "IOTA", QuoteAsset = "USDT", IsActive = true },
            // new() { Symbol = "ROSEUSDT", BaseAsset = "ROSE", QuoteAsset = "USDT", IsActive = true },
            // new() { Symbol = "ONEUSDT", BaseAsset = "ONE", QuoteAsset = "USDT", IsActive = true },
            // new() { Symbol = "FLUXUSDT", BaseAsset = "FLUX", QuoteAsset = "USDT", IsActive = true },
            // new() { Symbol = "OCEANUSDT", BaseAsset = "OCEAN", QuoteAsset = "USDT", IsActive = true },
            // new() { Symbol = "CELOUSDT", BaseAsset = "CELO", QuoteAsset = "USDT", IsActive = true },
            // new() { Symbol = "BATUSDT", BaseAsset = "BAT", QuoteAsset = "USDT", IsActive = true },
            // new() { Symbol = "QTUMUSDT", BaseAsset = "QTUM", QuoteAsset = "USDT", IsActive = true },
            // new() { Symbol = "NEXOUSDT", BaseAsset = "NEXO", QuoteAsset = "USDT", IsActive = true },
            // new() { Symbol = "MASKUSDT", BaseAsset = "MASK", QuoteAsset = "USDT", IsActive = true },
            // new() { Symbol = "ANKRUSDT", BaseAsset = "ANKR", QuoteAsset = "USDT", IsActive = true },
            // new() { Symbol = "COMPUSDT", BaseAsset = "COMP", QuoteAsset = "USDT", IsActive = true },
            // new() { Symbol = "BALUSDT", BaseAsset = "BAL", QuoteAsset = "USDT", IsActive = true },
            // new() { Symbol = "KLAYUSDT", BaseAsset = "KLAY", QuoteAsset = "USDT", IsActive = true },
            // new() { Symbol = "RNBUSDT", BaseAsset = "RNB", QuoteAsset = "USDT", IsActive = true },
            // new() { Symbol = "PYTHUSDT", BaseAsset = "PYTH", QuoteAsset = "USDT", IsActive = true },
            // new() { Symbol = "ARUSDT", BaseAsset = "AR", QuoteAsset = "USDT", IsActive = true },
            // new() { Symbol = "TWTUSDT", BaseAsset = "TWT", QuoteAsset = "USDT", IsActive = true },
            // new() { Symbol = "GMTUSDT", BaseAsset = "GMT", QuoteAsset = "USDT", IsActive = true },
            // new() { Symbol = "ASTRUSDT", BaseAsset = "ASTR", QuoteAsset = "USDT", IsActive = true },
            // new() { Symbol = "CKBUSDT", BaseAsset = "CKB", QuoteAsset = "USDT", IsActive = true },
            // new() { Symbol = "ONTUSDT", BaseAsset = "ONT", QuoteAsset = "USDT", IsActive = true },
            // new() { Symbol = "RSRUSDT", BaseAsset = "RSR", QuoteAsset = "USDT", IsActive = true },
            // new() { Symbol = "SYSUSDT", BaseAsset = "SYS", QuoteAsset = "USDT", IsActive = true },
            // new() { Symbol = "HOOKUSDT", BaseAsset = "HOOK", QuoteAsset = "USDT", IsActive = true },
            // new() { Symbol = "COTIUSDT", BaseAsset = "COTI", QuoteAsset = "USDT", IsActive = true },
            // new() { Symbol = "GLMRUSDT", BaseAsset = "GLMR", QuoteAsset = "USDT", IsActive = true },
            // new() { Symbol = "LPTUSDT", BaseAsset = "LPT", QuoteAsset = "USDT", IsActive = true },
            // new() { Symbol = "CELRUSDT", BaseAsset = "CELR", QuoteAsset = "USDT", IsActive = true },
            // new() { Symbol = "BANDUSDT", BaseAsset = "BAND", QuoteAsset = "USDT", IsActive = true },
            // new() { Symbol = "NKNUSDT", BaseAsset = "NKN", QuoteAsset = "USDT", IsActive = true },
            // new() { Symbol = "LOOMUSDT", BaseAsset = "LOOM", QuoteAsset = "USDT", IsActive = true },
            // new() { Symbol = "AKTUSDT", BaseAsset = "AKT", QuoteAsset = "USDT", IsActive = true },
            // new() { Symbol = "JTOUSDT", BaseAsset = "JTO", QuoteAsset = "USDT", IsActive = true },
            // new() { Symbol = "SUPERUSDT", BaseAsset = "SUPER", QuoteAsset = "USDT", IsActive = true },
            // new() { Symbol = "SKLUSDT", BaseAsset = "SKL", QuoteAsset = "USDT", IsActive = true },
            // new() { Symbol = "STORJUSDT", BaseAsset = "STORJ", QuoteAsset = "USDT", IsActive = true },
            // new() { Symbol = "ANTUSDT", BaseAsset = "ANT", QuoteAsset = "USDT", IsActive = true },
            // new() { Symbol = "POLYXUSDT", BaseAsset = "POLYX", QuoteAsset = "USDT", IsActive = true },
            // new() { Symbol = "JASMYUSDT", BaseAsset = "JASMY", QuoteAsset = "USDT", IsActive = true }
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