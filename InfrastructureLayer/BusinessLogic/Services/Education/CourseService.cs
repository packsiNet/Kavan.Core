using ApplicationLayer.Common;
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
public class CourseService(
    IRepository<Course> courseRepository,
    IRepository<CourseCategory> categoryRepository,
    IUserContextService userContext
) : ICourseService
{
    public async Task<Result<CourseDto>> CreateAsync(CreateCourseDto dto)
    {
        var ownerId = userContext.UserId;
        if (ownerId is null)
            return Result<CourseDto>.Failure(Error.Authentication("کاربر احراز نشده است"));

        var slugExists = await courseRepository.Query().AnyAsync(x => x.Slug == dto.Slug);
        if (slugExists)
            return Result<CourseDto>.Failure(Error.Duplicate("شناسه یکتا (Slug) تکراری است"));

        var categoryExists = await categoryRepository.Query().AnyAsync(x => x.Id == dto.CategoryId);
        if (!categoryExists)
            return Result<CourseDto>.Failure(Error.NotFound("دسته‌بندی دوره یافت نشد"));

        var entity = new Course
        {
            Title = dto.Title,
            Slug = dto.Slug,
            Description = dto.Description,
            Goal = dto.Goal,
            Price = dto.Price,
            IsFree = dto.IsFree,
            CourseLevelValue = dto.CourseLevelValue,
            CategoryId = dto.CategoryId,
            OwnerUserId = ownerId
        };

        await courseRepository.AddAsync(entity);

        var result = new CourseDto
        {
            Id = entity.Id,
            Title = entity.Title,
            Slug = entity.Slug,
            Description = entity.Description,
            Goal = entity.Goal,
            Price = entity.Price,
            IsFree = entity.IsFree,
            CourseLevelValue = entity.CourseLevelValue,
            CategoryId = entity.CategoryId,
            OwnerUserId = entity.OwnerUserId ?? 0
        };

        return Result<CourseDto>.Success(result);
    }

    public async Task<Result<CourseDto>> UpdateAsync(int id, UpdateCourseDto dto)
    {
        var ownerId = userContext.UserId;
        if (ownerId is null)
            return Result<CourseDto>.Failure(Error.Authentication("کاربر احراز نشده است"));

        var entity = await courseRepository.Query().FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null)
            return Result<CourseDto>.Failure(Error.NotFound("دوره یافت نشد"));

        if (entity.OwnerUserId != ownerId)
            return Result<CourseDto>.Failure(new Error("AUTHZ_ERROR", "دسترسی مدیریت این دوره برای شما مجاز نیست", RequestStatus.AuthenticationFailed));

        var slugExists = await courseRepository.Query().AnyAsync(x => x.Slug == dto.Slug && x.Id != id);
        if (slugExists)
            return Result<CourseDto>.Failure(Error.Duplicate("شناسه یکتا (Slug) تکراری است"));

        entity.Title = dto.Title;
        entity.Slug = dto.Slug;
        entity.Description = dto.Description;
        entity.Goal = dto.Goal;
        entity.Price = dto.Price;
        entity.IsFree = dto.IsFree;
        entity.CourseLevelValue = dto.CourseLevelValue;
        entity.CategoryId = dto.CategoryId;

        await courseRepository.UpdateAsync(entity);

        var result = new CourseDto
        {
            Id = entity.Id,
            Title = entity.Title,
            Slug = entity.Slug,
            Description = entity.Description,
            Goal = entity.Goal,
            Price = entity.Price,
            IsFree = entity.IsFree,
            CourseLevelValue = entity.CourseLevelValue,
            CategoryId = entity.CategoryId,
            OwnerUserId = entity.OwnerUserId ?? 0
        };

        return Result<CourseDto>.Success(result);
    }

    public async Task<Result> SetPricingAsync(int id, decimal price, bool isFree)
    {
        var ownerId = userContext.UserId;
        if (ownerId is null)
            return Result.Failure(Error.Authentication());

        var entity = await courseRepository.Query().FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null)
            return Result.Failure(Error.NotFound("دوره یافت نشد"));

        if (entity.OwnerUserId != ownerId)
            return Result.Failure(new Error("AUTHZ_ERROR", "دسترسی مدیریت این دوره برای شما مجاز نیست", RequestStatus.AuthenticationFailed));

        entity.Price = price;
        entity.IsFree = isFree;
        await courseRepository.UpdateAsync(entity);
        return Result.Success();
    }

    public async Task<Result> DeleteAsync(int id)
    {
        var ownerId = userContext.UserId;
        if (ownerId is null)
            return Result.Failure(Error.Authentication());

        var entity = await courseRepository.Query().FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null)
            return Result.Failure(Error.NotFound("دوره یافت نشد"));

        if (entity.OwnerUserId != ownerId)
            return Result.Failure(new Error("AUTHZ_ERROR", "دسترسی مدیریت این دوره برای شما مجاز نیست", RequestStatus.AuthenticationFailed));

        courseRepository.Remove(entity);
        return Result.Success();
    }
}
