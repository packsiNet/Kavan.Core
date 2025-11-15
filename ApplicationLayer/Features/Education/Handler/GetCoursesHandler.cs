using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.DTOs.Education;
using ApplicationLayer.Extensions;
using ApplicationLayer.Features.Education.Query;
using ApplicationLayer.Interfaces;
using DomainLayer.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApplicationLayer.Features.Education.Handler;

public class GetCoursesHandler(IRepository<Course> courseRepository) : IRequestHandler<GetCoursesQuery, HandlerResult>
{
    public async Task<HandlerResult> Handle(GetCoursesQuery request, CancellationToken cancellationToken)
    {
        var query = courseRepository.Query();
        if (request.CategoryId.HasValue)
            query = query.Where(x => x.CategoryId == request.CategoryId.Value);
        if (request.CourseLevelValue.HasValue)
            query = query.Where(x => x.CourseLevelValue == request.CourseLevelValue.Value);
        if (request.IsFree.HasValue)
            query = query.Where(x => x.IsFree == request.IsFree.Value);

        var list = await query.Select(x => new CourseDto
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
        }).ToListAsync(cancellationToken);

        return Result<List<CourseDto>>.Success(list).ToHandlerResult();
    }
}