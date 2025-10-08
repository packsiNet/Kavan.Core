using ApplicationLayer.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace PresentationApp.Controllers
{
    [ApiExplorerSettings(GroupName = "Users")]
    [Route("api/[controller]")]
    [ApiController]
    public class SampleController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpPost]
        [Route("Assign")]
        public async Task<IActionResult> AssignParcelFleetAsync()
            => Ok();
            //=> await ResultHelper.GetResultAsync(_mediator, );
    }
}
