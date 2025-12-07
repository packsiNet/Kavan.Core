using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.DTOs.Portfolio;
using ApplicationLayer.Interfaces;
using ApplicationLayer.Interfaces.Portfolio;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureLayer.BusinessLogic.Services.Portfolio;

[InjectAsScoped]
public class PortfolioService(IUnitOfWork _uow,
                              IRepository<PortfolioEntry> _entries,
                              IRepository<PortfolioSale> _sales,
                              IRepository<Cryptocurrency> _cryptos,
                              IRepository<Candle_1m> _candles_1m,
                              IUserContextService _user) : IPortfolioService
{
    public async Task<Result<PortfolioEntryDto>> AddEntryAsync(CreatePortfolioEntryDto dto)
    {
        if (_user.UserId == null) return Result<PortfolioEntryDto>.AuthenticationFailure();
        if (string.IsNullOrWhiteSpace(dto.Symbol)) return Result<PortfolioEntryDto>.ValidationFailure("نماد الزامی است");
        if (dto.Quantity <= 0) return Result<PortfolioEntryDto>.ValidationFailure("تعداد باید بزرگ‌تر از صفر باشد");
        if (dto.BuyPrice <= 0) return Result<PortfolioEntryDto>.ValidationFailure("قیمت خرید باید بزرگ‌تر از صفر باشد");
        var crypto = await _cryptos.GetDbSet().FirstOrDefaultAsync(x => x.Symbol == dto.Symbol);
        if (crypto == null) return Result<PortfolioEntryDto>.NotFound("رمزارز یافت نشد");

        var entity = new PortfolioEntry
        {
            CryptocurrencyId = crypto.Id,
            Symbol = crypto.Symbol,
            Quantity = dto.Quantity,
            BuyPrice = dto.BuyPrice,
            BuyDate = dto.BuyDate ?? DateTime.UtcNow
        };

        await _uow.BeginTransactionAsync();
        await _entries.AddAsync(entity);
        await _uow.SaveChangesAsync();
        await _uow.CommitAsync();

        return Result<PortfolioEntryDto>.Success(ToEntryDto(entity));
    }

    public async Task<Result<PortfolioEntryDto>> UpdateEntryAsync(int id, UpdatePortfolioEntryDto dto)
    {
        if (_user.UserId == null) return Result<PortfolioEntryDto>.AuthenticationFailure();
        var entity = await _entries.GetDbSet().FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) return Result<PortfolioEntryDto>.NotFound("ورودی یافت نشد");
        var ownerId = Microsoft.EntityFrameworkCore.EF.Property<int?>(entity, "CreatedByUserId");
        if (ownerId != _user.UserId) return Result<PortfolioEntryDto>.Failure(new Error("INCORRECT_USER", "اجازه ویرایش ندارید", ApplicationLayer.Common.Enums.RequestStatus.IncorrectUser));

        if (dto.Quantity <= 0) return Result<PortfolioEntryDto>.ValidationFailure("تعداد باید بزرگ‌تر از صفر باشد");
        if (dto.BuyPrice <= 0) return Result<PortfolioEntryDto>.ValidationFailure("قیمت خرید باید بزرگ‌تر از صفر باشد");

        entity.Quantity = dto.Quantity;
        entity.BuyPrice = dto.BuyPrice;
        if (dto.BuyDate != null) entity.BuyDate = dto.BuyDate.Value;
        entity.IsActive = dto.IsActive;
        entity.MarkAsUpdated();

        await _uow.BeginTransactionAsync();
        await _entries.UpdateAsync(entity);
        await _uow.SaveChangesAsync();
        await _uow.CommitAsync();
        return Result<PortfolioEntryDto>.Success(ToEntryDto(entity));
    }

    public async Task<Result> DeleteEntryAsync(int id)
    {
        if (_user.UserId == null) return Result.AuthenticationFailure();
        var entity = await _entries.GetDbSet().FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) return Result.NotFound("ورودی یافت نشد");
        var ownerId = Microsoft.EntityFrameworkCore.EF.Property<int?>(entity, "CreatedByUserId");
        if (ownerId != _user.UserId) return Result.Failure(new Error("INCORRECT_USER", "اجازه حذف ندارید", ApplicationLayer.Common.Enums.RequestStatus.IncorrectUser));
        await _uow.BeginTransactionAsync();
        _entries.Remove(entity);
        await _uow.SaveChangesAsync();
        await _uow.CommitAsync();
        return Result.Success();
    }

    public async Task<Result> DeleteSymbolAsync(string symbol)
    {
        if (_user.UserId == null) return Result.AuthenticationFailure();
        var uid = _user.UserId.Value;
        var entries = await _entries.GetDbSet().Where(x => EF.Property<int?>(x, "CreatedByUserId") == uid && x.Symbol == symbol).ToListAsync();
        var sales = await _sales.GetDbSet().Where(x => EF.Property<int?>(x, "CreatedByUserId") == uid && x.Symbol == symbol).ToListAsync();
        if (entries.Count == 0 && sales.Count == 0) return Result.NotFound("برای این نماد رکوردی یافت نشد");
        await _uow.BeginTransactionAsync();
        foreach (var e in entries) _entries.Remove(e);
        foreach (var s in sales) _sales.Remove(s);
        await _uow.SaveChangesAsync();
        await _uow.CommitAsync();
        return Result.Success();
    }

    public async Task<Result<PortfolioPositionsPageDto>> GetPositionsAsync(GetPortfolioRequestDto dto)
    {
        if (_user.UserId == null) return Result<PortfolioPositionsPageDto>.AuthenticationFailure();
        var uid = _user.UserId.Value;
        var buyQuery = _entries.Query().Where(x => EF.Property<int?>(x, "CreatedByUserId") == uid && x.IsActive);
        var sellQuery = _sales.Query().Where(x => EF.Property<int?>(x, "CreatedByUserId") == uid && x.IsActive);

        var groupedBuys = await buyQuery.GroupBy(x => new { x.CryptocurrencyId, x.Symbol })
            .Select(g => new
            {
                g.Key.CryptocurrencyId,
                Symbol = g.Key.Symbol,
                BuyQuantity = g.Sum(x => x.Quantity),
                BuyCost = g.Sum(x => x.Quantity * x.BuyPrice),
            })
            .OrderByDescending(x => x.BuyQuantity)
            .Skip((dto.Page - 1) * dto.PageSize)
            .Take(dto.PageSize)
            .ToListAsync();

        var total = await buyQuery.Select(x => x.Symbol).Distinct().CountAsync();
        var items = new List<PortfolioPositionDto>();
        foreach (var b in groupedBuys)
        {
            var soldQty = await sellQuery.Where(s => s.CryptocurrencyId == b.CryptocurrencyId).SumAsync(s => (decimal?)s.Quantity) ?? 0m;
            var remainingQty = b.BuyQuantity - soldQty;
            if (remainingQty < 0) remainingQty = 0;
            var avgBuy = b.BuyQuantity == 0 ? 0m : (b.BuyCost / b.BuyQuantity);
            var costRemaining = avgBuy * remainingQty;

            var last = await _candles_1m.Query().Where(c => c.CryptocurrencyId == b.CryptocurrencyId).OrderByDescending(c => c.CloseTime).FirstOrDefaultAsync();
            var currentPrice = last?.Close ?? 0m;
            var currentValue = remainingQty * currentPrice;
            var pnl = currentValue - costRemaining;
            var pnlPct = costRemaining == 0 ? 0 : (pnl / costRemaining) * 100m;
            items.Add(new PortfolioPositionDto
            {
                Symbol = b.Symbol,
                TotalQuantity = decimal.Round(remainingQty, 8),
                AverageBuyPrice = decimal.Round(avgBuy, 8),
                TotalCost = decimal.Round(costRemaining, 8),
                CurrentPrice = decimal.Round(currentPrice, 8),
                CurrentValue = decimal.Round(currentValue, 8),
                ProfitLoss = decimal.Round(pnl, 8),
                ProfitLossPercent = decimal.Round(pnlPct, 4)
            });
        }

        var page = new PortfolioPositionsPageDto { Items = items, Total = total, Page = dto.Page, PageSize = dto.PageSize };
        return Result<PortfolioPositionsPageDto>.Success(page);
    }

    public async Task<Result<PortfolioEntriesPageDto>> GetEntriesBySymbolAsync(string symbol, GetPortfolioEntriesRequestDto dto)
    {
        if (_user.UserId == null) return Result<PortfolioEntriesPageDto>.AuthenticationFailure();
        var uid = _user.UserId.Value;
        var query = _entries.Query().Where(x => Microsoft.EntityFrameworkCore.EF.Property<int?>(x, "CreatedByUserId") == uid && x.Symbol == symbol && x.IsActive);
        var total = await query.CountAsync();
        var list = await query.OrderByDescending(x => x.BuyDate).Skip((dto.Page - 1) * dto.PageSize).Take(dto.PageSize).Select(x => ToEntryDto(x)).ToListAsync();
        var page = new PortfolioEntriesPageDto { Items = list, Total = total, Page = dto.Page, PageSize = dto.PageSize };
        return Result<PortfolioEntriesPageDto>.Success(page);
    }

    public async Task<Result> AddSaleAsync(CreatePortfolioSaleDto dto)
    {
        if (_user.UserId == null) return Result.AuthenticationFailure();
        if (string.IsNullOrWhiteSpace(dto.Symbol)) return Result.ValidationFailure("نماد الزامی است");
        if (dto.Quantity <= 0) return Result.ValidationFailure("تعداد فروش باید بزرگ‌تر از صفر باشد");
        if (dto.SellPrice <= 0) return Result.ValidationFailure("قیمت فروش باید بزرگ‌تر از صفر باشد");

        var crypto = await _cryptos.GetDbSet().FirstOrDefaultAsync(x => x.Symbol == dto.Symbol);
        if (crypto == null) return Result.NotFound("رمزارز یافت نشد");

        var uid = _user.UserId.Value;
        var buyQty = await _entries.Query().Where(x => EF.Property<int?>(x, "CreatedByUserId") == uid && x.Symbol == dto.Symbol && x.IsActive).SumAsync(x => (decimal?)x.Quantity) ?? 0m;
        var sellQty = await _sales.Query().Where(x => EF.Property<int?>(x, "CreatedByUserId") == uid && x.Symbol == dto.Symbol && x.IsActive).SumAsync(x => (decimal?)x.Quantity) ?? 0m;
        var remaining = buyQty - sellQty;
        if (dto.Quantity > remaining) return Result.ValidationFailure("تعداد فروش از موجودی بیشتر است");

        var entity = new PortfolioSale
        {
            CryptocurrencyId = crypto.Id,
            Symbol = crypto.Symbol,
            Quantity = dto.Quantity,
            SellPrice = dto.SellPrice,
            SellDate = dto.SellDate ?? DateTime.UtcNow
        };

        await _uow.BeginTransactionAsync();
        await _sales.AddAsync(entity);
        await _uow.SaveChangesAsync();
        await _uow.CommitAsync();
        return Result.Success();
    }

    private static PortfolioEntryDto ToEntryDto(PortfolioEntry e)
    {
        return new PortfolioEntryDto
        {
            Id = e.Id,
            Symbol = e.Symbol,
            Quantity = e.Quantity,
            BuyPrice = e.BuyPrice,
            BuyDate = e.BuyDate,
            CreatedAt = e.CreatedAt
        };
    }
}
