using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.Dto.Trade;
using ApplicationLayer.Interfaces;
using ApplicationLayer.Interfaces.Services;
using ApplicationLayer.Interfaces.External;
using DomainLayer.Common.Attributes;
using DomainLayer.Common.Enums;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureLayer.BusinessLogic.Services.Trading;

[InjectAsScoped]
public class TradeService(
    IRepository<Trade> _tradeRepository,
    IRepository<TradeTp> _tpRepository,
    IRepository<FinancialPeriod> _periodRepository,
    IUserContextService _userContext,
    IUnitOfWork _uow,
    IMarketDataProxyService _marketDataService
) : ITradeService
{
    public async Task<Result<TradeDto>> CreateTradeAsync(CreateTradeDto dto)
    {
        var userId = _userContext.UserId;
        if (userId == null) return Result<TradeDto>.Failure(Error.Authentication("User not authenticated"));

        // 1. Validate Active Financial Period
        var activePeriod = await _periodRepository.Query()
            .FirstOrDefaultAsync(x => x.UserAccountId == userId && !x.IsClosed);

        if (activePeriod == null)
            return Result<TradeDto>.Failure(Error.Validation("Active financial period not found"));

        // 2. Create Trade Entity
        var entity = new Trade
        {
            UserAccountId = userId.Value,
            FinancialPeriodId = activePeriod.Id,
            Symbol = dto.Symbol.ToUpper(),
            Side = dto.Side,
            EntryPrice = dto.EntryPrice,
            StopLoss = dto.StopLoss,
            Quantity = dto.Quantity,
            Leverage = dto.Leverage,
            Status = TradeStatus.Open,
            OpenedAtUtc = DateTime.UtcNow,
            Emotion = new TradeEmotion
            {
                ConfidenceLevel = dto.ConfidenceLevel,
                EmotionBeforeEntry = dto.EmotionBeforeEntry,
                PlanCompliance = dto.PlanCompliance
            },
            Result = new TradeResult(), // Empty initially
            TakeProfits = dto.TakeProfits.Select(price => new TradeTp
            {
                Price = price,
                IsHit = false
            }).ToList()
        };

        await _tradeRepository.AddAsync(entity);
        await _uow.SaveChangesAsync();

        return Result<TradeDto>.Success(MapToDto(entity));
    }

    public async Task<Result<List<TradeDto>>> GetTradesByPeriodAsync(int periodId)
    {
        var userId = _userContext.UserId;
        if (userId == null) return Result<List<TradeDto>>.Failure(Error.Authentication("User not authenticated"));

        var trades = await _tradeRepository.Query()
            .Include(x => x.TakeProfits)
            .Where(x => x.FinancialPeriodId == periodId && x.UserAccountId == userId)
            .OrderByDescending(x => x.OpenedAtUtc)
            .ToListAsync();

        var dtos = trades.Select(MapToDto).ToList();
        return Result<List<TradeDto>>.Success(dtos);
    }

    public async Task<Result<TradeDto>> CloseTradeAsync(int tradeId)
    {
        // 1. Fetch trade
        var userId = _userContext.UserId;
        var trade = await _tradeRepository.Query()
            .Include(x => x.TakeProfits)
            .FirstOrDefaultAsync(x => x.Id == tradeId);

        if (trade == null) return Result<TradeDto>.Failure(Error.NotFound("Trade not found"));
        if (userId != null && trade.UserAccountId != userId) return Result<TradeDto>.Failure(Error.AccessDenied());

        // 2. Get Market Price
        decimal currentPrice;
        try
        {
             currentPrice = await _marketDataService.GetCurrentPriceAsync(trade.Symbol);
        }
        catch (Exception ex)
        {
             return Result<TradeDto>.Failure(Error.Validation($"Failed to get market price: {ex.Message}"));
        }

        // 3. Delegate to Domain
        try
        {
            trade.CloseByMarket(currentPrice, ExitReason.ManualExit);
        }
        catch (InvalidOperationException ex)
        {
            return Result<TradeDto>.Failure(Error.Validation(ex.Message));
        }

        await _tradeRepository.UpdateAsync(trade);
        await _uow.SaveChangesAsync();

        return Result<TradeDto>.Success(MapToDto(trade));
    }

    public async Task<Result<TradeDto>> CloseTradeInternalAsync(int tradeId, decimal exitPrice, ExitReason reason)
    {
        // Used by System/Monitoring Service (already trusted price/reason)
        var trade = await _tradeRepository.Query()
             .Include(x => x.TakeProfits)
             .FirstOrDefaultAsync(x => x.Id == tradeId);
 
        if (trade == null) return Result<TradeDto>.Failure(Error.NotFound("Trade not found"));

        try
        {
            trade.CloseByMarket(exitPrice, reason);
        }
        catch (InvalidOperationException ex)
        {
             // Idempotency check: if already closed, maybe just return Success or ignore
             if (trade.Status == TradeStatus.Closed) return Result<TradeDto>.Success(MapToDto(trade));
             return Result<TradeDto>.Failure(Error.Validation(ex.Message));
        }

        await _tradeRepository.UpdateAsync(trade);
        await _uow.SaveChangesAsync();

        return Result<TradeDto>.Success(MapToDto(trade));
    }

    public async Task<Result<TradeDto>> UpdateTradeAsync(UpdateTradeDto dto)
    {
        var userId = _userContext.UserId;
        var trade = await _tradeRepository.Query()
             .Include(x => x.TakeProfits)
             .FirstOrDefaultAsync(x => x.Id == dto.TradeId);

        if (trade == null) return Result<TradeDto>.Failure(Error.NotFound("Trade not found"));
        if (userId != null && trade.UserAccountId != userId) return Result<TradeDto>.Failure(Error.AccessDenied());

        // Update Emotion/Notes directly
        if (dto.ConfidenceLevel.HasValue) trade.Emotion.ConfidenceLevel = dto.ConfidenceLevel.Value;
        if (dto.EmotionBeforeEntry != null) trade.Emotion.EmotionBeforeEntry = dto.EmotionBeforeEntry;
        if (dto.PlanCompliance.HasValue) trade.Emotion.PlanCompliance = dto.PlanCompliance.Value;

        // Use Domain Logic for Risk Levels
        try
        {
            if (dto.StopLoss.HasValue || dto.TakeProfits != null)
            {
                 // Need to handle TP collection updates via Repository if EF Core doesn't track properly 
                 // (but since we Loaded with Include, simple collection manipulation in Domain works for tracking usually)
                 // However, we need to be careful with orphan deletion if we just clear list.
                 // EF Core with Owned types or related entities:
                 // TradeTp is an entity (has Id). We need to handle removal.
                 
                 // If we call UpdateRiskLevels, it clears the list.
                 // We should ensure the old TPs are deleted from DB.
                 if (dto.TakeProfits != null)
                 {
                     _tpRepository.RemoveRange(trade.TakeProfits);
                 }
                 
                 trade.UpdateRiskLevels(dto.StopLoss, dto.TakeProfits);
            }
        }
        catch (InvalidOperationException ex)
        {
             return Result<TradeDto>.Failure(Error.Validation(ex.Message));
        }

        await _tradeRepository.UpdateAsync(trade);
        await _uow.SaveChangesAsync();

        return Result<TradeDto>.Success(MapToDto(trade));
    }

    public async Task<Result<bool>> CancelTradeAsync(int tradeId, string reason)
    {
        var userId = _userContext.UserId;
        var trade = await _tradeRepository.Query()
            .FirstOrDefaultAsync(x => x.Id == tradeId);

        if (trade == null) return Result<bool>.Failure(Error.NotFound("Trade not found"));
        if (userId != null && trade.UserAccountId != userId) return Result<bool>.Failure(Error.AccessDenied());

        try
        {
            trade.Cancel(reason);
        }
        catch (InvalidOperationException ex)
        {
            return Result<bool>.Failure(Error.Validation(ex.Message));
        }

        await _tradeRepository.UpdateAsync(trade);
        await _uow.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    // DeleteTradeAsync removed or kept for admin only? User prompt says "Physical Delete Forbidden".
    // I will remove it to be safe, as "Cancel" is the replacement.
    /*
    public async Task<Result<bool>> DeleteTradeAsync(int tradeId)
    {
         // Removed per policy
         throw new NotImplementedException("Physical delete is forbidden. Use Cancel.");
    }
    */

    public async Task CheckOpenTradesAsync()
    {
        var openTrades = await _tradeRepository.Query()
            .Include(x => x.TakeProfits)
            .Where(x => x.Status == TradeStatus.Open || x.Status == TradeStatus.PartiallyClosed)
            .ToListAsync();

        foreach (var trade in openTrades)
        {
            try
            {
                var currentPrice = await _marketDataService.GetCurrentPriceAsync(trade.Symbol);
                var side = trade.SideEnum;

                // 1. Check Stop Loss
                bool slHit = side == TradeSide.Long
                    ? currentPrice <= trade.StopLoss
                    : currentPrice >= trade.StopLoss;

                if (slHit)
                {
                    trade.CloseByMarket(trade.StopLoss, ExitReason.StopLoss);
                }
                else
                {
                    // 2. Check Take Profits
                    var unhitTps = trade.TakeProfits.Where(x => !x.IsHit).ToList();
                    bool anyNewHit = false;

                    foreach (var tp in unhitTps)
                    {
                        bool tpHit = side == TradeSide.Long
                           ? currentPrice >= tp.Price
                           : currentPrice <= tp.Price;

                        if (tpHit)
                        {
                            tp.IsHit = true;
                            anyNewHit = true;
                        }
                    }

                    // Check if all TPs are hit OR the furthest TP is hit
                    // Logic: If all registered TPs are hit, close the trade.
                    if (trade.TakeProfits.Any() && trade.TakeProfits.All(x => x.IsHit))
                    {
                        trade.CloseByMarket(currentPrice, ExitReason.TakeProfit);
                    }
                }

                await _tradeRepository.UpdateAsync(trade);
            }
            catch (Exception)
            {
                // Continue checking other trades even if one fails (e.g. market data error)
                continue;
            }
        }

        await _uow.SaveChangesAsync();
    }

    public async Task<Result<List<TradeCalendarDto>>> GetTradeCalendarAsync(int? periodId)
    {
        var userId = _userContext.UserId;
        if (userId == null) return Result<List<TradeCalendarDto>>.Failure(Error.Authentication("User not authenticated"));

        var query = _tradeRepository.Query()
            .Where(x => x.UserAccountId == userId && x.Status == TradeStatus.Closed.Value && x.ClosedAtUtc != null);

        if (periodId.HasValue)
        {
            query = query.Where(x => x.FinancialPeriodId == periodId.Value);
        }

        var trades = await query.ToListAsync();

        var grouped = trades
            .GroupBy(x => x.ClosedAtUtc.Value.Date)
            .Select(g => new TradeCalendarDto
            {
                Date = g.Key,
                TradeCount = g.Count(),
                TotalPnLPercent = g.Sum(x => x.Result.PnLPercent ?? 
                    ((x.Result.ExitPrice.HasValue && x.EntryPrice != 0) 
                        ? ((x.Result.ExitPrice.Value - x.EntryPrice) / x.EntryPrice) * 100 * x.Leverage * (x.Side == TradeSide.Long ? 1 : -1) 
                        : 0)),
                TotalPnLAmount = g.Sum(x => x.Result.PnL ?? 
                    ((x.Result.ExitPrice.HasValue) 
                        ? (x.Result.ExitPrice.Value - x.EntryPrice) * x.Quantity * (x.Side == TradeSide.Long ? 1 : -1) 
                        : 0))
            })
            .OrderByDescending(x => x.Date)
            .ToList();

        return Result<List<TradeCalendarDto>>.Success(grouped);
    }

    private static TradeDto MapToDto(Trade entity)
    {
        return new TradeDto
        {
            Id = entity.Id,
            UserId = entity.UserAccountId,
            Symbol = entity.Symbol,
            Side = entity.Side,
            SideName = TradeSide.FromValue(entity.Side).Name,
            EntryPrice = entity.EntryPrice,
            StopLoss = entity.StopLoss,
            Quantity = entity.Quantity,
            Leverage = entity.Leverage,
            Status = entity.Status,
            StatusName = TradeStatus.FromValue(entity.Status).Name,
            OpenedAtUtc = entity.OpenedAtUtc,
            ClosedAtUtc = entity.ClosedAtUtc,
            FinancialPeriodId = entity.FinancialPeriodId,
            Emotion = new TradeEmotionDto
            {
                ConfidenceLevel = entity.Emotion.ConfidenceLevel,
                EmotionBeforeEntry = entity.Emotion.EmotionBeforeEntry,
                PlanCompliance = entity.Emotion.PlanCompliance
            },
            Result = new TradeResultDto
            {
                ExitPrice = entity.Result.ExitPrice,
                ExitReason = entity.Result.ExitReason,
                ExitReasonName = entity.Result.ExitReason.HasValue ? ExitReason.FromValue(entity.Result.ExitReason.Value).Name : null,
                RMultiple = entity.Result.RMultiple,
                PnLPercent = entity.Result.PnLPercent ?? 
                             ((entity.Result.ExitPrice.HasValue && entity.EntryPrice != 0) 
                                 ? ((entity.Result.ExitPrice.Value - entity.EntryPrice) / entity.EntryPrice) * 100 * entity.Leverage * (entity.Side == TradeSide.Long ? 1 : -1) 
                                 : null),
                PnL = entity.Result.PnL ?? 
                      ((entity.Result.ExitPrice.HasValue) 
                          ? (entity.Result.ExitPrice.Value - entity.EntryPrice) * entity.Quantity * (entity.Side == TradeSide.Long ? 1 : -1) 
                          : null),
                HoldingTime = entity.Result.HoldingTime
            },
            TakeProfits = [.. entity.TakeProfits.Select(tp => new TradeTpDto
            {
                Price = tp.Price,
                IsHit = tp.IsHit
            })]
        };
    }
}
