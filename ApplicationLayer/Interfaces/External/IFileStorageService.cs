using ApplicationLayer.Dto.BaseDtos;
using Microsoft.AspNetCore.Http;

namespace ApplicationLayer.Interfaces.External;

public interface IFileStorageService
{
    Task<Result<string>> SaveIdeaImageAsync(IFormFile file, CancellationToken cancellationToken = default);
    Task<Result<string>> SaveProfileImageAsync(IFormFile file, string kind, CancellationToken cancellationToken = default);
}