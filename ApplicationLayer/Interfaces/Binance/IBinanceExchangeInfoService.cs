using ApplicationLayer.Dto.Binance;

namespace ApplicationLayer.Interfaces.Binance;

/// <summary>
/// Service for fetching exchange information from Binance API
/// </summary>
public interface IBinanceExchangeInfoService
{
    /// <summary>
    /// Get all available trading symbols from Binance exchange
    /// </summary>
    /// <returns>List of exchange symbols with their details</returns>
    Task<List<BinanceSymbolDto>> GetExchangeSymbolsAsync();
    
    /// <summary>
    /// Get filtered USDT trading pairs only
    /// </summary>
    /// <returns>List of USDT trading pairs</returns>
    Task<List<BinanceSymbolDto>> GetUsdtTradingPairsAsync();
}