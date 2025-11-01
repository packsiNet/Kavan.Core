# مستندات پیاده‌سازی سیگنال‌های تکنیکال - Hamyar Trade

## 📋 خلاصه تغییرات

این مستند شامل تمام تغییرات انجام شده برای پیاده‌سازی سیستم سیگنال‌های تکنیکال پیشرفته در پروژه Hamyar Trade می‌باشد.

## 🎯 هدف پروژه

پیاده‌سازی سیستم تحلیل تکنیکال جامع شامل:
- ارزیابی‌کننده‌های سیگنال Bollinger Bands
- ارزیابی‌کننده‌های سیگنال ADX (Average Directional Index)
- ارزیابی‌کننده‌های سیگنال CCI (Commodity Channel Index)
- API endpoints برای دسترسی به سیگنال‌ها

---

## 🔧 تغییرات ساختاری

### 1. بهینه‌سازی ApplicationDbContext

**فایل:** `InfrastructureLayer/Context/ApplicationDbContext.cs`

**تغییرات:**
- حذف DbSet‌های غیرضروری برای مدل‌های Candle
- حذف DbSet برای TechnicalSignal و Cryptocurrency
- نگهداری فقط DbSet برای MarketAnalysisResult

**قبل از تغییر:**
```csharp
public DbSet<MarketAnalysisResult> MarketAnalysisResults { get; set; }
public DbSet<TechnicalSignal> TechnicalSignals { get; set; }
public DbSet<Cryptocurrency> Cryptocurrencies { get; set; }
public DbSet<Candle_1m> Candles_1m { get; set; }
public DbSet<Candle_5m> Candles_5m { get; set; }
public DbSet<Candle_1h> Candles_1h { get; set; }
public DbSet<Candle_4h> Candles_4h { get; set; }
public DbSet<Candle_1d> Candles_1d { get; set; }
```

**بعد از تغییر:**
```csharp
public DbSet<MarketAnalysisResult> MarketAnalysisResults { get; set; }
```

**دلیل تغییر:** Configuration‌ها برای مدیریت Entity Framework کافی هستند و DbSet‌های اضافی غیرضروری بودند.

---

## 🚀 پیاده‌سازی‌های جدید

### 2. ارزیابی‌کننده‌های Bollinger Bands

**مسیر:** `InfrastructureLayer/BusinessLogic/Services/TechnicalSignals/BollingerBands/`

#### 2.1 BB Squeeze Evaluator
**فایل:** `BBSqueezeEvaluator.cs`

**عملکرد:**
- تشخیص زمان‌هایی که نوسانات بازار کم است (باندها فشرده شده‌اند)
- محاسبه نسبت عرض باند به قیمت میانگین
- تولید سیگنال Neutral زمانی که نسبت کمتر از 2% باشد

**الگوریتم:**
```csharp
var bandWidth = (upperBand - lowerBand) / middleBand;
if (bandWidth < 0.02m) // کمتر از 2%
    return SignalType.Neutral; // BB Squeeze
```

#### 2.2 BB Expansion Evaluator
**فایل:** `BBExpansionEvaluator.cs`

**عملکرد:**
- تشخیص افزایش نوسانات (گسترش باندها)
- مقایسه عرض باند فعلی با میانگین 5 کندل قبلی
- تولید سیگنال Buy/Sell بر اساس جهت شکست

#### 2.3 BB Band Touch Evaluator
**فایل:** `BBBandTouchEvaluator.cs`

**عملکرد:**
- تشخیص تماس قیمت با باندهای بالایی یا پایینی
- تولید سیگنال Sell برای تماس با باند بالایی
- تولید سیگنال Buy برای تماس با باند پایینی

#### 2.4 BB Middle Band Cross Evaluator
**فایل:** `BBMiddleBandCrossEvaluator.cs`

**عملکرد:**
- تشخیص عبور قیمت از خط میانی (SMA 20)
- تولید سیگنال Buy برای عبور از پایین به بالا
- تولید سیگنال Sell برای عبور از بالا به پایین

### 3. ارزیابی‌کننده‌های ADX

**مسیر:** `InfrastructureLayer/BusinessLogic/Services/TechnicalSignals/ADX/`

#### 3.1 ADX Trend Strength Evaluator
**فایل:** `ADXTrendStrengthEvaluator.cs`

**عملکرد:**
- ارزیابی قدرت ترند بر اساس مقدار ADX
- ADX > 25: ترند قوی
- ADX < 20: ترند ضعیف یا بازار خنثی

