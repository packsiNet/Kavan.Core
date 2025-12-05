using ApplicationLayer.Common.Enums;
using ApplicationLayer.Common.Extensions;
using ApplicationLayer.DTOs.Plans;
using ApplicationLayer.Features.Plans.Commands;
using ApplicationLayer.Features.Plans.Query;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kavan.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = nameof(ApiDefinitions.Public))]
[Authorize(Roles = "User")]
public class PlansController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// دریافت لیست همه پلن‌ها
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllAsync()
        => await ResultHelper.GetResultAsync(mediator, new GetPlansQuery());

    /// <summary>
    /// دریافت جزئیات پلن بر اساس شناسه
    /// </summary>
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByIdAsync(int id)
        => await ResultHelper.GetResultAsync(mediator, new GetPlanByIdQuery(id));

    /// <summary>
    /// ایجاد پلن جدید
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreatePlanDto model)
        => await ResultHelper.GetResultAsync(mediator, new CreatePlanCommand(model));

    /// <summary>
    /// بروزرسانی پلن موجود
    /// </summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] UpdatePlanDto model)
        => await ResultHelper.GetResultAsync(mediator, new UpdatePlanCommand(id, model));

    /// <summary>
    /// حذف پلن
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAsync(int id)
        => await ResultHelper.GetResultAsync(mediator, new DeletePlanCommand(id));
}