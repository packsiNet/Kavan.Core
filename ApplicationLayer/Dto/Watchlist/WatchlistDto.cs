namespace ApplicationLayer.DTOs.Watchlist;

public class WatchlistDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsActive { get; set; }
    public List<WatchlistItemDto> Items { get; set; } = [];
}

public class WatchlistsTreeDto
{
    public List<WatchlistDto> Items { get; set; } = [];
}