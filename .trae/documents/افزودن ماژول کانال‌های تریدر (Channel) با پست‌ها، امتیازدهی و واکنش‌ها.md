## دامنه (DomainLayer)
- موجودیت‌ها:
  - Channel: `Id`, `OwnerUserId`, `Name`, `Slug`, `Category` (SmartEnum)، `AccessType` (SmartEnum)، `Description`, `IsActive`, `CreatedAt`.
  - ChannelMembership: `ChannelId`, `UserId`, `IsActive`, `JoinedAt` (یک عضو در هر کانال یک رکورد‌؛ ایندکس یونیک روی `(ChannelId, UserId)`).
  - ChannelRating: `ChannelId`, `UserId`, `Stars(1..5)`, `CreatedAt` (هر کاربر یک امتیاز در هر کانال؛ یونیک `(ChannelId, UserId)`).
  - ChannelPost: مشترک همه پست‌ها: `Id`, `ChannelId`, `Type` (SmartEnum: Signal, News), `Title`, `Description`, `ImageUrl`, `CreatedByUserId`, `CreatedAt`.
  - Signal جزئیات:
    - ChannelSignalDetail: `PostId`, `Symbol`, `Timeframe`, `TradeType` (SmartEnum: Spot/Futures), `StopLoss`.
    - ChannelSignalEntry: `PostId`, `Price` (چند رکورد برای ورودی‌ها).
    - ChannelSignalTp: `PostId`, `Price` (چند رکورد برای تی‌پی‌ها).
  - News جزئیات: ChannelNewsDetail: `PostId`, `Url` (اختیاری).
- اسمارت‌انوم‌ها (مطابق الگوی IdeaVisibility/IdeaTrend):
  - ChannelCategory: `SpotSignals`, `FuturesSignals`, `Announcements`.
  - ChannelAccessType: `Free`, `Paid`.
  - ChannelPostType: `Signal`, `News`.
  - TradeType: `Spot`, `Futures`.
- پیکربندی EF:
  - کانفیگ برای هر موجودیت در `InfrastructureLayer/Configuration/*Configuration.cs` با محدودیت‌ها، ایندکس‌ها، FKها و Shadow Properties (`CreatedByUserId`).

## لایه اپلیکیشن (ApplicationLayer)
- اینترفیس‌ها:
  - `IChannelService`: ساخت/ویرایش/حذف کانال، گرفتن کانال، جستجو، عضویت/لغو عضویت، امتیازدهی و دریافت آمار.
  - `IChannelPostService`: ساخت/ویرایش/حذف پست‌های Signal/News، دریافت پست‌های کانال، واکنش Like/Dislike و شمارنده‌ها.
- DTOها:
  - Channels:
    - CreateChannelDto: `Name`, `Category`, `AccessType`, `Description`.
    - UpdateChannelDto: همان فیلدها (قابل ویرایش).
    - ChannelDto: فیلدهای کانال + `MembersCount`, `AverageStars`, `RatingsCount`.
    - RateChannelDto: `ChannelId`, `Stars(1..5)`.
    - JoinChannelDto/LeaveChannelDto: `ChannelId`.
  - Posts:
    - CreateSignalPostDto: `ChannelId`, `Symbol`, `Timeframe`, `TradeType`, `EntryPoints[]`, `Tps[]`, `StopLoss`, `Title?`, `Description?`, `Image?`.
    - CreateNewsPostDto: `ChannelId`, `Title`, `Description`, `Image?`, `Url?`.
    - UpdateSignalPostDto / UpdateNewsPostDto (مشابه ساخت با امکان تغییر).
    - PostDto (مشترک) + جزئیات نوع در فیلدهای فرزند: `SignalDetail`, `NewsDetail`.
    - ReactPostDto: `PostId`, `Reaction` (Like/Dislike).
  - Paging: استفاده از `PaginationDto` موجود.
- فولدر Feature مطابق CQRS:
  - `ApplicationLayer/Features/Channels/Commands|Query|Handler|Validations` برای کانال‌ها.
  - `ApplicationLayer/Features/ChannelPosts/...` برای پست‌ها و واکنش‌ها.
- ولیدیشن‌ها (FluentValidation):
  - محدودیت نام کانال، دسته و دسترسی معتبر (SmartEnum)، امتیاز ۱ تا ۵، آرایه‌های Entry/TP خالی نباشند در سیگنال.
- هندلرها از `IUnitOfWork`، `IRepository<TEntity>`، `IUserContextService` استفاده کنند.

