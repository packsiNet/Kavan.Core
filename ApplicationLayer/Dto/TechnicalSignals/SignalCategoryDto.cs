using DomainLayer.Entities;

namespace ApplicationLayer.Dto.TechnicalSignals;

/// <summary>
/// DTO برای نمایش دسته‌بندی سیگنال‌ها
/// </summary>
public class SignalCategoryDto
{
    public string CategoryName { get; set; } = string.Empty;
    public string CategoryNamePersian { get; set; } = string.Empty;
    public List<SignalTypeDetailDto> SignalTypes { get; set; } = new();
}

/// <summary>
/// DTO برای جزئیات هر نوع سیگنال
/// </summary>
public class SignalTypeDetailDto
{
    public DetailedSignalType SignalType { get; set; }
    public int SignalTypeId { get; set; }
    public string SignalTypeName { get; set; } = string.Empty;
    public string SignalTypeNamePersian { get; set; } = string.Empty;
    public int CurrentCount { get; set; } // تعداد فعلی این سیگنال
}

/// <summary>
/// DTO برای نمایش ارزهایی که سیگنال خاصی دارند
/// </summary>
public class SignalSymbolDto
{
    public string Symbol { get; set; } = string.Empty;
    public DetailedSignalType DetailedSignalType { get; set; }
    public string DetailedSignalTypeName { get; set; } = string.Empty;
    public string DetailedSignalTypeNamePersian { get; set; } = string.Empty;
    public SignalType SignalType { get; set; }
    public string SignalTypeText { get; set; } = string.Empty;
    public string TimeFrame { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public decimal? Value { get; set; }
    public string? AdditionalData { get; set; }
}

/// <summary>
/// DTO برای درخواست ارزها بر اساس نوع سیگنال
/// </summary>
public class GetSymbolsBySignalTypeDto
{
    public DetailedSignalType DetailedSignalType { get; set; }
    public string? TimeFrame { get; set; }
    public SignalType? SignalType { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}

/// <summary>
/// DTO برای خلاصه آماری سیگنال‌ها بر اساس دسته‌بندی جدید
/// </summary>
public class DetailedSignalSummaryDto
{
    public string CategoryName { get; set; } = string.Empty;
    public string CategoryNamePersian { get; set; } = string.Empty;
    public DetailedSignalType DetailedSignalType { get; set; }
    public string DetailedSignalTypeName { get; set; } = string.Empty;
    public string DetailedSignalTypeNamePersian { get; set; } = string.Empty;
    public int BuySignals { get; set; }
    public int SellSignals { get; set; }
    public int NeutralSignals { get; set; }
    public int TotalSignals { get; set; }
    public List<string> ActiveSymbols { get; set; } = new(); // لیست ارزهایی که این سیگنال را دارند
}