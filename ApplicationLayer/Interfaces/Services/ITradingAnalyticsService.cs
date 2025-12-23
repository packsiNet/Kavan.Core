using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.Dto.Analytics;

namespace ApplicationLayer.Interfaces.Services;

public interface ITradingAnalyticsService
{
    Task<Result<PeriodSummaryDto>> GetPeriodSummaryAsync(int periodId);
    Task<Result<PeriodBehaviorDto>> GetPeriodBehaviorAsync(int periodId);
    Task<Result<PeriodInsightsDto>> GetPeriodInsightsAsync(int periodId);
}
