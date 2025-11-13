# طراحی موجودیت‌های دامنه ماژول آموزش

## مرور معماری فعلی
- موجودیت‌ها از `BaseEntityModel` با کلید `int identity` ارث‌بری می‌کنند (`DomainLayer/Common/BaseEntities/BaseEntityModel.cs:3`).
- کانفیگ EF Core به‌صورت کلاس‌های جدا و از طریق `ApplyConfigurationsFromAssembly` اعمال می‌شود (`InfrastructureLayer/Context/ApplicationDbContext.cs:33`).
- فیلترهای سراسری و Shadow Properties ممیزی فعال هستند (`InfrastructureLayer/Configuration/BaseEntityConfiguration.cs:12–17`, `InfrastructureLayer/Convention/AuditableShadowProperties.cs:64–75`).
- اسمارت‌انام‌ها در `ApplicationLayer/Common/Enums` پیاده‌سازی شده‌اند، نمونه: `SmartEnum<NotificationType, byte>` (`ApplicationLayer/Common/Enums/NotificationType.cs:9`).

## تصمیمات کلیدی
- کلید اصلی همه موجودیت‌ها `int identity` مطابق قرارداد فعلی.
- نگاشت سطح/نوع رسانه با `SmartEnum` و ذخیره مقدار پایه به‌صورت `byte` در دیتابیس.
- دسته‌بندی دوره‌ها به‌صورت «هر دوره یک دسته» برای سادگی فاز اول؛ در صورت نیاز، ارتقا به چند-به-چند با افزودن موجودیت واسط.
- حذف‌ها «نرم» هستند؛ روابط با `OnDelete(Restrict)` تا از حذف فیزیکی آبشاری جلوگیری شود.
- ایندکس یکتا بر `Course.Slug` برای انتشار پایدار.

## مدل‌ها (DomainLayer/Entities)
- Course
  - فیلدها: `Title`، `Slug`، `Description`، `Goal`، `Price (decimal)`، `IsFree (bool)`، `CourseLevelValue (byte)`، `CategoryId (int)`، اختیاری `OwnerUserId (int)` برای مدرس.
- Lesson
  - فیلدها: `CourseId`، `Title`، `Description`، `Order (int)`، `PublishAt (DateTime?)`، `IsFreePreview (bool)`، `DurationSeconds (int?)`.
- MediaFile
  - فیلدها: `LessonId`، `FileName`، `StorageKey`، `MimeType`، `SizeBytes (long)`، `DurationSeconds (int?)`، `MediaFileTypeValue (byte)`، `IsStreamOnly (bool)`.
- CourseCategory
  - فیلدها: `Name`، `Slug`، `Description`.

## اسمارت‌انام‌ها (ApplicationLayer/Common/Enums)
- CourseLevel : `Beginner = 1`، `Intermediate = 2`، `Advanced = 3`، `Professional = 4`.
- MediaFileType : `Video = 1`، `Document = 2`، `Attachment = 3`.

## نگاشت EF Core (InfrastructureLayer/Configuration)
- CourseConfiguration
  - `HasIndex(Slug).IsUnique()`؛ طول‌های مناسب برای `Title/Slug`؛ `Price` با `decimal(18,2)`.
  - رابطه‌ها: `HasOne(Category).WithMany().HasForeignKey(CategoryId).OnDelete(Restrict)`؛ در صورت وجود `OwnerUserId`، نگاشت به `UserAccount` با `Restrict`.
- LessonConfiguration
  - ایندکس ترکیبی `(CourseId, Order)`؛ `PublishAt` اختیاری.
  - رابطه: `HasOne(Course).WithMany().HasForeignKey(CourseId).OnDelete(Restrict)`.
- MediaFileConfiguration
  - ایندکس `(LessonId, MediaFileTypeValue)` برای جست‌وجو؛ محدودیت طول برای `FileName/StorageKey/MimeType`.
  - رابطه: `HasOne(Lesson).WithMany().HasForeignKey(LessonId).OnDelete(Restrict)`.
- CourseCategoryConfiguration
  - `HasIndex(Slug).IsUnique()`؛ محدودیت طول برای `Name/Slug`.

## مسیرها و نام‌گذاری فایل‌ها
- DomainLayer/Entities/Course.cs
- DomainLayer/Entities/Lesson.cs
- DomainLayer/Entities/MediaFile.cs
- DomainLayer/Entities/CourseCategory.cs
- ApplicationLayer/Common/Enums/CourseLevel.cs
- ApplicationLayer/Common/Enums/MediaFileType.cs
- InfrastructureLayer/Configuration/CourseConfiguration.cs
- InfrastructureLayer/Configuration/LessonConfiguration.cs
- InfrastructureLayer/Configuration/MediaFileConfiguration.cs
- InfrastructureLayer/Configuration/CourseCategoryConfiguration.cs

## گام‌های اجرا پس از تایید
1. افزودن موجودیت‌ها در دامنه با ارث‌بری از `BaseEntityModel` و پیاده‌سازی `IAuditableEntity`.
2. افزودن اسمارت‌انام‌ها در `ApplicationLayer/Common/Enums` با نوع پایه `byte`.
3. ایجاد کلاس‌های کانفیگ EF Core برای هر موجودیت و تنظیم روابط/ایندکس‌ها.
4. به‌روزرسانی `ApplicationDbContext` در صورت نیاز به `DbSet<T>`های جدید (در صورت استفاده مستقیم).
5. اجرای مایگریشن EF Core و بازبینی اسکیما.
6. تست‌های حداقلی: ایجاد/خواندن/روابط (Course→Lesson→MediaFile) و یکتایی `Slug`.

## ارجاعات معماری برای تطبیق
- `InfrastructureLayer/Context/ApplicationDbContext.cs:33–37` (اعمال کانفیگ‌ها و فیلترها)
- `InfrastructureLayer/Configuration/BaseEntityConfiguration.cs:12–17` (کلید و فیلترهای سراسری)
- `ApplicationLayer/Common/Enums/NotificationType.cs:9–29` (الگوی SmartEnum)
- `InfrastructureLayer/Configuration/CandleBaseConfiguration.cs:19–28` (نمونه نگاشت روابط و ایندکس‌ها)

لطفاً تایید کنید تا پیاده‌سازی دقیق مطابق این طرح آغاز شود.