#### 3.2 ADX Rising/Falling Evaluator
**فایل:** `ADXRisingFallingEvaluator.cs`

**عملکرد:**
- تشخیص افزایش یا کاهش قدرت ترند
- مقایسه ADX فعلی با 3 کندل قبلی
- تولید سیگنال بر اساس جهت تغییر ADX

#### 3.3 ADX DI Crossover Evaluator
**فایل:** `ADXDICrossoverEvaluator.cs`

**عملکرد:**
- تشخیص تقاطع DI+ و DI-
- تولید سیگنال Buy زمانی که DI+ از DI- عبور کند
- تولید سیگنال Sell زمانی که DI- از DI+ عبور کند

### 4. ارزیابی‌کننده‌های CCI

**مسیر:** `InfrastructureLayer/BusinessLogic/Services/TechnicalSignals/CCI/`

#### 4.1 CCI Overbought/Oversold Evaluator
**فایل:** `CCIOverboughtOversoldEvaluator.cs`

**عملکرد:**
- تشخیص شرایط اشباع خرید/فروش
- CCI > +100: اشباع خرید (Sell Signal)
- CCI < -100: اشباع فروش (Buy Signal)

#### 4.2 CCI Zero Line Cross Evaluator
**فایل:** `CCIZeroLineCrossEvaluator.cs`

**عملکرد:**
- تشخیص عبور CCI از خط صفر
- تولید سیگنال Buy برای عبور از پایین به بالا
- تولید سیگنال Sell برای عبور از بالا به پایین

#### 4.3 CCI Divergence Evaluator
**فایل:** `CCIDivergenceEvaluator.cs`

**عملکرد:**
- تشخیص واگرایی بین قیمت و CCI
- شناسایی الگوهای برگشتی احتمالی
- تحلیل 10 کندل اخیر برای یافتن الگوهای واگرایی

---

## 🔄 سرویس‌های پردازش

### 5. Technical Signal Background Service

**فایل:** `InfrastructureLayer/BusinessLogic/Services/TechnicalSignalBackgroundService.cs`

**عملکرد:**
- پردازش خودکار سیگنال‌ها هر 30 ثانیه
- پردازش تمام ارزهای دیجیتال و تایم‌فریم‌ها
- ذخیره سیگنال‌های جدید در دیتابیس

**ویژگی‌ها:**
- پردازش ناهمزمان (Asynchronous)
- مدیریت خطا و لاگ‌گذاری
- قابلیت توقف و شروع مجدد

---

## 🌐 API Endpoints

### 6. Technical Signals Controller

**فایل:** `Kavan.Api/Controllers/TechnicalSignalsController.cs`

#### 6.1 لیست Endpoints

| Method | Endpoint | توضیحات |
|--------|----------|---------|
| GET | `/api/TechnicalSignals` | دریافت سیگنال‌ها با فیلترهای مختلف |
| GET | `/api/TechnicalSignals/categories` | دریافت دسته‌بندی‌های اندیکاتورها |
| GET | `/api/TechnicalSignals/indicators` | دریافت لیست اندیکاتورها |
| GET | `/api/TechnicalSignals/summary` | دریافت خلاصه آماری سیگنال‌ها |
| GET | `/api/TechnicalSignals/symbol/{symbol}` | دریافت سیگنال‌های یک ارز خاص |
| GET | `/api/TechnicalSignals/category/{category}` | دریافت سیگنال‌های یک دسته خاص |

#### 6.2 پارامترهای فیلتر

- `symbol`: نماد ارز (مثل BTCUSDT)
- `indicatorCategory`: دسته اندیکاتور
- `indicatorName`: نام اندیکاتور
- `signalType`: نوع سیگنال (0=Buy, 1=Sell, 2=Neutral)
- `timeFrame`: تایم‌فریم (1m, 5m, 1h, 4h, 1d)
- `fromDate` و `toDate`: بازه زمانی
- `page` و `pageSize`: صفحه‌بندی

---

## 📊 ساختار داده

### 7. مدل‌های داده

#### 7.1 TechnicalSignal Entity
```csharp
public class TechnicalSignal : BaseEntityModel
{
    public string Symbol { get; set; }
    public string IndicatorCategory { get; set; }
    public string IndicatorName { get; set; }
    public SignalType SignalType { get; set; }
    public string TimeFrame { get; set; }
    public decimal Price { get; set; }
    public decimal? IndicatorValue { get; set; }
    public string? AdditionalData { get; set; }
    public DateTime SignalTime { get; set; }
    public decimal Confidence { get; set; }
}
```

