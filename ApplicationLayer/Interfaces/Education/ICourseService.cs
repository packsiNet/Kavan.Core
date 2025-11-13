using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.DTOs.Education;

namespace ApplicationLayer.Interfaces.Services;

public interface ICourseService
{
    Task<Result<CourseDto>> CreateAsync(CreateCourseDto dto);
    Task<Result<CourseDto>> UpdateAsync(int id, UpdateCourseDto dto);
    Task<Result> SetPricingAsync(int id, decimal price, bool isFree);
    Task<Result> DeleteAsync(int id);
}
