using ApplicationLayer.Dto.TechnicalSignals;
using ApplicationLayer.Features.TechnicalSignals.Query;
using ApplicationLayer.Interfaces;
using AutoMapper;
using DomainLayer.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using static DomainLayer.Entities.TechnicalSignal;

namespace ApplicationLayer.Features.TechnicalSignals.Handler;

public class GetTechnicalSignalsHandler : 
    IRequestHandler<GetTechnicalSignalsQuery, IEnumerable<TechnicalSignalDto>>,
    IRequestHandler<GetTechnicalSignalsSummaryQuery, IEnumerable<TechnicalSignalSummaryDto>>,
    IRequestHandler<GetTechnicalSignalCategoriesQuery, IEnumerable<string>>,
    IRequestHandler<GetTechnicalSignalIndicatorsQuery, IEnumerable<string>>
{
    private readonly IRepository<TechnicalSignal> _repository;
    private readonly IMapper _mapper;

    public GetTechnicalSignalsHandler(IRepository<TechnicalSignal> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TechnicalSignalDto>> Handle(
        GetTechnicalSignalsQuery request, 
        CancellationToken cancellationToken)
    {
        var filter = request.Filter;
        Expression<Func<TechnicalSignal, bool>> predicate = s => true;

        // Build dynamic filter
        if (!string.IsNullOrEmpty(filter.Symbol))
        {
            predicate = CombinePredicates(predicate, s => s.Symbol == filter.Symbol);
        }

        if (!string.IsNullOrEmpty(filter.IndicatorCategory))
        {
            predicate = CombinePredicates(predicate, s => s.IndicatorCategory == filter.IndicatorCategory);
        }

        if (!string.IsNullOrEmpty(filter.IndicatorName))
        {
            predicate = CombinePredicates(predicate, s => s.IndicatorName == filter.IndicatorName);
        }

        if (filter.SignalType.HasValue)
        {
            predicate = CombinePredicates(predicate, s => s.SignalType == filter.SignalType.Value);
        }

        if (!string.IsNullOrEmpty(filter.TimeFrame))
        {
            predicate = CombinePredicates(predicate, s => s.TimeFrame == filter.TimeFrame);
        }

        if (filter.FromDate.HasValue)
        {
            predicate = CombinePredicates(predicate, s => s.CreatedAt >= filter.FromDate.Value);
        }

        if (filter.ToDate.HasValue)
        {
            predicate = CombinePredicates(predicate, s => s.CreatedAt <= filter.ToDate.Value);
        }

        var query = _repository.Query();
        
        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        var signals = await query
            .OrderByDescending(s => s.CreatedAt)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        var result = signals.Select(signal =>
        {
            var dto = _mapper.Map<TechnicalSignalDto>(signal);
            dto.SignalTypeText = signal.SignalType.ToString();
            return dto;
        });

        return result;
    }

    public async Task<IEnumerable<TechnicalSignalSummaryDto>> Handle(
        GetTechnicalSignalsSummaryQuery request, 
        CancellationToken cancellationToken)
    {
        Expression<Func<TechnicalSignal, bool>> predicate = s => true;

        if (!string.IsNullOrEmpty(request.Symbol))
        {
            predicate = CombinePredicates(predicate, s => s.Symbol == request.Symbol);
        }

        if (!string.IsNullOrEmpty(request.TimeFrame))
        {
            predicate = CombinePredicates(predicate, s => s.TimeFrame == request.TimeFrame);
        }

        // Get signals from last 24 hours
        var cutoffTime = DateTime.UtcNow.AddHours(-24);
        predicate = CombinePredicates(predicate, s => s.CreatedAt >= cutoffTime);

        var query = _repository.Query();
        
        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        var signals = await query
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();

        var summary = signals
            .GroupBy(s => new { s.IndicatorCategory, s.IndicatorName })
            .Select(g => new TechnicalSignalSummaryDto
            {
                IndicatorCategory = g.Key.IndicatorCategory,
                IndicatorName = g.Key.IndicatorName,
                BuySignals = g.Count(s => s.SignalType == SignalType.Buy),
                SellSignals = g.Count(s => s.SignalType == SignalType.Sell),
                NeutralSignals = g.Count(s => s.SignalType == SignalType.Neutral),
                TotalSignals = g.Count()
            })
            .OrderBy(s => s.IndicatorCategory)
            .ThenBy(s => s.IndicatorName);

        return summary;
    }

    public async Task<IEnumerable<string>> Handle(
        GetTechnicalSignalCategoriesQuery request, 
        CancellationToken cancellationToken)
    {
        var categories = await _repository.Query()
            .Select(s => s.IndicatorCategory)
            .Distinct()
            .ToListAsync();

        return categories.OrderBy(c => c);
    }

    public async Task<IEnumerable<string>> Handle(
        GetTechnicalSignalIndicatorsQuery request, 
        CancellationToken cancellationToken)
    {
        Expression<Func<TechnicalSignal, bool>> predicate = s => true;

        if (!string.IsNullOrEmpty(request.Category))
        {
            predicate = s => s.IndicatorCategory == request.Category;
        }

        var indicators = await _repository.Query()
            .Where(predicate)
            .Select(s => s.IndicatorName)
            .Distinct()
            .ToListAsync();

        return indicators.OrderBy(i => i);
    }

    private static Expression<Func<TechnicalSignal, bool>> CombinePredicates(
        Expression<Func<TechnicalSignal, bool>> first,
        Expression<Func<TechnicalSignal, bool>> second)
    {
        var parameter = Expression.Parameter(typeof(TechnicalSignal));
        var body = Expression.AndAlso(
            Expression.Invoke(first, parameter),
            Expression.Invoke(second, parameter));
        return Expression.Lambda<Func<TechnicalSignal, bool>>(body, parameter);
    }
}