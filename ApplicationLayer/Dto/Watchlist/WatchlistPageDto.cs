using ApplicationLayer.Dto.BaseDtos;

namespace ApplicationLayer.DTOs.Watchlist;

public class WatchlistPageDto
{
    public List<WatchlistItemDto> Items { get; set; } = [];
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

public class GetWatchlistRequestDto
{
    public PaginationDto Pagination { get; set; } = new();
}