using ApplicationLayer.Common.Enums;
using ApplicationLayer.Common.Extensions;
using ApplicationLayer.DTOs.Education;
using ApplicationLayer.Features.Education.Commands;
using ApplicationLayer.Features.Education.Query;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kavan.Api.Controllers;

[Route("api/student/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = nameof(ApiDefinitions.Public))]
[Authorize(Policy = nameof(ApiDefinitions.Public), Roles = "Users")]
public class StudentCoursesController(IMediator mediator) : ControllerBase
{
    [HttpPost("{courseId:int}/enroll")]
    public async Task<IActionResult> EnrollAsync(int courseId, [FromBody] EnrollCourseDto model)
        => await ResultHelper.GetResultAsync(mediator, new EnrollCourseCommand(new EnrollCourseDto
        {
            CourseId = courseId,
            CouponCode = model.CouponCode,
            PaymentReference = model.PaymentReference
        }));

    [HttpGet("my")]
    public async Task<IActionResult> GetMyCoursesAsync()
        => await ResultHelper.GetResultAsync(mediator, new GetMyCoursesQuery());
}
