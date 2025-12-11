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
public class CourseCategoriesController(IMediator mediator) : ControllerBase
{
    [HttpGet("dropdown")]
    public async Task<IActionResult> GetDropdownAsync()
        => await ResultHelper.GetResultAsync(mediator, new GetCourseCategoriesQuery());
}
