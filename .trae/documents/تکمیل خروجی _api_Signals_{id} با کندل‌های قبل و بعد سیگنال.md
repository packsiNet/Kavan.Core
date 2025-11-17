## هدف
- بازگرداندن یک چارت کامل در خروجی `GET /api/Signals/{id}` شامل:
  - 100 کندل قبل از کندل محرک سیگنال
  - اسنپ‌شات کندل‌های ثبت‌شده‌ی سیگنال (میانه)
  - حداکثر 10 کندل بعد از کندل محرک
- تشخیص دقیق کندل محرک با استفاده از فیلد `IsTrigger` و حفظ ترتیب زمانی.

## تغییرات لایه‌ها
### ApplicationLayer
- افزودن اینترفیس `ICandlesQueryService` در `ApplicationLayer/Interfaces/Services/Candles/ICandlesQueryService.cs` با امضای:
  - ```csharp
    Task<List<CandleBase>> GetAroundAsync(
      int cryptocurrencyId,
      string timeframe,
      DateTime pivotCloseTime,
      int beforeCount = 100,
      int afterCount = 10,
      CancellationToken ct = default)
    ```
- بروزرسانی هندلر `GetSignalByIdHandler` در `ApplicationLayer/Features/Signals/Handler/GetSignalByIdHandler.cs`:
  - تزریق `ICandlesQueryService` در سازنده.
  - پس از واکشی `Signal` و `SignalCandle`ها، دریافت کندل‌های قبل/بعد با `GetAroundAsync`.
  - ادغام سه بخش: قبل + اسنپ‌شات + بعد، با حذف موارد تکراری بر اساس `CloseTime`.
  - نگاشت به `SignalDto.Candles` و تعیین `IsTrigger` برای کندل محرک (برای اسنپ‌شات از `c.IsTrigger`، برای کندل‌های قبل/بعد مقدار `false`).
  - حفظ `LastCandle` با `IsTrigger = true`.

### InfrastructureLayer
- پیاده‌سازی `CandlesQueryService` با `[InjectAsScoped]` در `InfrastructureLayer/BusinessLogic/Services/Candles/CandlesQueryService.cs` که بسته به `timeframe` از جداول:
  - `Candle_1m`, `Candle_5m`, `Candle_1h`, `Candle_4h`, `Candle_1d`
  واکشی را انجام دهد:
  - قبل: `CloseTime <= pivotCloseTime` مرتب‌سازی نزولی، `Take(beforeCount)`, سپس معکوس به ترتیب صعودی.
  - بعد: `CloseTime > pivotCloseTime` مرتب‌سازی صعودی، `Take(afterCount)`.
  - خروجی: ادغام قبل و بعد به صورت لیست زمان‌مند.

## جزئیات ادغام در هندلر
- مکان فعلی هندلر: `ApplicationLayer/Features/Signals/Handler/GetSignalByIdHandler.cs:17-85`.
- گام‌ها:
  - واکشی سیگنال: `Repository<Signal>` و اسنپ‌شات: `Repository<SignalCandle>`.
  - فراخوانی سرویس کندل: `GetAroundAsync(log.CryptocurrencyId, log.Timeframe, log.CandleCloseTime, 100, 10)`.
  - ادغام:
    - ساخت `HashSet<long>` از `CloseTime.Ticks` برای جلوگیری از تکرار.
    - افزودن کندل‌های «قبل» به لیست خروجی (بدون `IsTrigger`).
    - افزودن اسنپ‌شات سیگنال‌ها به لیست خروجی و حفظ `IsTrigger`.
    - افزودن کندل‌های «بعد» به لیست خروجی (بدون `IsTrigger`).
  - نگاشت به `SignalDto.Candles` و `SignalDto.LastCandle` با `IsTrigger = true`.

## بهینه‌سازی‌ها (اختیاری)
- ایندکس‌های کارایی برای جداول کندل:
  - `(CryptocurrencyId, CloseTime)` اگر وجود ندارد، اضافه شود.
- پارامترپذیری:
  - در آینده می‌توان شمارنده‌های `beforeCount` و `afterCount` را از کوئری‌استرینگ `GET /api/Signals/{id}?before=100&after=10` دریافت کرد؛ فعلاً مقدار پیش‌فرض 100/10 اعمال می‌شود.

## لاگ‌گیری و خطا
- در `GetSignalByIdHandler` در صورت نبود کندل‌های بعد از سیگنال، خروجی فقط شامل قبل + اسنپ‌شات خواهد بود؛ پیام مناسب در لاگ سطح Debug.
- مدیریت خطا مشابه الگوی فعلی هندلرها با `Result.GeneralFailure(...)`.

## آزمون‌ها
- سناریوها:
  - سیگنال با اسنپ‌شات 3 کندلی؛ بررسی ادغام 100 قبل + 3 اسنپ‌شات + ≤10 بعد.
  - عدم وجود کندل‌های بعد؛ خروجی باید بدون بخش «بعد» باشد.
  - جلوگیری از تکرار در صورت همپوشانی اسنپ‌شات با قبل/بعد.
- ابزار: تست‌های یکپارچه با پایگاه‌داده درون‌حافظه/SQLite و داده‌های نمونه.

## نکات فرانت‌اند
- از `GET /api/Signals/{id}`، آرایه‌ی `SignalDto.Candles` اکنون شامل چارت کامل است.
- کندل محرک را با `IsTrigger = true` پیدا و لیبل‌گذاری کنید؛ موقعیت زمانی برای هم‌ترازسازی روی چارت از `CloseTime` استفاده شود.
- برای نمایش الگوهای چندکندلی، بخش میانی (اسنپ‌شات) را هایلایت کنید و کندل محرک را برجسته نمایید.

آیا با این طرح موافقید تا پیاده‌سازی انجام شود؟