using ApplicationLayer.Common.Enums;
using ApplicationLayer.Common.Extensions;
using ApplicationLayer.Dto.AI;
using ApplicationLayer.Features.Ai.Query;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kavan.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = nameof(ApiDefinitions.Public))]
public class AiController(IMediator mediator) : ControllerBase
{
    [HttpPost("translate")]
    [AllowAnonymous]
    public async Task<IActionResult> TranslateAsync([FromBody] AiTranslateRequestDto model)
        => await ResultHelper.GetResultAsync(mediator, new TranslateTextQuery(model));
}

