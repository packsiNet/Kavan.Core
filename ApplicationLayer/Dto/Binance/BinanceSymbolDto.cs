namespace ApplicationLayer.Dto.Binance;

/// <summary>
/// DTO for Binance symbol information from exchange info API
/// </summary>
public class BinanceSymbolDto
{
    public string Symbol { get; set; } = string.Empty;
    public string BaseAsset { get; set; } = string.Empty;
    public string QuoteAsset { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool IsSpotTradingAllowed { get; set; }
    public bool IsMarginTradingAllowed { get; set; }
}

/// <summary>
/// DTO for Binance exchange info API response
/// </summary>
public class BinanceExchangeInfoDto
{
    public string Timezone { get; set; } = string.Empty;
    public long ServerTime { get; set; }
    public List<BinanceSymbolDto> Symbols { get; set; } = new();
}