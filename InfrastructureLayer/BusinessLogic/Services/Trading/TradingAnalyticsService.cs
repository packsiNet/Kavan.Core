using ApplicationLayer.Dto.Analytics;
using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.Interfaces;
using ApplicationLayer.Interfaces.Services;
using DomainLayer.Common.Attributes;
using DomainLayer.Common.Enums;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureLayer.BusinessLogic.Services.Trading;

[InjectAsScoped]
public class TradingAnalyticsService(
    IRepository<Trade> _tradeRepository,
    IUserContextService _userContext
) : ITradingAnalyticsService
{
    public async Task<Result<PeriodSummaryDto>> GetPeriodSummaryAsync(int periodId)
    {
        var userId = _userContext.UserId;
        if (userId == null) return Result<PeriodSummaryDto>.Failure(Error.Authentication("User not authenticated"));

        var trades = await _tradeRepository.Query()
            .Where(x => x.FinancialPeriodId == periodId && x.UserId == userId && x.Status == TradeStatus.Closed)
            .ToListAsync();

        if (!trades.Any())
        {
            return Result<PeriodSummaryDto>.Success(new PeriodSummaryDto { FinancialPeriodId = periodId });
        }

        var totalTrades = trades.Count;
        var winningTrades = trades.Count(x => x.Result.PnLPercent > 0);
        var losingTrades = trades.Count(x => x.Result.PnLPercent < 0);
        var breakEvenTrades = trades.Count(x => x.Result.PnLPercent == 0);

        var winRate = (decimal)winningTrades / totalTrades * 100;
        
        var avgWin = winningTrades > 0 ? trades.Where(x => x.Result.PnLPercent > 0).Average(x => x.Result.PnLPercent) ?? 0 : 0;
        var avgLoss = losingTrades > 0 ? trades.Where(x => x.Result.PnLPercent < 0).Average(x => x.Result.PnLPercent) ?? 0 : 0;
        
        // Expectancy = (WinRate * AvgWin) - (LossRate * AvgLoss)
        // Note: AvgLoss is usually negative, so we use absolute value for formula or add if negative
        var lossRate = (decimal)losingTrades / totalTrades;
        var expectancy = (winRate / 100 * avgWin) + (lossRate * avgLoss); // avgLoss is negative

        var avgR = trades.Average(x => x.Result.RMultiple) ?? 0;
        
        // Profit Factor = Gross Profit / Gross Loss
        var grossProfit = trades.Where(x => x.Result.PnLPercent > 0).Sum(x => x.Result.PnLPercent) ?? 0;
        var grossLoss = Math.Abs(trades.Where(x => x.Result.PnLPercent < 0).Sum(x => x.Result.PnLPercent) ?? 0);
        var profitFactor = grossLoss == 0 ? grossProfit : grossProfit / grossLoss;

        // Max Drawdown (Simplified: Max accumulated loss from peak)
        // This requires time-series calculation, simplified here as max single loss or simple cumulative drawdown
        // Better implementation: Calculate cumulative PnL curve and find max drop
        decimal maxDrawdown = 0;
        decimal peak = 0;
        decimal cumulative = 0;

        // Sort by close time
        var sortedTrades = trades.OrderBy(x => x.ClosedAtUtc).ToList();
        foreach (var trade in sortedTrades)
        {
            cumulative += trade.Result.PnLPercent ?? 0;
            if (cumulative > peak) peak = cumulative;
            
            var dd = peak - cumulative;
            if (dd > maxDrawdown) maxDrawdown = dd;
        }

        var summary = new PeriodSummaryDto
        {
            FinancialPeriodId = periodId,
            TotalTrades = totalTrades,
            WinningTrades = winningTrades,
            LosingTrades = losingTrades,
            BreakEvenTrades = breakEvenTrades,
            WinRate = Math.Round(winRate, 2),
            Expectancy = Math.Round(expectancy, 2),
            AvgR = Math.Round(avgR, 2),
            ProfitFactor = Math.Round(profitFactor, 2),
            MaxDrawdown = Math.Round(maxDrawdown, 2),
            TotalPnLPercent = Math.Round(cumulative, 2)
        };

        return Result<PeriodSummaryDto>.Success(summary);
    }

    public async Task<Result<PeriodBehaviorDto>> GetPeriodBehaviorAsync(int periodId)
    {
        var userId = _userContext.UserId;
        if (userId == null) return Result<PeriodBehaviorDto>.Failure(Error.Authentication("User not authenticated"));

        var trades = await _tradeRepository.Query()
            .Where(x => x.FinancialPeriodId == periodId && x.UserId == userId && x.Status == TradeStatus.Closed)
            .ToListAsync();

        var behavior = new PeriodBehaviorDto
        {
            FinancialPeriodId = periodId,
            TradesBySide = trades.GroupBy(x => x.Side)
                .ToDictionary(g => TradeSide.FromValue(g.Key).Name, g => g.Count()),
            TradesByReason = trades.GroupBy(x => x.Result.ExitReason)
                .Where(g => g.Key.HasValue)
                .ToDictionary(g => ExitReason.FromValue(g.Key.Value).Name, g => g.Count()),
            AvgLeverage = trades.Any() ? Math.Round((decimal)trades.Average(x => x.Leverage), 1) : 0
        };

        if (trades.Any())
        {
            // Average Holding Time
            var totalSeconds = trades.Sum(x => x.Result.HoldingTime?.TotalSeconds ?? 0);
            behavior.AvgHoldingTime = TimeSpan.FromSeconds(totalSeconds / trades.Count);
        }

        return Result<PeriodBehaviorDto>.Success(behavior);
    }

    public async Task<Result<PeriodInsightsDto>> GetPeriodInsightsAsync(int periodId)
    {
        var userId = _userContext.UserId;
        if (userId == null) return Result<PeriodInsightsDto>.Failure(Error.Authentication("User not authenticated"));

        var trades = await _tradeRepository.Query()
            .Where(x => x.FinancialPeriodId == periodId && x.UserId == userId && x.Status == TradeStatus.Closed)
            .ToListAsync();

        var insights = new List<InsightItemDto>();

        if (trades.Any())
        {
            // Rule 1: Plan Compliance
            var complianceRate = (double)trades.Count(x => x.Emotion.PlanCompliance) / trades.Count;
            if (complianceRate < 0.8)
            {
                insights.Add(new InsightItemDto
                {
                    Type = "Warning",
                    RuleName = "PlanCompliance",
                    Message = $"Low plan compliance detected ({complianceRate:P0}). Stick to your trading plan to improve consistency."
                });
            }

            // Rule 2: Over-trading check (Simplified)
            // If many trades have short holding time and are losses
            var quickLosses = trades.Count(x => x.Result.PnLPercent < 0 && x.Result.HoldingTime < TimeSpan.FromMinutes(15));
            if (quickLosses > 5) // Arbitrary threshold
            {
                insights.Add(new InsightItemDto
                {
                    Type = "Warning",
                    RuleName = "OverTrading",
                    Message = "High number of quick losses detected. You might be tilting or scalping aggressively."
                });
            }

            // Rule 3: WinRate vs R:R
            var winRate = (double)trades.Count(x => x.Result.PnLPercent > 0) / trades.Count;
            var avgR = trades.Average(x => x.Result.RMultiple) ?? 0;
            
            if (winRate < 0.4 && avgR < 1.5m)
            {
                insights.Add(new InsightItemDto
                {
                    Type = "Optimization",
                    RuleName = "SystemQuality",
                    Message = "Low WinRate with low Risk/Reward ratio. Consider improving entry filters or extending targets."
                });
            }
        }
        else
        {
            insights.Add(new InsightItemDto
            {
                Type = "Info",
                RuleName = "NoData",
                Message = "Not enough data to generate insights yet."
            });
        }

        return Result<PeriodInsightsDto>.Success(new PeriodInsightsDto
        {
            FinancialPeriodId = periodId,
            Insights = insights
        });
    }
}
