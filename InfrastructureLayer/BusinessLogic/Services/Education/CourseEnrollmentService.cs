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
public class CourseEnrollmentService(
    IRepository<Course> courseRepository,
    IRepository<CourseEnrollment> enrollmentRepository,
    IUserContextService userContext
) : ICourseEnrollmentService
{
    public async Task<Result<EnrollmentDto>> EnrollAsync(EnrollCourseDto dto)
    {
        var userId = userContext.UserId;
        if (userId is null)
            return Result<EnrollmentDto>.Failure(Error.Authentication("کاربر احراز نشده است"));

        var course = await courseRepository.Query().FirstOrDefaultAsync(x => x.Id == dto.CourseId);
        if (course is null)
            return Result<EnrollmentDto>.Failure(Error.NotFound("دوره یافت نشد"));

        var already = await enrollmentRepository.Query().AnyAsync(x => x.CourseId == dto.CourseId && x.UserAccountId == userId);
        if (already)
            return Result<EnrollmentDto>.Failure(new Error("EXISTS", "قبلاً در این دوره عضو شده‌اید", RequestStatus.Exists));

        var isPaid = !course.IsFree;
        if (isPaid && string.IsNullOrWhiteSpace(dto.PaymentReference))
            return Result<EnrollmentDto>.Failure(Error.Validation("برای دوره پولی، مرجع پرداخت الزامی است"));

        var entity = new CourseEnrollment
        {
            CourseId = dto.CourseId,
            UserAccountId = userId.Value,
            IsPaid = isPaid,
            PricePaid = isPaid ? course.Price : 0,
            CouponCode = dto.CouponCode,
            PaymentReference = dto.PaymentReference,
            EnrolledAt = DateTime.UtcNow
        };

        await enrollmentRepository.AddAsync(entity);

        var result = new EnrollmentDto
        {
            Id = entity.Id,
            CourseId = entity.CourseId,
            UserAccountId = entity.UserAccountId,
            IsPaid = entity.IsPaid,
            PricePaid = entity.PricePaid,
            CouponCode = entity.CouponCode,
            PaymentReference = entity.PaymentReference,
            EnrolledAt = entity.EnrolledAt
        };

        return Result<EnrollmentDto>.Success(result);
    }
}
