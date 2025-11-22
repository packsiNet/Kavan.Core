# مستند پرامپت برای پیاده‌سازی UI بخش Ideas

## مرور کلی
- حوزه: مدیریت و نمایش «ایده‌های تریدرها» با وضعیت عمومی/خصوصی و فیلترها
- Base URL: `/api/ideas`
- گروه Swagger:
  - عمومی (خواندن): Public
  - عملیات نویسه‌ای (ایجاد/ویرایش/حذف): Trader با احراز هویت و نقش `Users`

## مدل‌های داده مورد استفاده در UI
- IdeaDto (آیتم لیست/جزئیات):
  - `id`, `symbol`, `timeframe` (`1m|5m|1h|4h|1d`), `trend` (`Bullish|Bearish|Range`), `title`, `description`, `image` (URL)، `status` (`public|private`), `tags: string[]`, `createdAt`
- IdeasPageDto (صفحه‌بندی):
  - `items: IdeaDto[]`, `total`, `page`, `pageSize`
- قوانین اعتبارسنجی کلیدی:
  - `symbol, timeframe, trend, title, status` الزامی
  - تصویر اختیاری، ولی اگر ارسال شود: `image/jpeg|image/png|image/webp` و حداکثر `5MB`

## Endpointها و نحوه مصرف در UI
1) دریافت لیست عمومی با فیلتر و صفحه‌بندی
- Method/Path: `GET /api/ideas`
- Query Params:
  - `symbol` (اختیاری)
  - `timeframe` یکی از `1m,5m,1h,4h,1d` (اختیاری)
  - `trend` یکی از `Bullish,Bearish,Range` (اختیاری)
  - `tags` رشته کامایی مانند `breakout,btc,long` (اختیاری)
  - `page` پیش‌فرض `1`, `pageSize` پیش‌فرض `10`
- Response:
  - `HandlerResult(ObjectResult=IdeasPageDto, RequestStatus, Message)`
- UI رفتار:
  - فرم فیلتر بالا: `symbol`, `timeframe`, `trend`, `tags` (چندگانه)
  - لیست کارت‌ها با تصویر (در صورت وجود)، عنوان، نماد، تایم‌فریم، روند و تگ‌ها
  - صفحه‌بندی یا infinite scroll با `page/pageSize`

2) دریافت جزئیات ایده عمومی
- Method/Path: `GET /api/ideas/{id}`
- Response:
  - `HandlerResult(ObjectResult=IdeaDto)`
- UI رفتار:
  - نمایش صفحه جزئیات با تصویر، عنوان، توضیحات کامل، نماد، تایم‌فریم، روند و تگ‌ها

3) ایجاد ایده (نیازمند احراز هویت نقش Trader/Users)
- Method/Path: `POST /api/ideas`
- Content-Type: `multipart/form-data`
- Form Fields:
  - `symbol: string`
  - `timeframe: string` از مجموعه مجاز
  - `trend: string` از مجموعه مجاز
  - `title: string`
  - `description: string` (اختیاری)
  - `status: string` (`Public` یا `Private`)
  - `tags[]: string[]` (ارسال به‌صورت فیلدهای تکراری `tags` یا اندیس‌دار `tags[0]`)
  - `image: file` (اختیاری؛ انواع مجاز و ≤ 5MB)
- Response:
  - `HandlerResult(ObjectResult=IdeaDto)` با `image` به‌صورت URL (مثلاً `/uploads/ideas/{guid}.png`)
- UI رفتار:
  - فرم ایجاد با پیش‌نمایش تصویر انتخاب‌شده، ورودی‌های انتخابی برای `timeframe/trend/status`
  - تگ‌ها با ورودی چندگانه و نمایش chip
  - پس از موفقیت: هدایت به صفحه جزئیات یا نمایش toast موفقیت

4) بروزرسانی ایده (نیازمند احراز هویت)
- Method/Path: `PUT /api/ideas/{id}`
- Content-Type: `multipart/form-data`
- Form Fields: مشابه ایجاد؛ `image` اختیاری و در صورت ارسال، تصویر جدید جایگزین می‌شود
- Response: `HandlerResult(ObjectResult=IdeaDto)`
- UI رفتار:
  - فرم/مودال ویرایش با داده‌های قبلی، امکان تغییر فیلدها و تصویر

