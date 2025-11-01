using ApplicationLayer.Dto.TechnicalSignals;
using ApplicationLayer.Features.TechnicalSignals.Query;
using ApplicationLayer.Interfaces;
using DomainLayer.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApplicationLayer.Features.TechnicalSignals.Handler;

public class GetSymbolsBySignalTypeHandler : IRequestHandler<GetSymbolsBySignalTypeQuery, IEnumerable<SignalSymbolDto>>
{
    private readonly IRepository<TechnicalSignal> _technicalSignalRepository;

    public GetSymbolsBySignalTypeHandler(IRepository<TechnicalSignal> technicalSignalRepository)
    {
        _technicalSignalRepository = technicalSignalRepository;
    }

    public async Task<IEnumerable<SignalSymbolDto>> Handle(GetSymbolsBySignalTypeQuery request, CancellationToken cancellationToken)
    {
        var query = _technicalSignalRepository
            .Query()
            .Where(s => s.DetailedSignalType == request.DetailedSignalType);

        // اعمال فیلترهای اختیاری
        if (!string.IsNullOrEmpty(request.TimeFrame))
        {
            query = query.Where(x => x.TimeFrame == request.TimeFrame);
        }

        if (request.SignalType.HasValue)
        {
            query = query.Where(x => x.SignalType == request.SignalType.Value);
        }

        if (request.FromDate.HasValue)
        {
            query = query.Where(x => x.CreatedAt >= request.FromDate.Value);
        }

        if (request.ToDate.HasValue)
        {
            query = query.Where(x => x.CreatedAt <= request.ToDate.Value);
        }

        // مرتب‌سازی بر اساس جدیدترین سیگنال‌ها
        query = query.OrderByDescending(x => x.CreatedAt);

        // صفحه‌بندی
        var signals = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var result = signals.Select(signal => new SignalSymbolDto
        {
            Symbol = signal.Symbol,
            DetailedSignalType = signal.DetailedSignalType,
            DetailedSignalTypeName = signal.DetailedSignalType.ToString(),
            DetailedSignalTypeNameLocalized = new DetailedSignalTypeNameDto
            {
                English = signal.DetailedSignalType.ToString(),
                Persian = SignalCategories.GetPersianName(signal.DetailedSignalType)
            },
            SignalType = signal.SignalType,
            SignalTypeText = GetSignalTypeText(signal.SignalType),
            TimeFrame = signal.TimeFrame,
            CreatedAt = signal.CreatedAt,
            Value = signal.Value,
            AdditionalData = signal.AdditionalData
        }).ToList();

        return result;
    }

    private static string GetSignalTypeText(SignalType signalType)
    {
        return signalType switch
        {
            SignalType.Buy => "خرید",
            SignalType.Sell => "فروش",
            SignalType.Neutral => "خنثی",
            _ => signalType.ToString()
        };
    }
}