using ApplicationLayer.DTOs.Education;
using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.Extensions;
using ApplicationLayer.Features.Education.Query;
using ApplicationLayer.Interfaces;
using DomainLayer.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApplicationLayer.Features.Education.Handler;

public class GetMyCoursesHandler(IRepository<CourseEnrollment> enrollmentRepository, IRepository<Course> courseRepository, IUserContextService userContext) : IRequestHandler<GetMyCoursesQuery, HandlerResult>
{
    public async Task<HandlerResult> Handle(GetMyCoursesQuery request, CancellationToken cancellationToken)
    {
        var userId = userContext.UserId;
        if (userId is null)
            return Result<List<CourseDto>>.Failure(Error.Authentication()).ToHandlerResult();

        var query = from e in enrollmentRepository.Query()
                    join c in courseRepository.Query() on e.CourseId equals c.Id
                    where e.UserAccountId == userId
                    select new CourseDto
                    {
                        Id = c.Id,
                        Title = c.Title,
                        Slug = c.Slug,
                        Description = c.Description,
                        Goal = c.Goal,
                        Price = c.Price,
                        IsFree = c.IsFree,
                        CourseLevelValue = c.CourseLevelValue,
                        CategoryId = c.CategoryId,
                        OwnerUserId = c.OwnerUserId ?? 0
                    };

        var list = await query.ToListAsync(cancellationToken);
        return Result<List<CourseDto>>.Success(list).ToHandlerResult();
    }
}
