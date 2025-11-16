using ApplicationLayer.Common.Enums;
using ApplicationLayer.Common.Extensions;
using ApplicationLayer.Features.Education.Query;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kavan.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = nameof(ApiDefinitions.Public))]
[AllowAnonymous]
public class CoursesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllAsync([FromQuery] int? categoryId, [FromQuery] byte? level, [FromQuery] bool? isFree)
        => await ResultHelper.GetResultAsync(mediator, new GetCoursesQuery(categoryId, level, isFree));

    [HttpGet("{courseId:int}/lessons/{lessonId:int}/stream")]
    //[Authorize(Policy = nameof(ApiDefinitions.Public), Roles = "Users,Instructor")]
    public async Task<IActionResult> GetLessonStreamTokenAsync(int courseId, int lessonId)
        => await ResultHelper.GetResultAsync(mediator, new GetLessonStreamTokenQuery(courseId, lessonId));
}