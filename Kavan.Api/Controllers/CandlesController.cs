using ApplicationLayer.Common.Enums;
using ApplicationLayer.Common.Extensions;
using ApplicationLayer.Dto.Candles;
using ApplicationLayer.Features.Candles.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kavan.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = nameof(ApiDefinitions.Admin))]
public class CandlesController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Seed 1m candles for BTCUSDT with double-top then breakout pattern
    /// </summary>
    [HttpPost("seed/btcusdt/1m")]
    [AllowAnonymous]
    public async Task<IActionResult> SeedBtcusdt1m([FromBody] SeedCandlesRequestDto model)
        => await ResultHelper.GetResultAsync(mediator, new SeedBTCUSDT1mCommand(model));
}