using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.Interfaces.External;
using DomainLayer.Common.Attributes;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace InfrastructureLayer.BusinessLogic.Services.Uploads;

[InjectAsScoped]
public class FileStorageService(IWebHostEnvironment _env) : IFileStorageService
{
    public async Task<Result<string>> SaveIdeaImageAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        if (file == null || file.Length == 0)
            return Result<string>.ValidationFailure("فایل تصویر ارسال نشده است");

        var allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "image/jpeg", "image/png", "image/webp"
        };
        if (!allowed.Contains(file.ContentType))
            return Result<string>.ValidationFailure("فرمت تصویر مجاز نیست (jpeg/png/webp)");

        const long maxSize = 5 * 1024 * 1024; // 5MB
        if (file.Length > maxSize)
            return Result<string>.ValidationFailure("حجم تصویر بیش از 5MB است");

        var uploadsRoot = Path.Combine(_env.WebRootPath ?? Path.Combine(AppContext.BaseDirectory, "wwwroot"), "uploads", "ideas");
        Directory.CreateDirectory(uploadsRoot);

        var ext = Path.GetExtension(file.FileName);
        if (string.IsNullOrWhiteSpace(ext))
        {
            ext = file.ContentType switch
            {
                "image/jpeg" => ".jpg",
                "image/png" => ".png",
                "image/webp" => ".webp",
                _ => ".bin"
            };
        }

        var fileName = $"{Guid.NewGuid():N}{ext}";
        var fullPath = Path.Combine(uploadsRoot, fileName);
        using (var stream = new FileStream(fullPath, FileMode.CreateNew))
        {
            await file.CopyToAsync(stream, cancellationToken);
        }

        var url = $"/uploads/ideas/{fileName}";
        return Result<string>.Success(url);
    }
}