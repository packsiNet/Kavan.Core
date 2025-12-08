using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.Interfaces.External;
using DomainLayer.Common.Attributes;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace InfrastructureLayer.BusinessLogic.Services.Uploads;

[InjectAsScoped]
public class FileStorageService(IWebHostEnvironment _env, IConfiguration _configuration) : IFileStorageService
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

        // Get domain from config or fallback
        var domain = _configuration["Domain"]?.TrimEnd('/') ?? "https://api.packsi.net";
        var url = $"{domain}/uploads/ideas/{fileName}";
        
        return Result<string>.Success(url);
    }

    public async Task<Result<string>> SaveProfileImageAsync(IFormFile file, string kind, CancellationToken cancellationToken = default)
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

        var uploadsRoot = Path.Combine(_env.WebRootPath ?? Path.Combine(AppContext.BaseDirectory, "wwwroot"), "uploads", "profiles");
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

        var fileName = $"{kind}_{Guid.NewGuid():N}{ext}";
        var fullPath = Path.Combine(uploadsRoot, fileName);
        using (var stream = new FileStream(fullPath, FileMode.CreateNew))
        {
            await file.CopyToAsync(stream, cancellationToken);
        }

        var domain = _configuration["Domain"]?.TrimEnd('/') ?? "https://api.packsi.net";
        var url = $"{domain}/uploads/profiles/{fileName}";
        
        return Result<string>.Success(url);
    }
}