using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.Dto.MarketAnalysis;
using ApplicationLayer.Extensions;
using ApplicationLayer.Features.MarketAnalysis.Query;
using ApplicationLayer.Interfaces;
using DomainLayer.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApplicationLayer.Features.MarketAnalysis.Handler;

/// <summary>
/// Handler for getting available cryptocurrency symbols from database
/// </summary>
public class GetAvailableSymbolsHandler : IRequestHandler<GetAvailableSymbolsQuery, HandlerResult>
{
    private readonly IRepository<Cryptocurrency> _cryptocurrencyRepository;

    public GetAvailableSymbolsHandler(IRepository<Cryptocurrency> cryptocurrencyRepository)
    {
        _cryptocurrencyRepository = cryptocurrencyRepository;
    }

    public async Task<HandlerResult> Handle(GetAvailableSymbolsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var cryptocurrencies = await _cryptocurrencyRepository.Query()
                .Where(c => c.IsActive)
                .OrderBy(c => c.Symbol)
                .ToListAsync(cancellationToken);

            var symbolInfos = cryptocurrencies.Select(crypto => new SymbolInfoDto
            {
                Symbol = crypto.Symbol,
                Name = new SymbolNameDto
                {
                    English = GetCryptocurrencyName(crypto.Symbol),
                    Persian = GetCryptocurrencyPersianName(crypto.Symbol)
                },
                Category = GetCryptocurrencyCategory(crypto.Symbol)
            }).ToList();

            var response = new AvailableSymbolsResponseDto
            {
                Success = true,
                Data = symbolInfos,
                Count = symbolInfos.Count,
                Timestamp = DateTime.UtcNow,
                Message = "Available symbols retrieved successfully"
            };

            return Result<AvailableSymbolsResponseDto>.Success(response).ToHandlerResult();
        }
        catch (Exception ex)
        {
            return Result<AvailableSymbolsResponseDto>.GeneralFailure($"Error retrieving symbols: {ex.Message}").ToHandlerResult();
        }
    }

    private string GetCryptocurrencyName(string symbol)
    {
        var baseAsset = symbol.Replace("USDT", "").Replace("BUSD", "").Replace("BTC", "").Replace("ETH", "");
        
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
            { "XLM", "Stellar" }
        };

        return cryptoNames.TryGetValue(baseAsset, out var name) ? name : baseAsset;
    }

    private string GetCryptocurrencyPersianName(string symbol)
    {
        var baseAsset = symbol.Replace("USDT", "").Replace("BUSD", "").Replace("BTC", "").Replace("ETH", "");
        
        var cryptoPersianNames = new Dictionary<string, string>
        {
            { "BTC", "بیت‌کوین" },
            { "ETH", "اتریوم" },
            { "BNB", "بایننس کوین" },
            { "ADA", "کاردانو" },
            { "SOL", "سولانا" },
            { "XRP", "ریپل" },
            { "DOT", "پولکادات" },
            { "AVAX", "آوالانچ" },
            { "MATIC", "پولیگان" },
            { "LINK", "چین‌لینک" },
            { "UNI", "یونی‌سواپ" },
            { "LTC", "لایت‌کوین" },
            { "BCH", "بیت‌کوین کش" },
            { "ALGO", "الگوراند" },
            { "VET", "وی‌چین" },
            { "ICP", "اینترنت کامپیوتر" },
            { "FIL", "فایل‌کوین" },
            { "TRX", "ترون" },
            { "ETC", "اتریوم کلاسیک" },
            { "XLM", "استلار" }
        };

        return cryptoPersianNames.TryGetValue(baseAsset, out var name) ? name : baseAsset;
    }

    private string GetCryptocurrencyCategory(string symbol)
    {
        var majorCryptos = new HashSet<string> { "BTCUSDT", "ETHUSDT", "BNBUSDT" };
        return majorCryptos.Contains(symbol) ? "major" : "altcoin";
    }
}