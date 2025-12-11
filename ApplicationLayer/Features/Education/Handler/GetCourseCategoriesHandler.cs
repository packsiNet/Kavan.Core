using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.DTOs.Education;
using ApplicationLayer.Extensions;
using ApplicationLayer.Features.Education.Query;
using ApplicationLayer.Interfaces;
using DomainLayer.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApplicationLayer.Features.Education.Handler;

public class GetCourseCategoriesHandler(IRepository<CourseCategory> categoryRepository) : IRequestHandler<GetCourseCategoriesQuery, HandlerResult>
{
    public async Task<HandlerResult> Handle(GetCourseCategoriesQuery request, CancellationToken cancellationToken)
    {
        var list = await categoryRepository
            .Query()
            .OrderBy(x => x.Name)
            .Select(x => new CourseCategoryDto
            {
                Id = x.Id,
                Name = x.Name
            })
            .ToListAsync(cancellationToken);

        return Result<List<CourseCategoryDto>>.Success(list).ToHandlerResult();
    }
}
