using ApplicationLayer.Common.Enums;
using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.DTOs.Education;
using ApplicationLayer.Interfaces;
using ApplicationLayer.Interfaces.Services;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureLayer.BusinessLogic.Services.Education;

[InjectAsScoped]
public class LessonService(
    IRepository<Course> courseRepository,
    IRepository<Lesson> lessonRepository,
    IRepository<MediaFile> mediaRepository,
    IUserContextService userContext
) : ILessonService
{
    public async Task<Result<LessonDto>> CreateAsync(CreateLessonDto dto)
    {
        var ownerId = userContext.UserId;
        if (ownerId is null)
            return Result<LessonDto>.Failure(Error.Authentication("کاربر احراز نشده است"));

        var course = await courseRepository.Query().FirstOrDefaultAsync(x => x.Id == dto.CourseId);
        if (course is null)
            return Result<LessonDto>.Failure(Error.NotFound("دوره یافت نشد"));

        if (course.OwnerUserId != ownerId)
            return Result<LessonDto>.Failure(new Error("AUTHZ_ERROR", "دسترسی مدیریت این دوره برای شما مجاز نیست", RequestStatus.AuthenticationFailed));

        var existsOrder = await lessonRepository.Query().AnyAsync(x => x.CourseId == dto.CourseId && x.Order == dto.Order);
        if (existsOrder)
            return Result<LessonDto>.Failure(Error.Duplicate("ترتیب جلسه در این دوره تکراری است"));

        var entity = new Lesson
        {
            CourseId = dto.CourseId,
            Title = dto.Title,
            Description = dto.Description,
            Order = dto.Order,
            PublishAt = dto.PublishAt,
            IsFreePreview = dto.IsFreePreview,
            DurationSeconds = dto.DurationSeconds
        };

        await lessonRepository.AddAsync(entity);

        var result = new LessonDto
        {
            Id = entity.Id,
            CourseId = entity.CourseId,
            Title = entity.Title,
            Description = entity.Description,
            Order = entity.Order,
            PublishAt = entity.PublishAt,
            IsFreePreview = entity.IsFreePreview,
            DurationSeconds = entity.DurationSeconds
        };

        return Result<LessonDto>.Success(result);
    }

    public async Task<Result<MediaFileDto>> AddMediaAsync(AddMediaFileDto dto)
    {
        var ownerId = userContext.UserId;
        if (ownerId is null)
            return Result<MediaFileDto>.Failure(Error.Authentication("کاربر احراز نشده است"));

        var lesson = await lessonRepository.Query().FirstOrDefaultAsync(x => x.Id == dto.LessonId);
        if (lesson is null)
            return Result<MediaFileDto>.Failure(Error.NotFound("جلسه یافت نشد"));

        var course = await courseRepository.Query().FirstOrDefaultAsync(x => x.Id == lesson.CourseId);
        if (course is null)
            return Result<MediaFileDto>.Failure(Error.NotFound("دوره یافت نشد"));

        if (course.OwnerUserId != ownerId)
            return Result<MediaFileDto>.Failure(new Error("AUTHZ_ERROR", "دسترسی مدیریت این دوره برای شما مجاز نیست", RequestStatus.AuthenticationFailed));

        var entity = new MediaFile
        {
            LessonId = dto.LessonId,
            FileName = dto.FileName,
            StorageKey = dto.StorageKey,
            MimeType = dto.MimeType,
            SizeBytes = dto.SizeBytes,
            DurationSeconds = dto.DurationSeconds,
            MediaFileTypeValue = dto.MediaFileTypeValue,
            IsStreamOnly = true
        };

        await mediaRepository.AddAsync(entity);

        var result = new MediaFileDto
        {
            Id = entity.Id,
            LessonId = entity.LessonId,
            FileName = entity.FileName,
            StorageKey = entity.StorageKey,
            MimeType = entity.MimeType,
            SizeBytes = entity.SizeBytes,
            DurationSeconds = entity.DurationSeconds,
            MediaFileTypeValue = entity.MediaFileTypeValue,
            IsStreamOnly = entity.IsStreamOnly
        };

        return Result<MediaFileDto>.Success(result);
    }

    public async Task<Result> ScheduleAsync(int lessonId, DateTime? publishAt)
    {
        var ownerId = userContext.UserId;
        if (ownerId is null)
            return Result.Failure(Error.Authentication());

        var lesson = await lessonRepository.Query().FirstOrDefaultAsync(x => x.Id == lessonId);
        if (lesson is null)
            return Result.Failure(Error.NotFound("جلسه یافت نشد"));

        var course = await courseRepository.Query().FirstOrDefaultAsync(x => x.Id == lesson.CourseId);
        if (course is null)
            return Result.Failure(Error.NotFound("دوره یافت نشد"));

        if (course.OwnerUserId != ownerId)
            return Result.Failure(new Error("AUTHZ_ERROR", "دسترسی مدیریت این دوره برای شما مجاز نیست", RequestStatus.AuthenticationFailed));

        lesson.PublishAt = publishAt;
        await lessonRepository.UpdateAsync(lesson);
        return Result.Success();
    }

    public async Task<Result> DeleteAsync(int lessonId)
    {
        var ownerId = userContext.UserId;
        if (ownerId is null)
            return Result.Failure(Error.Authentication());

        var lesson = await lessonRepository.Query().FirstOrDefaultAsync(x => x.Id == lessonId);
        if (lesson is null)
            return Result.Failure(Error.NotFound("جلسه یافت نشد"));

        var course = await courseRepository.Query().FirstOrDefaultAsync(x => x.Id == lesson.CourseId);
        if (course is null)
            return Result.Failure(Error.NotFound("دوره یافت نشد"));

        if (course.OwnerUserId != ownerId)
            return Result.Failure(new Error("AUTHZ_ERROR", "دسترسی مدیریت این دوره برای شما مجاز نیست", RequestStatus.AuthenticationFailed));

        lessonRepository.Remove(lesson);
        return Result.Success();
    }
}