## لایه زیرساخت (InfrastructureLayer)
- پیاده‌سازی سرویس‌ها با [InjectAsScoped]:
  - `ChannelService`: 
    - Create/Update/Delete کانال (فقط مالک).
    - GetById، Search (فیلتر بر دسته/دسترسی)، GetMyChannels (بر اساس مالک).
    - Join/Leave: Free → عضویت فعال؛ Paid → ایجاد عضویت ولی `IsActive=false` (جایگزین پرداخت تا بعداً تکمیل شود).
    - Rate: فقط اعضای فعال اجازه امتیازدهی دارند؛ به‌روزرسانی امتیاز موجود یا ایجاد جدید؛ بازگشت آمار.
  - `ChannelPostService`:
    - CreateSignalPost/CreateNewsPost: فقط مالک کانال؛ ذخیره جزئیات و فرزندان.
    - Update/Delete پست: فقط مالک یا سازنده.
    - GetPostsByChannel: دسترسی خواندن بر اساس `AccessType` و عضویت؛ برای `Paid` فقط اعضا.
    - ReactToPost: اعضای کانال می‌توانند Like/Dislike بزنند؛ یک واکنش در هر کاربر/پست؛ تغییر واکنش مجاز.
- کوئری‌های کارا برای شمارنده‌ها و میانگین با `GroupBy`/Aggregate.
- ایندکس‌ها:
  - `(Channel.OwnerUserId)`, `(ChannelMembership.ChannelId,UserId)` یونیک، `(ChannelRating.ChannelId)`, `(PostReaction.PostId,UserId)` یونیک.

## API (Kavan.Api)
- `ChannelsController` (گروه Public برای مشاهده/جستجو، Trader برای عملیات مالک و عضویت):
  - GET `api/Channels/{id}` (عمومی)
  - GET `api/Channels` با فیلترها و صفحه‌بندی (عمومی)
  - GET `api/Channels/my` (کانال‌های متعلق به کاربر فعلی)
  - POST `api/Channels` (ساخت کانال؛ فقط User)
  - PUT `api/Channels/{id}` (ویرایش؛ فقط مالک)
  - DELETE `api/Channels/{id}` (حذف؛ فقط مالک)
  - POST `api/Channels/{id}/join` (عضویت؛ User)
  - POST `api/Channels/{id}/leave` (لغو عضویت)
  - POST `api/Channels/{id}/rate` (امتیازدهی؛ فقط اعضا)
- `ChannelPostsController`:
  - GET `api/ChannelPosts/channel/{channelId}` (لیست پست‌ها با صفحه‌بندی؛ کنترل دسترسی بر اساس نوع کانال و عضویت)
  - POST `api/ChannelPosts/signal` / `news` (ایجاد پست؛ فقط مالک)
  - PUT `api/ChannelPosts/{postId}` (ویرایش)
  - DELETE `api/ChannelPosts/{postId}` (حذف)
  - POST `api/ChannelPosts/{postId}/react` (Like/Dislike؛ فقط اعضا)

## کنترل دسترسی و مالکیت
- استفاده از `IUserContextService.UserId` برای مالکیت کانال و محدودیت‌های ویرایش.
- دسترسی خواندن پست‌ها:
  - `Free`: هر کاربر (حتی ناشناس) می‌تواند بخواند.
  - `Paid`: فقط اعضای فعال.
- امتیازدهی و واکنش‌ها: فقط اعضای کانال.

## ولیدیشن و قواعد کسب‌وکار
- امتیاز ستاره بین ۱ تا ۵.
- برای سیگنال: حداقل یک Entry و یک TP، `StopLoss` اجباری.
- جلوگیری از عضویت تکراری، امتیاز تکراری (آپدیت به‌جای ایجاد).
- حذف کانال، حذف پست‌ها و وابسته‌ها با Transaction.

## انطباق با معماری پروژه
- موجودیت‌ها در `DomainLayer/Entities`، بدون وابستگی به سایر لایه‌ها.
- CQRS در `ApplicationLayer/Features/...` با Command/Query/Handler/Validator و DTOها.
- سرویس‌ها در `InfrastructureLayer/BusinessLogic/Services/Channels` و `ChannelPosts` با [InjectAsScoped].
- کنترلرها فقط از `IMediator` استفاده کنند.

## ملاحظات پایگاه‌داده و کارایی
- ایندکس‌های ضروری برای عضویت، واکنش و امتیاز.
- صفحه‌بندی استاندارد (`PaginationDto`) در همه لیست‌ها.
- انتخاب فیلدهای ضروری در DTOها، اجتناب از N+1.

## گسترش‌های آینده (پرداخت)
- در `ChannelMembership` فیلدهای رزرو مانند `IsActive`, `ActivatedAt` نگه‌داری شود تا پس از اتصال پرداخت، فعال‌سازی عضویت انجام شود.
- Endpoint «ActivateMembership» پس از پرداخت قابل افزودن است.

## خروجی‌های مورد انتظار
- ایجاد/مدیریت کانال‌ها توسط تریدر، با نوع و دسترسی.
- امتیازدهی ستاره‌ای فقط توسط اعضا.
- واکنش Like/Dislike به پست‌ها.
- پست‌های سیگنال با جزئیات فنی و پست‌های خبری با ساختار ساده.

## تایید
اگر این طرح مورد تأیید است، پیاده‌سازی کامل را با ایجاد موجودیت‌ها، DTOها، CQRS Features، سرویس‌ها، کانفیگ‌های EF و کنترلرهای API شروع می‌کنم و پس از اتمام، بیلد و تست‌های اولیه را اجرا و نتایج را گزارش می‌دهم.