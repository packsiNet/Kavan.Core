using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.DTOs.Education;
using ApplicationLayer.Interfaces;
using ApplicationLayer.Interfaces.Services;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

namespace InfrastructureLayer.BusinessLogic.Services.Streaming;

[InjectAsScoped]
public class StreamAccessService(
    IRepository<Course> courseRepository,
    IRepository<Lesson> lessonRepository,
    IRepository<CourseEnrollment> enrollmentRepository
) : IStreamAccessService
{
    private static readonly ConcurrentDictionary<string, (int lessonId, DateTime expires)> _tokens = new();

    public async Task<Result<StreamTokenDto>> IssueTokenAsync(int courseId, int lessonId, int userId)
    {
        var course = await courseRepository.Query().FirstOrDefaultAsync(x => x.Id == courseId);
        if (course is null)
            return Result<StreamTokenDto>.Failure(Error.NotFound("دوره یافت نشد"));

        var lesson = await lessonRepository.Query().FirstOrDefaultAsync(x => x.Id == lessonId && x.CourseId == courseId);
        if (lesson is null)
            return Result<StreamTokenDto>.Failure(Error.NotFound("جلسه یافت نشد"));

        var now = DateTime.UtcNow;
        var isPublished = !lesson.PublishAt.HasValue || lesson.PublishAt <= now || lesson.IsFreePreview;
        if (!isPublished)
            return Result<StreamTokenDto>.Failure(Error.Validation("زمان انتشار این جلسه هنوز فرا نرسیده است"));

        var isOwner = course.OwnerUserId.HasValue && course.OwnerUserId.Value == userId;
        var hasAccess = course.IsFree || isOwner || await enrollmentRepository.Query().AnyAsync(x => x.CourseId == courseId && x.UserAccountId == userId);
        if (!hasAccess)
            return Result<StreamTokenDto>.Failure(Error.Authentication("دسترسی به این محتوا برای شما مجاز نیست"));

        var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        var expires = now.AddMinutes(15);
        _tokens[token] = (lessonId, expires);
        return Result<StreamTokenDto>.Success(new StreamTokenDto { Token = token, ExpiresAt = expires });
    }

    public Task<Result<string>> ValidateTokenAsync(string token)
    {
        if (!_tokens.TryGetValue(token, out var entry))
            return Task.FromResult(Result<string>.Failure(Error.Validation("توکن نامعتبر است")));

        if (entry.expires < DateTime.UtcNow)
        {
            _tokens.TryRemove(token, out _);
            return Task.FromResult(Result<string>.Failure(Error.ExpiredToken()));
        }

        return Task.FromResult(Result<string>.Success(token));
    }
}