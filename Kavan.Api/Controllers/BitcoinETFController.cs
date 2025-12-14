using ApplicationLayer.Common.Enums;
using ApplicationLayer.Common.Extensions;
using ApplicationLayer.Features.BitcoinETF.Query;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Kavan.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = nameof(ApiDefinitions.Public))]
public class BitcoinETFController(IMediator mediator) : ControllerBase
{
    [HttpGet("latest-metrics")]
    public async Task<IActionResult> GetLatestAsync()
        => await ResultHelper.GetResultAsync(mediator, new GetLatestBitcoinEtfMetricsQuery());

    [HttpGet("latest-tx-count")]
    public async Task<IActionResult> GetLatestTxCountAsync()
        => await ResultHelper.GetResultAsync(mediator, new GetLatestBitcoinTxCountQuery());

    [HttpGet("bitcoin-active-wallet")]
    public async Task<IActionResult> GetLatestUsersAsync()
        => await ResultHelper.GetResultAsync(mediator, new GetLatestBitcoinUserCountQuery());
}
