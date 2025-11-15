using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.DTOs.Education;

namespace ApplicationLayer.Interfaces.Services;

public interface ICourseEnrollmentService
{
    Task<Result<EnrollmentDto>> EnrollAsync(EnrollCourseDto dto);
}