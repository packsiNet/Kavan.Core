using ApplicationLayer.Dto.TechnicalSignals;
using MediatR;

namespace ApplicationLayer.Features.TechnicalSignals.Query;

/// <summary>
/// Query برای دریافت تمام دسته‌بندی‌های سیگنال با جزئیات
/// </summary>
public record GetSignalCategoriesQuery : IRequest<List<SignalCategoryDto>>;