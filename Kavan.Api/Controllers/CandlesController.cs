using ApplicationLayer.Common.Enums;
using ApplicationLayer.Common.Extensions;
using ApplicationLayer.Features.Candles.Query;
using ApplicationLayer.Features.Cryptocurrencies.Query;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Kavan.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = nameof(ApiDefinitions.Trader))]
public class CandlesController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Retrieves aggregated historical candles from the local database.
    /// Does not include the currently forming (live) candle.
    /// </summary>
    /// <param name="cryptocurrencyId">ID of the cryptocurrency</param>
    /// <param name="timeframe">e.g. 5m, 1h, 4h, 1d</param>
    /// <param name="limit">Max number of candles (default 100)</param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetAsync([FromQuery] int cryptocurrencyId, [FromQuery] string timeframe, [FromQuery] int limit = 100)
        => await ResultHelper.GetResultAsync(mediator, new GetCandlesQuery(cryptocurrencyId, timeframe, limit));

    /// <summary>
    /// Retrieves list of supported timeframes.
    /// </summary>
    /// <returns>List of timeframes</returns>
    [HttpGet("timeframes")]
    public async Task<IActionResult> GetTimeframesAsync()
        => await ResultHelper.GetResultAsync(mediator, new GetTimeframesQuery());

    /// <summary>
    /// Retrieves list of cryptocurrencies with Id and Symbol.
    /// </summary>
    /// <returns>List of cryptocurrencies</returns>
    [HttpGet("cryptocurrencies")]
    public async Task<IActionResult> GetCryptocurrenciesAsync()
        => await ResultHelper.GetResultAsync(mediator, new GetCryptocurrenciesQuery());
}
