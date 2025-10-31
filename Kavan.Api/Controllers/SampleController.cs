using ApplicationLayer.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Kavan.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SampleController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAsync()
        => await ResultHelper.GetResultAsync(mediator, new SampleGetQuery());
}