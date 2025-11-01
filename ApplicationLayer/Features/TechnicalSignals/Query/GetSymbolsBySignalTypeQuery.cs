using ApplicationLayer.Dto.TechnicalSignals;
using DomainLayer.Entities;
using MediatR;

namespace ApplicationLayer.Features.TechnicalSignals.Query;

/// <summary>
/// Query برای دریافت ارزهایی که سیگنال خاصی دارند
/// </summary>
public record GetSymbolsBySignalTypeQuery(
    DetailedSignalType DetailedSignalType,
    string? TimeFrame = null,
    SignalType? SignalType = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null,
    int Page = 1,
    int PageSize = 50
) : IRequest<List<SignalSymbolDto>>;