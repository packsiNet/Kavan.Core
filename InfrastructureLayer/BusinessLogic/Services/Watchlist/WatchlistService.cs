using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.DTOs.Watchlist;
using ApplicationLayer.Interfaces;
using ApplicationLayer.Interfaces.Watchlist;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureLayer.BusinessLogic.Services.Watchlists;

[InjectAsScoped]
public class WatchlistService(IUnitOfWork _uow,
                              IRepository<Watchlist> _lists,
                              IRepository<WatchlistItem> _items,
                              IRepository<Cryptocurrency> _cryptos,
                              IRepository<Candle_1m> _candles_1m,
                              IUserContextService _user) : IWatchlistService
{
    public async Task<Result<WatchlistDto>> CreateListAsync(CreateWatchlistDto dto)
    {
        if (_user.UserId == null) return Result<WatchlistDto>.AuthenticationFailure();
        var entity = new Watchlist { OwnerUserId = _user.UserId.Value, Name = dto.Name };
        await _uow.BeginTransactionAsync();
        await _lists.AddAsync(entity);
        await _uow.SaveChangesAsync();
        await _uow.CommitAsync();
        return Result<WatchlistDto>.Success(ToListDto(entity, new List<WatchlistItemDto>()));
    }

    public async Task<Result<WatchlistDto>> UpdateListAsync(int id, UpdateWatchlistDto dto)
    {
        if (_user.UserId == null) return Result<WatchlistDto>.AuthenticationFailure();
        var entity = await _lists.GetDbSet().FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) return Result<WatchlistDto>.NotFound("واچ‌لیست یافت نشد");
        if (entity.OwnerUserId != _user.UserId.Value) return Result<WatchlistDto>.Failure(new Error("INCORRECT_USER", "اجازه ویرایش ندارید", ApplicationLayer.Common.Enums.RequestStatus.IncorrectUser));
        entity.Name = dto.Name;
        entity.IsActive = dto.IsActive;
        entity.MarkAsUpdated();
        await _uow.BeginTransactionAsync();
        await _lists.UpdateAsync(entity);
        await _uow.SaveChangesAsync();
        await _uow.CommitAsync();
        var items = await _items.Query().Where(x => x.WatchlistId == id).Select(x => x).ToListAsync();
        var dtos = new List<WatchlistItemDto>();
        foreach (var it in items) dtos.Add(await ToItemDtoAsync(it));
        return Result<WatchlistDto>.Success(ToListDto(entity, dtos));
    }

    public async Task<Result> DeleteListAsync(int id)
    {
        if (_user.UserId == null) return Result.AuthenticationFailure();
        var entity = await _lists.GetDbSet().FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) return Result.Success();
        if (entity.OwnerUserId != _user.UserId.Value) return Result.Failure(new Error("INCORRECT_USER", "اجازه حذف ندارید", ApplicationLayer.Common.Enums.RequestStatus.IncorrectUser));
        await _uow.BeginTransactionAsync();
        _lists.Remove(entity);
        await _uow.SaveChangesAsync();
        await _uow.CommitAsync();
        return Result.Success();
    }

    public async Task<Result<WatchlistsTreeDto>> GetListsAsync()
    {
        if (_user.UserId == null) return Result<WatchlistsTreeDto>.AuthenticationFailure();
        var query = _lists.Query().Where(x => x.OwnerUserId == _user.UserId.Value && x.IsActive);
        var list = await query.OrderByDescending(x => x.CreatedAt).Select(x => x).ToListAsync();
        var dtos = new List<WatchlistDto>();
        foreach (var wl in list)
        {
            var items = await _items.Query().Where(x => x.WatchlistId == wl.Id).OrderByDescending(x => x.CreatedAt).Select(x => x).ToListAsync();
            var itemDtos = new List<WatchlistItemDto>();
            foreach (var it in items) itemDtos.Add(await ToItemDtoAsync(it));
            dtos.Add(ToListDto(wl, itemDtos));
        }
        var tree = new WatchlistsTreeDto { Items = dtos };
        return Result<WatchlistsTreeDto>.Success(tree);
    }

    public async Task<Result<WatchlistItemDto>> AddItemAsync(int watchlistId, string symbol)
    {
        if (_user.UserId == null) return Result<WatchlistItemDto>.AuthenticationFailure();
        var wl = await _lists.GetDbSet().FirstOrDefaultAsync(x => x.Id == watchlistId);
        if (wl == null) return Result<WatchlistItemDto>.NotFound("واچ‌لیست یافت نشد");
        if (wl.OwnerUserId != _user.UserId.Value) return Result<WatchlistItemDto>.Failure(new Error("INCORRECT_USER", "اجازه افزودن ندارید", ApplicationLayer.Common.Enums.RequestStatus.IncorrectUser));
        var crypto = await _cryptos.GetDbSet().FirstOrDefaultAsync(x => x.Symbol == symbol);
        if (crypto == null) return Result<WatchlistItemDto>.NotFound("نماد یافت نشد");
        var exists = await _items.Query().Where(x => x.WatchlistId == watchlistId && x.Symbol == crypto.Symbol).AnyAsync();
        if (exists)
        {
            var it = await _items.GetDbSet().FirstOrDefaultAsync(x => x.WatchlistId == watchlistId && x.Symbol == crypto.Symbol);
            return Result<WatchlistItemDto>.Success(await ToItemDtoAsync(it));
        }
        var entity = new WatchlistItem { WatchlistId = watchlistId, Symbol = crypto.Symbol };
        await _uow.BeginTransactionAsync();
        await _items.AddAsync(entity);
        await _uow.SaveChangesAsync();
        await _uow.CommitAsync();
        return Result<WatchlistItemDto>.Success(await ToItemDtoAsync(entity));
    }

    public async Task<Result> RemoveItemAsync(int itemId)
    {
        if (_user.UserId == null) return Result.AuthenticationFailure();
        var entity = await _items.GetDbSet().FirstOrDefaultAsync(x => x.Id == itemId);
        if (entity == null) return Result.Success();
        var wl = await _lists.GetDbSet().FirstOrDefaultAsync(x => x.Id == entity.WatchlistId);
        if (wl == null || wl.OwnerUserId != _user.UserId.Value) return Result.Failure(new Error("INCORRECT_USER", "اجازه حذف ندارید", ApplicationLayer.Common.Enums.RequestStatus.IncorrectUser));
        await _uow.BeginTransactionAsync();
        _items.Remove(entity);
        await _uow.SaveChangesAsync();
        await _uow.CommitAsync();
        return Result.Success();
    }

    private async Task<WatchlistItemDto> ToItemDtoAsync(WatchlistItem entity)
    {
        var crypto = await _cryptos.GetDbSet().FirstOrDefaultAsync(x => x.Symbol == entity.Symbol);
        var last = crypto == null ? null : await _candles_1m.Query().Where(c => c.CryptocurrencyId == crypto.Id).OrderByDescending(c => c.CloseTime).FirstOrDefaultAsync();
        var price = last?.Close ?? 0m;
        return new WatchlistItemDto { Id = entity.Id, Symbol = entity.Symbol, CurrentPrice = price, UpdatedAt = DateTime.UtcNow };
    }

    private static WatchlistDto ToListDto(Watchlist wl, List<WatchlistItemDto> items)
    {
        return new WatchlistDto { Id = wl.Id, Name = wl.Name, IsActive = wl.IsActive, Items = items };
    }
}