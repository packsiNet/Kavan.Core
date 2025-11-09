namespace ApplicationLayer.Interfaces.Services;

public interface IBinanceKlineService
{
    /// <summary>
    /// دریافت کندل‌های 1 دقیقه‌ای از Binance API
    /// </summary>
    /// <param name="symbol">نماد رمزارز (مثال: BTCUSDT)</param>
    /// <param name="limit">تعداد کندل‌ها (پیش‌فرض: 5)</param>
    /// <param name="startTime">زمان شروع (اختیاری)</param>
    /// <param name="endTime">زمان پایان (اختیاری)</param>
    /// <returns>لیست کندل‌های دریافتی</returns>
    Task<List<BinanceKlineDto>> GetKlinesAsync(string symbol, int limit = 5, long? startTime = null, long? endTime = null);
}

public class BinanceKlineDto
{
    public long OpenTime { get; set; }
    public decimal OpenPrice { get; set; }
    public decimal HighPrice { get; set; }
    public decimal LowPrice { get; set; }
    public decimal ClosePrice { get; set; }
    public decimal Volume { get; set; }
    public long CloseTime { get; set; }
    public decimal QuoteAssetVolume { get; set; }
    public int NumberOfTrades { get; set; }
    public decimal TakerBuyBaseAssetVolume { get; set; }
    public decimal TakerBuyQuoteAssetVolume { get; set; }
}