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

    public async Task<Result<PortfolioPositionsPageDto>> GetPositionsAsync(GetPortfolioRequestDto dto)
    {
        if (_user.UserId == null) return Result<PortfolioPositionsPageDto>.AuthenticationFailure();
        var uid = _user.UserId.Value;
        var query = _entries.Query().Where(x => Microsoft.EntityFrameworkCore.EF.Property<int?>(x, "CreatedByUserId") == uid && x.IsActive);
        var grouped = await query.GroupBy(x => new { x.CryptocurrencyId, x.Symbol })
            .Select(g => new
            {
                g.Key.CryptocurrencyId,
                Symbol = g.Key.Symbol,
                TotalQuantity = g.Sum(x => x.Quantity),
                TotalCost = g.Sum(x => x.Quantity * x.BuyPrice),
                AverageBuyPrice = g.Sum(x => x.Quantity * x.BuyPrice) / g.Sum(x => x.Quantity)
            })
            .OrderByDescending(x => x.TotalQuantity)
            .Skip((dto.Page - 1) * dto.PageSize)
            .Take(dto.PageSize)
            .ToListAsync();

        var total = await query.Select(x => x.Symbol).Distinct().CountAsync();
        var items = new List<PortfolioPositionDto>();
        foreach (var p in grouped)
        {
            var last = await _candles_1m.Query().Where(c => c.CryptocurrencyId == p.CryptocurrencyId).OrderByDescending(c => c.CloseTime).FirstOrDefaultAsync();
            var currentPrice = last?.Close ?? 0m;
            var currentValue = p.TotalQuantity * currentPrice;
            var pnl = currentValue - p.TotalCost;
            var pnlPct = p.TotalCost == 0 ? 0 : (pnl / p.TotalCost) * 100m;
            items.Add(new PortfolioPositionDto
            {
                Symbol = p.Symbol,
                TotalQuantity = p.TotalQuantity,
                AverageBuyPrice = decimal.Round(p.AverageBuyPrice, 8),
                TotalCost = decimal.Round(p.TotalCost, 8),
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