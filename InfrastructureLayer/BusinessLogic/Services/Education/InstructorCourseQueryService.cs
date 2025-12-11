using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.DTOs.Education;
using ApplicationLayer.Interfaces;
using ApplicationLayer.Interfaces.Services;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureLayer.BusinessLogic.Services.Education;

[InjectAsScoped]
public class InstructorCourseQueryService(
    IRepository<Course> courseRepository,
    IUserContextService userContext
) : IInstructorCourseQueryService
{
    public async Task<Result<List<CourseDto>>> GetMyCreatedAsync()
    {
        var userId = userContext.UserId;
        if (userId is null)
            return Result<List<CourseDto>>.Failure(Error.Authentication());

        var list = await courseRepository.Query()
            .Where(x => x.OwnerUserId == userId)
            .Select(x => new CourseDto
            {
                Id = x.Id,
                Title = x.Title,
                Slug = x.Slug,
                Description = x.Description,
                Goal = x.Goal,
                Price = x.Price,
                IsFree = x.IsFree,
                CourseLevelValue = x.CourseLevelValue,
                CategoryId = x.CategoryId,
                OwnerUserId = x.OwnerUserId ?? 0
            })
            .ToListAsync();

        return Result<List<CourseDto>>.Success(list);
    }
}
