## گروه و مسیرها
- گروه Swagger: Public
- کنترلر: `Kavan.Api/Controllers/AuthController.cs`

## Signup
- روش: POST
- مسیر: `/api/auth/signup`
- احراز هویت: ندارد
- Content-Type: `application/json`
- بدنه درخواست: `SignUpDto`
  - `displayName` (string, الزامی)
  - `email` (string, الزامی، فرمت ایمیل)
  - `phonePrefix` (string، الزامی) مثال: `+98`
  - `phoneNumber` (string، الزامی) مثال: `9121234567`
  - `password` (string، الزامی، حداقل 6 کاراکتر)
  - `inviteCode` (string، اختیاری)
- پاسخ: `AuthorizeResultDto`
  - `accessTokens` (string، JWT)
  - `refreshToken` (string)
  - `tokenId` (string)
  - `userFullName` (string)
- کدهای وضعیت (درون `HandlerResult.RequestStatus`):
  - Success → 200 با آبجکت بالا
  - Duplicate/ValidationFailed → 400/409 با پیام مناسب
- نکات UI:
  - پس از موفقیت، ذخیره‌ی `accessTokens` و `refreshToken`
  - نمایش پیام موفقیت، ریدایرکت به داشبورد یا وضعیت ورود

## Signin
- روش: POST
- مسیر: `/api/auth/signin`
- احراز هویت: ندارد
- Content-Type: `application/json`
- بدنه درخواست: `SignInDto`
  - `validationMethod` (int، الزامی)
    - 1 = ورود با نام کاربری/رمز
    - 2 = OTP موبایل
    - 3 = OTP ایمیل
  - `userName` (string، الزامی) 
    - در روش 1: نام کاربری یا شماره تلفن نرمال‌سازی‌شده
    - در روش 2/3: ایمیل یا شماره موبایل مرتبط
  - `phonePrefix` (string، اختیاری، در صورت ورود با موبایل)
  - `password` (string، الزامی برای روش 1)
  - `securityCode` (int، الزامی برای روش 2/3)
- پاسخ: `AuthorizeResultDto`
  - `accessTokens`, `refreshToken`, `tokenId`, `userFullName`
- کدهای وضعیت:
  - Success → 200
  - IncorrectUser/NotFound/ValidationFailed → 400/404
- نکات UI:
  - برای OTP، ابتدا فرایند ارسال کد را در UI/سرویس مجزا فراخوانی کنید (در صورت نیاز)، سپس `signin` با کد ارسال‌شده
  - مدیریت خطاهای نام کاربری/رمز نادرست با نمایش پیام مناسب

## Refresh Token
- روش: POST
- مسیر: `/api/auth/token/refresh`
- احراز هویت: ندارد (با توکن‌های ارسالی اعتبارسنجی می‌شود)
- Content-Type: `application/json`
- بدنه درخواست: `TokenRequestDto`
  - `accessTokens` (string، JWT فعلی که منقضی شده یا در حال انقضا)
  - `refreshToken` (string، الزامی)
- پاسخ: `AuthorizeResultDto`
  - `accessTokens` (جدید)
  - `refreshToken` (جدید)
  - `userFullName`
- کدهای وضعیت:
  - Success → 200
  - ValidationFailed/ExpiredToken/NotFound → 400/401/404
- نکات:
  - توکن قبلی با JTI بررسی می‌شود؛ در صورت معتبر بودن و منقضی‌شدن دسترسی، RefreshToken جدید صادر و قبلی مصرف می‌شود
  - در کلاینت، پس از دریافت، جایگزینی `accessTokens` و `refreshToken`

## Revoke (اختیاری برای مدیریت نشست‌ها)
- روش: POST
- مسیر: `/api/auth/token/revoke`
- احراز هویت: لازم (`Users`)
- Content-Type: `application/json`
- بدنه: `{ userId: number }`
- پاسخ: موفق/ناموفق (بدون آبجکت)
- کاربرد: خروج سراسری و ابطال RefreshTokenهای فعال کاربر

## هدرها و احراز هویت در کلاینت
- هدر Authorization برای درخواست‌های محافظت‌شده: `Authorization: Bearer {accessTokens}`
- پس از `signup`/`signin`: ذخیره‌ی توکن‌ها و ارسال در درخواست‌های بعدی

## نگاشت وضعیت‌ها برای UI
- `RequestStatus.Successful` → نمایش موفقیت و ادامه‌ی جریان
- `IncorrectUser/NotFound` → پیام خطای کاربر/سابقه‌ی ناموجود
- `ValidationFailed` → نمایش خطاهای اعتبارسنجی فرم
- `ExpiredToken` → درخواست رفرش توکن

## نمونه‌های درخواست
- Signup نمونه:
```json
POST /api/auth/signup
{
  "displayName": "Ali Trader",
  "email": "ali@example.com",
  "phonePrefix": "+98",
  "phoneNumber": "9121234567",
  "password": "My$tr0ngPass",
  "inviteCode": "INV-2025"
}
```
- Signin با رمز:
```json
POST /api/auth/signin
{
  "validationMethod": 1,
  "userName": "9121234567",
  "phonePrefix": "+98",
  "password": "My$tr0ngPass"
}
```
- Signin با OTP موبایل:
```json
POST /api/auth/signin
{
  "validationMethod": 2,
  "userName": "9121234567",
  "phonePrefix": "+98",
  "securityCode": 12345
}
```
- RefreshToken:
```json
POST /api/auth/token/refresh
{
  "accessTokens": "<expired-or-expiring-jwt>",
  "refreshToken": "<refresh-token>"
}
```

## مسیرهای کد مرجع
- کنترلر: `Kavan.Api/Controllers/AuthController.cs`
- هندلر رفرش: `ApplicationLayer/Features/Identity/Handler/TokenRequestHandler.cs`
- سرویس‌ها: `InfrastructureLayer/BusinessLogic/Services/IdentityService.cs`, `RefreshTokenService.cs`, `UserAccountServices.cs`

در صورت نیاز، نمونه پاسخ‌های موفق/خطا را هم اضافه می‌کنم تا به‌صورت کامل در UI مصرف شوند.