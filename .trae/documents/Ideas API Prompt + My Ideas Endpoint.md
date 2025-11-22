## خلاصه پرامپت برای UI بخش Ideas

### لیست عمومی ایده‌ها
- روش: GET
- آدرس: `/api/ideas`
- احراز هویت: ندارد (عمومی)
- پارامترها:
  - `symbol` اختیاری (مانند `BTCUSDT`)
  - `timeframe` اختیاری (`1m|5m|1h|4h|1d`)
  - `trend` اختیاری (`Bullish|Bearish|Range`)
  - `tags` اختیاری (رشته کامایی: `breakout,btc`)
  - `page` پیش‌فرض 1
  - `pageSize` پیش‌فرض 10
- خروجی: `HandlerResult` با `ObjectResult = IdeasPageDto`
  - `Items[]` از نوع `IdeaDto` (فیلدها: `id,symbol,timeframe,trend,title,description,image,status,tags[],createdAt`)
  - `Total`, `Page`, `PageSize`
- نمونه درخواست: `GET /api/ideas?symbol=BTCUSDT&timeframe=1h&trend=Bullish&page=1&pageSize=20`

### جزئیات یک ایده عمومی
- روش: GET
- آدرس: `/api/ideas/{id}`
- احراز هویت: ندارد (عمومی)
- خروجی: `HandlerResult` با `ObjectResult = IdeaDto`

### ایجاد ایده
- روش: POST
- آدرس: `/api/ideas`
- احراز هویت: لازم (Policy=`Trader`, Role=`Users`)
- Content-Type: `multipart/form-data`
- فیلدهای فرم:
  - `symbol` الزامی
  - `timeframe` الزامی (`1m|5m|1h|4h|1d`)
  - `trend` الزامی (`Bullish|Bearish|Range`)
  - `title` الزامی
  - `description` اختیاری
  - `status` الزامی (`Public|Private`)
  - `tags` اختیاری (تکرار شونده؛ چند مقدار)
  - `image` اختیاری (`IFormFile`، مجاز: `jpeg|png|webp`، حجم ≤ 5MB)
- خروجی: `HandlerResult` با `ObjectResult = IdeaDto` (شامل `image` به‌صورت URL نسبی `/uploads/ideas/{file}`)

### بروزرسانی ایده
- روش: PUT
- آدرس: `/api/ideas/{id}`
- احراز هویت: لازم (Policy=`Trader`, Role=`Users`)
- Content-Type: `multipart/form-data`
- فیلدها: مشابه ایجاد؛ `image` در صورت ارسال، تصویر جدید جایگزین می‌شود
- خروجی: `HandlerResult` با `ObjectResult = IdeaDto`

### حذف ایده
- روش: DELETE
- آدرس: `/api/ideas/{id}`
- احراز هویت: لازم (Policy=`Trader`, Role=`Users`)
- خروجی: `HandlerResult` بدون `ObjectResult`

### رفتار ولیدیشن و خطا
- ولیدیشن سمت سرور:
  - `timeframe/trend/status` طبق دامنه‌ی مجاز
  - `image` در صورت ارسال: نوع مجاز و حجم ≤ 5MB
- پاسخ‌ها بر اساس `RequestStatus` (موفق/خطا/یافت‌نشد/تداخل/اعتبارسنجی): برای UI تبدیل به اعلان‌ها/Toast

## نیاز جدید: لیست ایده‌های کاربر (برای مدیریت/ویرایش)

### هدف
- افزودن اندپوینت «لیست ایده‌های من» برای کاربر لاگین‌شده با صفحه‌بندی و فیلترهای پایه

### طراحی API (پیشنهاد)
- روش: GET
- آدرس: `/api/ideas/my`
- احراز هویت: لازم (Policy=`Trader`, Role=`Users`)
- پارامترها:
  - `page` پیش‌فرض 1
  - `pageSize` پیش‌فرض 10
  - فیلترهای اختیاری مشابه لیست عمومی: `symbol`, `timeframe`, `trend`, `tags`
- خروجی: `HandlerResult` با `ObjectResult = IdeasPageDto` (فقط متعلق به کاربر)

### تغییرات لازم در بک‌اند
1. افزودن مالکیت به موجودیت Idea:
   - فیلد جدید: `OwnerUserId` (int) برای نگه‌داری شناسه‌ی کاربر
   - مقداردهی در ایجاد (با `IUserContextService` و Claims)
2. سرویس‌ها:
   - افزودن متد `GetMineAsync(GetIdeasRequestDto)` در `IIdeaService` و پیاده‌سازی آن
   - اعمال محدودیت مالکیت در `UpdateAsync/DeleteAsync`: فقط مالک مجاز باشد
3. CQRS:
   - `GetMyIdeasQuery` + `GetMyIdeasHandler`
4. کنترلر:
   - افزودن اکشن جدید `[HttpGet("my")]` با خواندن صفحه‌بندی و فیلترها
5. مهاجرت دیتابیس:
   - افزودن ستون `OwnerUserId` و ایندکس مرکب (`OwnerUserId, CreatedAt`) برای مرتب‌سازی سریع

### رفتار UI بر اساس اندپوینت جدید
- صفحه «ایده‌های من»: جدول/کارت‌ها با صفحه‌بندی
- اکشن‌ها:
  - ویرایش: لینک به فرم PUT با prefill از `IdeaDto`
  - حذف: فراخوانی DELETE و رفرش لیست
  - فیلتر: همان فیلترهای عمومی؛ روی داده‌های مالک اعمال شود

## نکات کلیدی برای UI
- آپلود تصویر: `multipart/form-data`، نمایش Progress و محدودیت‌ها
- نمایش وضعیت: `status` به دو حالت عمومی/خصوصی با سوییچ
- تگ‌ها: ورودی چندگانه؛ ارسال به‌صورت مقادیر تکراری یا رشته کامایی
- صفحه‌بندی: استفاده از `Total` برای محاسبه صفحات؛ نگهداری `page/pageSize` در QueryString

## ارجاع فایل‌ها (برای توسعه)
- کنترلر ایده‌ها: `Kavan.Api/Controllers/IdeasController.cs`
- ولیدیشن ایده‌ها: `ApplicationLayer/Features/Ideas/Validations/IdeaValidators.cs`
- سرویس ذخیره‌سازی فایل: `InfrastructureLayer/BusinessLogic/Services/Uploads/FileStorageService.cs`
- سرویس ایده: `InfrastructureLayer/BusinessLogic/Services/Ideas/IdeaService.cs`

لطفاً تأیید کنید تا «لیست ایده‌های من» و محدودیت مالکیت در بروزرسانی/حذف پیاده‌سازی شوند.