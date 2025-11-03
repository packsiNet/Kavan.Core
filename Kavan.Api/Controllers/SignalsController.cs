using ApplicationLayer.Common.Enums;
using ApplicationLayer.Common.Extensions;
using ApplicationLayer.Dto.Signals;
using ApplicationLayer.Features.Signals.Query;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kavan.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = nameof(ApiDefinitions.Trader))]
public class SignalsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Analyze signals for given request
    /// </summary>
    [HttpPost("analyze")]
    [AllowAnonymous]
    public async Task<IActionResult> AnalyzeAsync([FromBody] SignalRequestDto model)
        => await ResultHelper.GetResultAsync(mediator, new GetSignalsQuery(model));
}