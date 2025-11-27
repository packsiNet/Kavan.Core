namespace ApplicationLayer.DTOs.Watchlist;

public class WatchlistItemDto
{
    public int Id { get; set; }
    public string Symbol { get; set; }
    public decimal CurrentPrice { get; set; }
    public DateTime UpdatedAt { get; set; }
}