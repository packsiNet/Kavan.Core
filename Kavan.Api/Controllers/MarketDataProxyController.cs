using ApplicationLayer.Common.Enums;
using ApplicationLayer.Common.Extensions;
using ApplicationLayer.Features.MarketDataProxy.Query;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kavan.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = nameof(ApiDefinitions.Trader))]
public class MarketDataProxyController(IMediator mediator) : ControllerBase
{
    [HttpGet("binance/klines")]
    [AllowAnonymous]
    public async Task<IActionResult> GetBinanceKlinesAsync([FromQuery] string symbol, [FromQuery] string interval)
    {
        var effectiveInterval = string.IsNullOrWhiteSpace(interval) ? "1M" : interval;
        return await ResultHelper.GetResultAsync(mediator, new GetBinanceKlinesQuery(symbol, effectiveInterval));
    }

    [HttpGet("coinbase/candles")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCoinbaseCandlesAsync([FromQuery] string symbol, [FromQuery] int granularity)
        => await ResultHelper.GetResultAsync(mediator, new GetCoinbaseCandlesQuery(symbol, granularity));
}
