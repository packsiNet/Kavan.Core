## اهداف
- ایجاد پروفایل سازمانی کامل و قابل جست‌وجو برای کاربران شرکت
- نمایش عمومی اطلاعات شرکت و ارائه فرم مدیریت برای مالک

## انتیتی‌ها و اسکیمای دیتابیس
- OrganizationProfile (1:1 با UserAccount)
  - Id, UserAccountId (FK), IsPublic(bool)
  - OrganizationName, LegalName, Description, FoundedYear(int?), RegistrationNumber
  - Country, City, Address
  - ContactEmailPublic, ContactPhonePublic
  - LogoUrl, BannerUrl
  - HasExchange(bool), HasInvestmentPanel(bool)
  - ایندکس‌ها: IX_OrganizationProfile_UserAccountId (Unique), IX_OrganizationProfile_IsPublic, IX_OrganizationProfile_Country
- OrganizationActivity (1:N)
  - Id, OrganizationProfileId(FK), Title, Description
  - ایندکس: IX_OrganizationActivity_OrganizationProfileId
- OrganizationWebsite (1:N)
  - Id, OrganizationProfileId(FK), Url, WebsiteType(SmartEnum: Main|Blog|Support|Docs)
  - ایندکس: IX_OrganizationWebsite_OrganizationProfileId
- OrganizationSocialLink (1:N)
  - Id, OrganizationProfileId(FK), Platform(SmartEnum: Twitter|Telegram|LinkedIn|Instagram|YouTube|GitHub), Url
  - ایندکس: IX_OrganizationSocialLink_OrganizationProfileId
- OrganizationLicense (1:N)
  - Id, OrganizationProfileId(FK), RegulatorName, LicenseNumber, Country
  - ایندکس: IX_OrganizationLicense_OrganizationProfileId
- OrganizationExchange (اختیاری 1:N)
  - Id, OrganizationProfileId(FK), Name, Country, Url
  - ایندکس: IX_OrganizationExchange_OrganizationProfileId
- OrganizationInvestmentPanel (اختیاری 1:N)
  - Id, OrganizationProfileId(FK), Name, Url, MinimumInvestment(decimal?), ProfitShareModel(string)
  - ایندکس: IX_OrganizationInvestmentPanel_OrganizationProfileId

## SmartEnumها
- OrganizationServiceType: Exchange, InvestmentPanel, Brokerage, PropTrading, Education, Advisor, Wallet, Research, Media
- WebsiteType: Main, Blog, Support, Docs
- SocialPlatform: Twitter(X), Telegram, LinkedIn, Instagram, YouTube, GitHub

## DTOها
- OrganizationProfileDto: فیلدهای سازمانی + مجموعه‌ها (Activities[], Websites[], SocialLinks[], Licenses[], Exchanges[], InvestmentPanels[])
- CreateOrganizationProfileDto/UpdateOrganizationProfileDto:
  - فیلدهای پایه + `IFormFile Logo`, `IFormFile Banner`
  - مجموعه‌ها به‌صورت آرایه‌های ساده (Title/Description/Url/...)
- PublicProfileDto: اگر کاربر سازمانی باشد OrganizationProfileDto؛ در غیر این‌صورت UserProfileDto

## سرویس‌ها
- ApplicationLayer.Interfaces/Profiles/IOrganizationProfileService:
  - GetPublicByUserIdAsync(userId)
  - GetMineAsync(userId)
  - UpsertAsync(userId, UpdateOrganizationProfileDto, files)
  - UploadLogoAsync(userId, IFormFile)
  - UploadBannerAsync(userId, IFormFile)
- پیاده‌سازی در InfrastructureLayer/BusinessLogic/Services/Profiles/OrganizationProfileService.cs با `[InjectAsScoped]`
  - استفاده از `IRepository<T>` برای هر انتیتی، `IUnitOfWork` برای تراکنش، `IUserContextService` برای مالکیت
  - ذخیره رسانه با `IFileStorageService` در مسیر `/uploads/profiles`

## CQRS و ولیدیشن
- Feature: ApplicationLayer/Features/Profiles/Organizations/
  - Commands: UpsertOrganizationProfileCommand, UploadLogoCommand, UploadBannerCommand
  - Queries: GetPublicProfileByUserIdQuery, GetMyOrganizationProfileQuery, SearchOrganizationsQuery (فیلتر بر اساس Country, HasExchange, HasInvestmentPanel, ServiceType)
  - Handlers: تزریق سرویس؛ خروجی `HandlerResult`
  - Validators: اعتبارسنجی فیلدها و فایل‌ها (نوع تصویر و حجم)

## کنترلرها
- PublicProfilesController (گروه Public):
  - GET `/api/profiles/{userId}` → نمای عمومی پروفایل
  - GET `/api/profiles/organizations/search` → جست‌وجو با پارامترهای فیلتر
- MyProfileController (گروه Trader):
  - GET `/api/my/organization` → دریافت پروفایل سازمانی مالک
  - PUT `/api/my/organization` (multipart/form-data) → ساخت/ویرایش با لوگو/بنر اختیاری
  - POST `/api/my/organization/logo` (multipart/form-data) → آپلود لوگو
  - POST `/api/my/organization/banner` (multipart/form-data) → آپلود بنر

## امنیت و دسترسی
- مشاهده عمومی براساس `IsPublic`
- ویرایش فقط مالک (بر اساس `UserContextService.UserId`) یا ادمین
- پاکسازی فایل قدیمی هنگام جایگزینی لوگو/بنر

## مهاجرت و کانفیگ EF
- افزودن کلاس‌های Configuration برای هر انتیتی و ثبت ایندکس‌ها
- اجرای Migration و به‌روزرسانی دیتابیس (سازگاری با Convention و Shadow Properties موجود)

## خروجی‌های UI
- کارت‌های شرکت با لوگو، نام، کشور، خدمات، وضعیت HasExchange/HasInvestmentPanel
- صفحه جزئیات با تب‌های «اطلاعات پایه»، «فعالیت‌ها»، «مجوزها»، «وب‌سایت‌ها»، «شبکه‌های اجتماعی»، «رسانه‌ها»
- فرم مدیریت با اعتبارسنجی و آپلود تصویر

اگر تأیید می‌کنید، مطابق این طرح پیاده‌سازی کامل (انتیتی‌ها، DTOها، سرویس، CQRS، کنترلرها و ولیدیشن‌ها) انجام می‌شود و بیلد/تست جهت تأیید صحت انجام خواهد شد.