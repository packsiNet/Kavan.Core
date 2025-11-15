using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.DTOs.Education;

namespace ApplicationLayer.Interfaces.Services;

public interface ILessonService
{
    Task<Result<LessonDto>> CreateAsync(CreateLessonDto dto);

    Task<Result<MediaFileDto>> AddMediaAsync(AddMediaFileDto dto);

    Task<Result> ScheduleAsync(int lessonId, DateTime? publishAt);

    Task<Result> DeleteAsync(int lessonId);
}