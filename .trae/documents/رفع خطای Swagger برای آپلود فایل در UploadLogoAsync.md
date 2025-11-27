## مسئله
- SwaggerGen در تولید Operation برای اکشن `UploadLogoAsync` خطا می‌دهد چون روی پارامتر `IFormFile` از `[FromForm]` استفاده شده است؛ الگوی صحیح برای فرم‌های چندبخشی (multipart/form-data) باید رعایت شود.

## راه‌حل پیشنهادی
### 1) امضای اکشن را مطابق الگوی استاندارد اصلاح کنیم
- گزینه A (ساده):
  - اکشن:
  - ```csharp
    [HttpPost("my-profile/logo", Name = "UploadLogo")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> UploadLogoAsync(
        [FromForm] string description,
        [FromForm] DateTime clientDate,
        IFormFile file)
    {
        // ارسال به Mediator یا منطق ذخیره
        return Ok();
    }
    ```
  - نکته: برای `IFormFile`، اتریبیوت `[FromForm]` نگذاریم.

- گزینه B (توصیه‌شده و سازگار با CQRS):
  - DTO فرم (فقط پارامتر اکشن دارای `[FromForm]` باشد):
  - ```csharp
    public class UploadLogoForm
    {
        public string Description { get; set; }
        public DateTime ClientDate { get; set; }
        public IFormFile File { get; set; }
    }
    ```
  - اکشن کنترلر:
  - ```csharp
    [HttpPost("my-profile/logo", Name = "UploadLogo")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> UploadLogoAsync([FromForm] UploadLogoForm form)
    {
        await _mediator.Send(new UploadLogoCommand(form.File, form.Description, form.ClientDate));
        return Ok();
    }
    ```
  - این روش با قوانین پروژه سازگار است (کنترلر فقط Mediator را صدا می‌زند).

### 2) پیاده‌سازی CQRS مرتبط
- مسیرها:
  - `ApplicationLayer/Dto/Profile/UploadLogoForm.cs`
  - `ApplicationLayer/Features/Profile/Commands/UploadLogoCommand.cs`
  - `ApplicationLayer/Features/Profile/Handler/UploadLogoHandler.cs`
  - `ApplicationLayer/Features/Profile/Validations/UploadLogoValidator.cs`
- هندلر: دریافت فایل، اعتبارسنجی نوع/حجم، ذخیره فایل در ذخیره‌ساز، بروزرسانی رکورد کاربر (از طریق `IUnitOfWork`).
- ولیدیشن: محدودیت حجم (مثلاً ≤ 2MB)، پسوند مجاز (png/jpg), الزام وجود فایل.

### 3) سفارشی‌سازی Swagger (اختیاری)
- تنظیم OperationId برای خوانایی بهتر:
  - ```csharp
    [HttpPost("my-profile/logo", Name = "UploadLogo")]
    ```
- یا در تنظیمات Swagger:
  - ```csharp
    services.AddSwaggerGen(o => {
      o.CustomOperationIds(api => api.TryGetMethodInfo(out var mi) ? mi.Name : null);
    });
    ```

### 4) چند فایل (در صورت نیاز)
- پشتیبانی از چند فایل: `IList<IFormFile> Files` داخل فرم؛ Swagger به‌درستی ورودی‌ها را نمایش می‌دهد.

### 5) آزمون و تأیید
- باز کردن `swagger.json` برای گروه مربوطه و مشاهده عدم خطا.
- تست با curl:
  - ```bash
    curl -F "description=logo" -F "clientDate=2025-11-22" -F "file=@logo.png" http://localhost:5005/api/my-profile/logo
    ```

## خروجی مورد انتظار
- خطای SwaggerGen رفع می‌شود؛ صفحه Swagger فرم آپلود با فیلدها و فایل به‌صورت صحیح نمایش داده می‌شود.

اگر موافق هستید، گزینه B را پیاده‌سازی می‌کنم تا ضمن رفع خطا، الگوی CQRS پروژه نیز رعایت شود.