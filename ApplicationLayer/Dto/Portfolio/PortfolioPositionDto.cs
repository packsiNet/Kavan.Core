namespace ApplicationLayer.DTOs.Portfolio;

public class PortfolioPositionDto
{
    public string Symbol { get; set; }
    public decimal TotalQuantity { get; set; }
    public decimal AverageBuyPrice { get; set; }
    public decimal TotalCost { get; set; }
    public decimal CurrentPrice { get; set; }
    public decimal CurrentValue { get; set; }
    public decimal ProfitLoss { get; set; }
    public decimal ProfitLossPercent { get; set; }
}

public class PortfolioPositionsPageDto
{
    public List<PortfolioPositionDto> Items { get; set; } = [];
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

public class GetPortfolioRequestDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}

public class PortfolioEntriesPageDto
{
    public List<PortfolioEntryDto> Items { get; set; } = [];
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

public class GetPortfolioEntriesRequestDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}