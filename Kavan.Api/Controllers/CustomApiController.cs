using ApplicationLayer.Common.Enums;
using ApplicationLayer.Common.Extensions;
using ApplicationLayer.Features.CustomApis.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Kavan.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = nameof(ApiDefinitions.Public))]
public class CustomApiController(IMediator mediator) : ControllerBase
{
    [HttpGet("GetCandleHistory")]
    public async Task<IActionResult> GetCandleHistoryAsync([FromQuery] DateTime startDateUtc)
        => await ResultHelper.GetResultAsync(mediator, new GetCandleHistoryCommand(startDateUtc));
}