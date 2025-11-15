using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.DTOs.Education;

namespace ApplicationLayer.Interfaces.Services;

public interface IStreamAccessService
{
    Task<Result<StreamTokenDto>> IssueTokenAsync(int courseId, int lessonId, int userId);

    Task<Result<string>> ValidateTokenAsync(string token);
}