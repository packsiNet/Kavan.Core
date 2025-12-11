using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.DTOs.Education;

namespace ApplicationLayer.Interfaces.Services;

public interface IInstructorCourseQueryService
{
    Task<Result<List<CourseDto>>> GetMyCreatedAsync();
}
