using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.DTOs.Watchlist;

namespace ApplicationLayer.Interfaces.Watchlist;

public interface IWatchlistService
{
    Task<Result<WatchlistDto>> CreateListAsync(CreateWatchlistDto dto);
    Task<Result<WatchlistDto>> UpdateListAsync(int id, UpdateWatchlistDto dto);
    Task<Result> DeleteListAsync(int id);
    Task<Result<WatchlistsTreeDto>> GetListsAsync();
    Task<Result<WatchlistItemDto>> AddItemAsync(int watchlistId, string symbol);
    Task<Result> RemoveItemAsync(int itemId);
}