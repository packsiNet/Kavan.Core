using ApplicationLayer.Common.Enums;
using ApplicationLayer.Features.DuneGasPrice.Query;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Kavan.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = nameof(ApiDefinitions.Public))]
public class DuneGasPriceController(IMediator mediator) : ControllerBase
{
    [HttpGet("latest")]
    public async Task<IActionResult> GetLatestAsync()
        => Ok(await mediator.Send(new GetLatestGasPriceQuery()));
}
