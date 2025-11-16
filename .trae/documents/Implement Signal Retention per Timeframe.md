**Findings**

* هیچ منطق پاکسازی/حذف سیگنال‌های قدیمی در کد وجود ندارد. هیچ سرویس، Handler یا Job برای حذف رکوردهای `Signal`/`SignalCandle` پیاده‌سازی نشده است. فقط حذف آبشاری `SignalCandles` در صورت حذف یک `Signal` پیکربندی شده است.

**Goal**

* افزودن «سیاست نگه‌داری سیگنال‌ها» متفاوت برای هر تایم‌فریم و اجرای پاکسازی خودکار و ایمن.

**Retention Policy (Configurable)**

* تعریف در `appsettings.json` زیر کلید `SignalRetention`: مقادیر روز/ساعت به‌ازای تایم‌فریم‌ها. مثال اولیه (قابل تغییر):

  * `1m`: 7 روز

  * `5m`: 14 روز

  * `1h`: 90 روز

  * `4h`: 180 روز

  * `1d`: 365 روز

* امکان تعریف استثنا (مثلاً عدم حذف برخی `Category`/`SignalName`) در همان بخش کانفیگ.

**Design**

* InfrastructureLayer:

  * افزودن Hosted Service جدید: `SignalsRetentionBackgroundService` با `PeriodicTimer` (مثلاً هر 6 ساعت)، همراه با `SemaphoreSlim` برای جلوگیری از هم‌پوشانی اجرا.

  * سرویس، کانفیگ را از `IOptions<SignalRetentionOptions>` می‌خواند و برای هر تایم‌فریم `cutoff` را محاسبه می‌کند؛ سپس رکوردهای `Signal` با شرط `Timeframe == tf && SignalTime < cutoff` را حذف می‌کند. حذف آبشاری، شمع‌های مرتبط را پاک می‌کند.

  * لاگ‌گیری خلاصه اجرا: تعداد حذف‌شده برای هر تایم‌فریم، مدت اجرا، دسته‌ها/سیگنال‌های مستثنی.

* ApplicationLayer:

  * Feature Folder: `SignalsRetention` با Command/Handler:

    * `PurgeOldSignalsCommand` (ورودی اختیاری: `timeframes[]`, `until`, `dryRun`, `excludeCategories[]`, `excludeSignalNames[]`)

    * Handler با `IRepository<Signal>` و `IUnitOfWork` حذف On-demand را اجرا و خروجی خلاصه می‌دهد.

  * DTO نتیجه: شامل شمارش حذف‌ها به‌ازای هر تایم‌فریم.

* API Layer:

  * Controller `SignalsRetentionController` با دو Endpoint:

    * `POST /api/SignalsRetention/purge` برای اجرای دستی پاکسازی با پارامترهای اختیاری.

    * `GET /api/SignalsRetention/status` برای مشاهده آخرین اجرای سرویس (زمان، تعداد حذف‌ها).

* امنیت و ایمنی:

  * اجرای در تراکنش EF در Batchهای محدود (مثلاً 5–10k رکورد در هر Batch) برای جلوگیری از Lock طولانی.

  * پشتیبانی `dry-run` (فقط شمارش، بدون حذف).

  * لاگ سطح Warning اگر تعداد حذف غیرعادی باشد.

**Implementation Steps**

1. تعریف `SignalRetentionOptions` در InfrastructureLayer/Configuration و خواندن از `appsettings.json`.
2. ایجاد `SignalsRetentionBackgroundService` (با `PeriodicTimer`, `SemaphoreSlim`) و ثبت در DI (`AddHostedService`).
3. پیاده‌سازی حذف Batchی بر اساس `Timeframe` و `SignalTime`؛ احترام به استثناهای کانفیگ.
4. ساخت Feature `SignalsRetention` در ApplicationLayer (Command/Handler/DTO) مطابق الگوی CQRS پروژه.
5. افزودن Controller در Kavan.Api با Endpointهای `purge` و `status` که فقط از Mediator استفاده کنند.
6. افزودن لاگ‌های عملیاتی (تعداد حذف، مدت اجرا، خطاها) و تنظیم health check ساده (مثلاً زمان آخرین اجرا در حافظه).
7. تست:

   * داده نمونه برای هر تایم‌فریم و سنین مختلف.

   * اجرای `dry-run` و مقایسه شمارش با انتظار.

   * اجرای واقعی و بررسی حذف آبشاری `SignalCandles`.

**Notes**

* حذف بر مبنای تایم‌فریم و زمان سیگنال انجام می‌شود؛ می‌توان بعداً فیلترهای سطح دسته/نام سیگنال را اضافه/تغییر داد.

* هیچ تغییری در ساختار جدول سیگنال نیاز نیست؛ رفتار Cascade کافی است.

* اجرای دوره‌ای قابل تنظیم (مثلاً هر 6 یا 12 ساعت) و قابل خاموش‌کردن از کانفیگ است.

