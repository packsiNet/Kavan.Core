using DomainLayer.Entities;

namespace ApplicationLayer.Interfaces.Services;

public interface ICandleManagementService
{
    /// <summary>
    /// دریافت و ذخیره کندل‌های 1 دقیقه‌ای برای یک رمزارز با بررسی پیوستگی زمانی
    /// </summary>
    /// <param name="cryptocurrency">موجودیت رمزارز</param>
    /// <returns></returns>
    Task ProcessCandlesForSymbolAsync(Cryptocurrency cryptocurrency);

    /// <summary>
    /// دریافت و ذخیره کندل‌های از دست رفته بین دو بازه زمانی
    /// </summary>
    /// <param name="cryptocurrencyId">شناسه رمزارز</param>
    /// <param name="symbol">نماد رمزارز</param>
    /// <param name="fromCloseTime">زمان بسته شدن آخرین کندل موجود</param>
    /// <param name="toOpenTime">زمان باز شدن کندل جدید</param>
    /// <returns></returns>
    Task FetchMissingCandlesAsync(int cryptocurrencyId, string symbol, long fromCloseTime, long toOpenTime);
}