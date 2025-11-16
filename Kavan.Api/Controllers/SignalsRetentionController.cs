using ApplicationLayer.Dto.SignalsRetention;
using ApplicationLayer.Features.SignalsRetention.Commands;
using ApplicationLayer.Features.SignalsRetention.Query;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Kavan.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SignalsRetentionController(IMediator mediator) : ControllerBase
    {
        [HttpPost("purge")]
        public async Task<ActionResult<PurgeOldSignalsResultDto>> Purge([FromBody] PurgeOldSignalsCommand cmd)
            => Ok(await mediator.Send(cmd));

        [HttpGet("status")]
        public async Task<ActionResult<RetentionStatusDto>> Status()
            => Ok(await mediator.Send(new GetRetentionStatusQuery()));
    }
}