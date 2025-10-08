using ApplicationLayer.Common.Extensions;
using ApplicationLayer.Features.Signals.Query;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace PresentationApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "Signals")]
    public class SignalsController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        /// <summary>
        /// دریافت تمام سیگنال‌ها
        /// </summary>
        [HttpGet("All")]
        public async Task<IActionResult> GetAllAsync()
        {
            var query = new GetSignalsQuery(Symbol: null, TimeFrame: null, Limit: null);
            return await ResultHelper.GetResultAsync(_mediator, query);
        }

        /// <summary>
        /// دریافت سیگنال‌ها بر اساس Symbol
        /// </summary>
        [HttpGet("BySymbol/{symbol}")]
        public async Task<IActionResult> GetBySymbolAsync([FromRoute] string symbol)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                return BadRequest("Symbol نمی‌تواند خالی باشد.");

            var query = new GetSignalsQuery(Symbol: symbol, TimeFrame: null, Limit: null);
            return await ResultHelper.GetResultAsync(_mediator, query);
        }

        /// <summary>
        /// دریافت سیگنال‌ها بر اساس TimeFrame
        /// </summary>
        [HttpGet("ByTimeFrame/{timeFrame}")]
        public async Task<IActionResult> GetByTimeFrameAsync([FromRoute] string timeFrame)
        {
            if (string.IsNullOrWhiteSpace(timeFrame))
                return BadRequest("TimeFrame نمی‌تواند خالی باشد.");

            var query = new GetSignalsQuery(Symbol: null, TimeFrame: timeFrame, Limit: null);
            return await ResultHelper.GetResultAsync(_mediator, query);
        }

        /// <summary>
        /// دریافت آخرین سیگنال‌ها (به‌صورت پیش‌فرض 50 آیتم)
        /// </summary>
        [HttpGet("Latest")]
        public async Task<IActionResult> GetLatestAsync([FromQuery] int limit = 50)
        {
            if (limit <= 0)
                return BadRequest("Limit باید بزرگ‌تر از صفر باشد.");

            var query = new GetSignalsQuery(Symbol: null, TimeFrame: null, Limit: limit);
            return await ResultHelper.GetResultAsync(_mediator, query);
        }
    }
}