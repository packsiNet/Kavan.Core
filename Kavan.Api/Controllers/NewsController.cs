using ApplicationLayer.Common.Extensions;
using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.Dto.News;
using ApplicationLayer.Features.News.Commands;
using ApplicationLayer.Features.News.Query;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Kavan.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = "Public")]
public class NewsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAsync(
        [FromQuery] string[] currencies,
        [FromQuery] string[] regions,
        [FromQuery] string? kind,
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var model = new GetNewsRequestDto
        {
            Currencies = currencies ?? Array.Empty<string>(),
            Regions = regions ?? Array.Empty<string>(),
            Kind = kind,
            Search = search,
            Pagination = new PaginationDto { Page = page, PageSize = pageSize }
        };

        return await ResultHelper.GetResultAsync(mediator, new GetNewsQuery(model));
    }

    [HttpPost("sync")]
    public async Task<IActionResult> SyncAsync([FromBody] CryptoPanicQuery? query)
    {
        var q = query ?? new CryptoPanicQuery { Public = true };
        return await ResultHelper.GetResultAsync(mediator, new SyncNewsCommand(q));
    }

    public class AuthTokenRequest { public string AuthToken { get; set; } }

    [HttpPost("sync-with-token")]
    public async Task<IActionResult> SyncWithTokenAsync([FromBody] AuthTokenRequest request)
    {
        if (string.IsNullOrWhiteSpace(request?.AuthToken))
            return BadRequest("authToken is required");

        var previous = Environment.GetEnvironmentVariable("CRYPTOPANIC_AUTH_TOKEN");
        Environment.SetEnvironmentVariable("CRYPTOPANIC_AUTH_TOKEN", request.AuthToken);
        try
        {
            var q = new CryptoPanicQuery { Public = true };
            return await ResultHelper.GetResultAsync(mediator, new SyncNewsCommand(q));
        }
        finally
        {
            Environment.SetEnvironmentVariable("CRYPTOPANIC_AUTH_TOKEN", previous);
        }
    }
}