5) حذف ایده (نیازمند احراز هویت)
- Method/Path: `DELETE /api/ideas/{id}`
- Response: `HandlerResult(RequestStatus=Successful)`
- UI رفتار:
  - دکمه حذف با تایید، به‌روزرسانی لیست پس از موفقیت

## قواعد اعتبارسنجی در UI
- `timeframe` فقط: `1m,5m,1h,4h,1d`
- `trend` فقط: `Bullish,Bearish,Range`
- `status` فقط: `Public,Private` (نوت: در خروجی به `public/private` نگاشت می‌شود)
- `image` (در صورت ارسال): MIME یکی از `image/jpeg|image/png|image/webp` و اندازه ≤ `5MB`
- فیلدهای الزامی قبل از ارسال بررسی شوند و خطاها کنار فیلدها نمایش داده شوند

## سناریوهای UI و رفتارها
- لیست ایده‌ها:
  - فیلترهای بالا با اعمال فوری (debounce برای نماد و تگ‌ها)
  - کارت‌ها با رنگ/برچسب روند: bullish سبز، bearish قرمز، range خاکستری
  - صفحه‌بندی یا بارگذاری تنبل با شمارنده `total`
- جزئیات ایده:
  - عنوان برجسته، تصویر بزرگ، توضیح، متادیتا (نماد/تایم‌فریم/روند/تگ‌ها)
- ایجاد/ویرایش:
  - ورودی‌ها با انتخاب‌گرهای از پیش‌تعریف‌شده، پیش‌نمایش تصویر، مدیریت تگ‌ها
  - ارسال با `multipart/form-data` و نمایش پیشرفت آپلود (Progress bar اختیاری)
- حذف:
  - تاییدیه قبل از اقدام؛ خطایابی در صورت عدم مجوز یا عدم وجود آیتم

## مدیریت خطا و نمایش پیام‌ها
- پاسخ‌ها در قالب `HandlerResult` همراه با `RequestStatus` و `Message`
- نگاشت پیام‌ها:
  - `Successful` → نمایش موفقیت
  - `ValidationFailed`/`Failed` → نمایش خطا و highlight فیلدهای مشکل‌دار
  - `NotFound` → نمایش پیام عدم یافتن
  - `Unauthorized` → هدایت به ورود
- نمایش toast/alert براساس `RequestStatus` و به‌روزرسانی UI مطابق نتیجه

## نمونه درخواست‌ها برای توسعه UI
- ایجاد (curl):
```bash
curl -X POST "{BASE}/api/ideas" \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: multipart/form-data" \
  -F "symbol=BTCUSDT" \
  -F "timeframe=1h" \
  -F "trend=Bullish" \
  -F "title=Breakout محتمل روی 1H" \
  -F "description=انتظار شکست مقاومت و پولبک" \
  -F "status=Public" \
  -F "tags[]=breakout" \
  -F "tags[]=btc" \
  -F "image=@C:/path/to/image.png;type=image/png"
```
- دریافت لیست:
```bash
curl "{BASE}/api/ideas?symbol=BTCUSDT&timeframe=1h&trend=Bullish&tags=breakout,btc&page=1&pageSize=12"
```
- بروزرسانی:
```bash
curl -X PUT "{BASE}/api/ideas/123" \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: multipart/form-data" \
  -F "title=به‌روزرسانی عنوان" \
  -F "image=@C:/path/new.png;type=image/png"
```
- حذف:
```bash
curl -X DELETE "{BASE}/api/ideas/123" -H "Authorization: Bearer {token}"
```

## نکات تکمیلی برای UI
- تگ‌ها در ایجاد/ویرایش به‌صورت `tags[]` ارسال شوند تا صحیح bind شوند
- نمایش تصویر با URL برگشتی (`/uploads/ideas/{file}`) و در نبود تصویر، placeholder
- هماهنگی زبان: مقادیر انتخابی `trend` و `timeframe` به انگلیسی مطابق API ارسال شوند و در UI با معادل فارسی نمایش داده شوند
- حفظ وضعیت فیلترها در URL (QueryString) برای share و bookmark

— در صورت تایید، پیاده‌سازی UI مطابق این پرامپت انجام خواهد شد؛ اگر نیاز به تغییر قالب پاسخ یا افزودن متادیتا دارید، اعلام کنید تا به مستند افزوده شود.