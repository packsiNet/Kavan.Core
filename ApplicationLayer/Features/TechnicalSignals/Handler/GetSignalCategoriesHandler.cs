using ApplicationLayer.Dto.TechnicalSignals;
using ApplicationLayer.Features.TechnicalSignals.Query;
using ApplicationLayer.Interfaces;
using DomainLayer.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApplicationLayer.Features.TechnicalSignals.Handler;

public class GetSignalCategoriesHandler : IRequestHandler<GetSignalCategoriesQuery, List<SignalCategoryDto>>
{
    private readonly IRepository<TechnicalSignal> _technicalSignalRepository;

    public GetSignalCategoriesHandler(IRepository<TechnicalSignal> technicalSignalRepository)
    {
        _technicalSignalRepository = technicalSignalRepository;
    }

    public async Task<List<SignalCategoryDto>> Handle(GetSignalCategoriesQuery request, CancellationToken cancellationToken)
    {
        // Get all signal types from the last 24 hours to calculate counts
        var signalCounts = await _technicalSignalRepository
            .Query()
            .Where(s => s.CreatedAt >= DateTime.UtcNow.AddHours(-24))
            .GroupBy(s => s.DetailedSignalType)
            .Select(g => new { SignalType = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var signalCountDict = signalCounts.ToDictionary(x => x.SignalType, x => x.Count);

        var result = new List<SignalCategoryDto>();

        foreach (var category in DomainLayer.Entities.SignalCategories.Categories)
        {
            var categoryDto = new SignalCategoryDto
            {
                CategoryName = category.Key,
                CategoryNamePersian = GetCategoryPersianName(category.Key),
                SignalTypes = new List<SignalTypeDetailDto>()
            };

            foreach (var signalType in category.Value)
            {
                var count = signalCountDict.GetValueOrDefault(signalType, 0);
                
                categoryDto.SignalTypes.Add(new SignalTypeDetailDto
                {
                    SignalType = signalType,
                    SignalTypeId = (int)signalType,
                    SignalTypeName = signalType.ToString(),
                    SignalTypeNamePersian = DomainLayer.Entities.SignalCategories.GetPersianName(signalType),
                    CurrentCount = count
                });
            }

            result.Add(categoryDto);
        }

        return result;
    }

    private static string GetCategoryPersianName(string categoryName)
    {
        return categoryName switch
        {
            "Ichimoku" => "ایچیموکو",
            "Bollinger Bands" => "باند بولینگر",
            "RSI" => "RSI",
            "MACD" => "MACD",
            "Moving Averages" => "میانگین متحرک",
            "Stochastic" => "استوکاستیک",
            "ADX" => "ADX",
            "CCI" => "CCI",
            "Williams %R" => "ویلیامز %R",
            "Volume" => "حجم",
            "Support/Resistance" => "حمایت/مقاومت",
            "Candlestick Patterns" => "الگوهای کندل استیک",
            "Fibonacci" => "فیبوناچی",
            "Pivot Points" => "نقاط محوری",
            _ => categoryName
        };
    }
}