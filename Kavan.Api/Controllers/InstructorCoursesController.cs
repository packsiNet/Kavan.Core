using ApplicationLayer.Common.Enums;
using ApplicationLayer.Common.Extensions;
using ApplicationLayer.DTOs.Education;
using ApplicationLayer.Features.Education.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kavan.Api.Controllers;

[Route("api/instructor/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = nameof(ApiDefinitions.Public))]
[Authorize(Roles = "Instructor")]
public class InstructorCoursesController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateCourseAsync([FromBody] CreateCourseDto model)
        => await ResultHelper.GetResultAsync(mediator, new CreateCourseCommand(model));

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateCourseAsync(int id, [FromBody] UpdateCourseDto model)
        => await ResultHelper.GetResultAsync(mediator, new UpdateCourseCommand(id, model));

    [HttpPut("{id:int}/pricing")]
    public async Task<IActionResult> SetPricingAsync(int id, [FromBody] SetCoursePricingDto model)
        => await ResultHelper.GetResultAsync(mediator, new SetCoursePricingCommand(id, model));

    [HttpPost("{courseId:int}/lessons")]
    public async Task<IActionResult> CreateLessonAsync(int courseId, [FromBody] CreateLessonDto model)
        => await ResultHelper.GetResultAsync(mediator, new CreateLessonCommand(new CreateLessonDto
        {
            CourseId = courseId,
            Title = model.Title,
            Description = model.Description,
            Order = model.Order,
            PublishAt = model.PublishAt,
            IsFreePreview = model.IsFreePreview,
            DurationSeconds = model.DurationSeconds
        }));

    [HttpPut("lessons/{lessonId:int}/schedule")]
    public async Task<IActionResult> ScheduleLessonAsync(int lessonId, [FromBody] ScheduleLessonDto model)
        => await ResultHelper.GetResultAsync(mediator, new ScheduleLessonCommand(lessonId, model));

    [HttpPost("lessons/{lessonId:int}/media")]
    public async Task<IActionResult> AddMediaAsync(int lessonId, [FromBody] AddMediaFileDto model)
        => await ResultHelper.GetResultAsync(mediator, new AddMediaFileCommand(new AddMediaFileDto
        {
            LessonId = lessonId,
            FileName = model.FileName,
            StorageKey = model.StorageKey,
            MimeType = model.MimeType,
            SizeBytes = model.SizeBytes,
            DurationSeconds = model.DurationSeconds,
            MediaFileTypeValue = model.MediaFileTypeValue
        }));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCourseAsync(int id)
        => await ResultHelper.GetResultAsync(mediator, new DeleteCourseCommand(id));

    [HttpDelete("lessons/{lessonId:int}")]
    public async Task<IActionResult> DeleteLessonAsync(int lessonId)
        => await ResultHelper.GetResultAsync(mediator, new DeleteLessonCommand(lessonId));
}