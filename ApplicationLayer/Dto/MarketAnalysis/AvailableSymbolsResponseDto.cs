namespace ApplicationLayer.Dto.MarketAnalysis;

/// <summary>
/// Response DTO for available cryptocurrency symbols
/// </summary>
public class AvailableSymbolsResponseDto
{
    public bool Success { get; set; } = true;
    public List<SymbolInfoDto> Data { get; set; } = new();
    public int Count { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string Message { get; set; } = "Available symbols retrieved successfully";
}

/// <summary>
/// DTO for individual symbol information with dual language support
/// </summary>
public class SymbolInfoDto
{
    public string Symbol { get; set; } = string.Empty;
    public SymbolNameDto Name { get; set; } = new();
    public string Category { get; set; } = string.Empty;
}

/// <summary>
/// DTO for symbol name with dual language support
/// </summary>
public class SymbolNameDto
{
    public string English { get; set; } = string.Empty;
    public string Persian { get; set; } = string.Empty;
}