#### 7.2 SignalType Enum
```csharp
public enum SignalType
{
    Buy = 0,
    Sell = 1,
    Neutral = 2
}
```

---

## 🔧 Configuration‌ها

### 8. Entity Framework Configurations

تمام مدل‌ها دارای Configuration مجزا هستند:

- `TechnicalSignalConfiguration.cs`
- `CryptocurrencyConfiguration.cs`
- `CandleBaseConfiguration.cs`
- `Candle_1mConfiguration.cs`
- `Candle_5mConfiguration.cs`
- `Candle_1hConfiguration.cs`
- `Candle_4hConfiguration.cs`
- `Candle_1dConfiguration.cs`

---

## 🧪 تست‌ها

### 9. تست‌های انجام شده

#### 9.1 تست API Endpoints
- ✅ `/api/TechnicalSignals/categories` - بازگشت 9 دسته اندیکاتور
- ✅ `/api/TechnicalSignals/indicators` - بازگشت لیست اندیکاتورها
- ✅ `/api/TechnicalSignals/category/Bollinger%20Bands` - سیگنال‌های BB
- ✅ `/api/TechnicalSignals/category/ADX` - سیگنال‌های ADX
- ✅ `/api/TechnicalSignals/category/CCI` - سیگنال‌های CCI
- ✅ `/api/TechnicalSignals/summary` - آمار کلی سیگنال‌ها
- ✅ `/api/TechnicalSignals/symbol/BTCUSDT` - سیگنال‌های بیت‌کوین
- ✅ فیلتر کردن بر اساس نوع سیگنال

#### 9.2 نتایج تست
- همه endpoints با موفقیت پاسخ می‌دهند
- سیگنال‌ها برای ارزهای مختلف تولید می‌شوند
- فیلترها درست عمل می‌کنند

---

## 🚀 نحوه اجرا

### 10. راه‌اندازی سیستم

```bash
# اجرای API
dotnet run --project Kavan.Api

# دسترسی به Swagger
http://localhost:5005/swagger
```

### 11. نمونه درخواست‌ها

```bash
# دریافت دسته‌بندی‌ها
GET http://localhost:5005/api/TechnicalSignals/categories

# دریافت سیگنال‌های فروش Bollinger Bands
GET http://localhost:5005/api/TechnicalSignals?indicatorCategory=Bollinger%20Bands&signalType=1

# دریافت سیگنال‌های بیت‌کوین
GET http://localhost:5005/api/TechnicalSignals/symbol/BTCUSDT
```

---

## 📈 آمار عملکرد

### 12. نتایج حاصل

- **تعداد اندیکاتورهای پیاده‌سازی شده:** 9 دسته
- **تعداد ارزیابی‌کننده‌های جدید:** 10 کلاس
- **تعداد API endpoints:** 6 endpoint
- **پشتیبانی از تایم‌فریم‌ها:** 5 تایم‌فریم (1m, 5m, 1h, 4h, 1d)
- **تعداد ارزهای پشتیبانی شده:** تمام ارزهای موجود در دیتابیس

---

## 🔮 قابلیت‌های آینده

### 13. پیشنهادات توسعه

1. **اندیکاتورهای جدید:**
   - MACD
   - Stochastic
   - Williams %R
   - Fibonacci Retracements

2. **ویژگی‌های پیشرفته:**
   - تحلیل الگوهای کندل‌استیک
   - سیگنال‌های ترکیبی
   - هوش مصنوعی برای پیش‌بینی

3. **بهبودهای عملکرد:**
   - کش کردن سیگنال‌ها
   - پردازش موازی
   - بهینه‌سازی کوئری‌ها

---

## 📝 نتیجه‌گیری

سیستم سیگنال‌های تکنیکال Hamyar Trade با موفقیت پیاده‌سازی شد و شامل:

- ✅ 10 ارزیابی‌کننده سیگنال پیشرفته
- ✅ API کامل با 6 endpoint
- ✅ پشتیبانی از 5 تایم‌فریم
- ✅ سیستم پردازش خودکار
- ✅ ساختار قابل توسعه

سیستم آماده استفاده و قابل توسعه برای اندیکاتورهای جدید می‌باشد.

---

**تاریخ ایجاد:** نوامبر 2024  
**نسخه:** 1.0  
**وضعیت:** تکمیل شده و تست شده