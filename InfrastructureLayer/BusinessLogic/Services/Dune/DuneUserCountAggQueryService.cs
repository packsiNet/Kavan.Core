using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.Dto.BitcoinETF;
using ApplicationLayer.Interfaces;
using ApplicationLayer.Interfaces.Services.Dune;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureLayer.BusinessLogic.Services.Dune;

[InjectAsScoped]
public class DuneUserCountAggQueryService : IDuneUserCountAggQueryService
{
    private readonly IRepository<BitcoinActiveAddress> _repo;

    public DuneUserCountAggQueryService(IRepository<BitcoinActiveAddress> repo)
    {
        _repo = repo;
    }

    public async Task<Result<BitcoinUserCountsResponseDto>> GetAggregatedAsync(CancellationToken cancellationToken)
    {
        var rows = await _repo.GetDbSet()
            .IgnoreQueryFilters()
            .Where(x => !x.IsDeleted)
            .OrderByDescending(x => x.Time)
            .Select(x => new { x.Time, x.Users })
            .ToListAsync(cancellationToken);

        if (rows.Count == 0)
            return Result<BitcoinUserCountsResponseDto>.NotFound("No user counts found");

        var daily = rows.Select(r => new BitcoinUserCountDailyDto { Time = r.Time, Users = r.Users }).ToList();

        static DateTime WeekStartMonday(DateTime dt)
        {
            var date = dt.Date;
            var diff = ((int)date.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
            return date.AddDays(-diff);
        }

        var weeklyGroups = rows
            .GroupBy(r => WeekStartMonday(r.Time))
            .OrderBy(g => g.Key)
            .Select(g => new BitcoinUserCountWeeklyDto
            {
                WeekStart = DateTime.SpecifyKind(g.Key, DateTimeKind.Utc),
                WeekEnd = DateTime.SpecifyKind(g.Key.AddDays(6), DateTimeKind.Utc),
                Users = g.Sum(x => x.Users)
            })
            .ToList();

        var monthlyGroups = rows
            .GroupBy(r => new { r.Time.Year, r.Time.Month })
            .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
            .Select(g =>
            {
                var start = new DateTime(g.Key.Year, g.Key.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                var end = DateTime.SpecifyKind(start.AddMonths(1).AddDays(-1), DateTimeKind.Utc);
                return new BitcoinUserCountMonthlyDto
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    MonthStart = start,
                    MonthEnd = end,
                    Users = g.Sum(x => x.Users)
                };
            })
            .ToList();

        var response = new BitcoinUserCountsResponseDto
        {
            Daily = daily,
            Weekly = weeklyGroups,
            Monthly = monthlyGroups
        };

        return Result<BitcoinUserCountsResponseDto>.Success(response);
    }
}
