using ApplicationLayer.Features.Signals.Query;
using ApplicationLayer.Features.SignalsCatalog.Commands;
using ApplicationLayer.Features.SignalsCatalog.Query;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Kavan.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SignalsCatalogController(IMediator mediator) : ControllerBase
    {
        [HttpPost("seed")]
        public async Task<IActionResult> Seed()
            => Ok(await mediator.Send(new SeedSignalCatalogCommand()));

        [HttpGet("tree")]
        public async Task<IActionResult> GetTree()
            => Ok(await mediator.Send(new GetSignalCatalogTreeQuery()));

        [HttpGet("list")]
        public async Task<IActionResult> GetList()
            => Ok(await mediator.Send(new GetSignalCatalogListQuery()));

        [HttpGet("signals")] 
        public async Task<IActionResult> GetSignals([FromQuery] string category, [FromQuery] string name)
            => Ok(await mediator.Send(new GetSignalsByClassificationQuery(category, name)));
    }
}