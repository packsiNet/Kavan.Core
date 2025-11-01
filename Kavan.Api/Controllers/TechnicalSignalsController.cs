using ApplicationLayer.Dto.TechnicalSignals;
using ApplicationLayer.Features.TechnicalSignals.Query;
using DomainLayer.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Kavan.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TechnicalSignalsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TechnicalSignalsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get technical signals with filtering options
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TechnicalSignalDto>>> GetTechnicalSignals(
        [FromQuery] string symbol = null,
        [FromQuery] string indicatorCategory = null,
        [FromQuery] string indicatorName = null,
        [FromQuery] SignalType? signalType = null,
        [FromQuery] string timeFrame = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        var filter = new TechnicalSignalFilterDto
        {
            Symbol = symbol,
            IndicatorCategory = indicatorCategory,
            IndicatorName = indicatorName,
            SignalType = signalType,
            TimeFrame = timeFrame,
            FromDate = fromDate,
            ToDate = toDate,
            Page = page,
            PageSize = Math.Min(pageSize, 100) // Limit max page size
        };

        var query = new GetTechnicalSignalsQuery(filter);
        var result = await _mediator.Send(query);
        
        return Ok(result);
    }

    /// <summary>
    /// Get technical signals summary grouped by indicator
    /// </summary>
    [HttpGet("summary")]
    public async Task<ActionResult<IEnumerable<TechnicalSignalSummaryDto>>> GetTechnicalSignalsSummary(
        [FromQuery] string symbol = null,
        [FromQuery] string timeFrame = null)
    {
        var query = new GetTechnicalSignalsSummaryQuery(symbol, timeFrame);
        var result = await _mediator.Send(query);
        
        return Ok(result);
    }

    /// <summary>
    /// Get all available indicator categories
    /// </summary>
    [HttpGet("categories")]
    public async Task<ActionResult<IEnumerable<string>>> GetCategories()
    {
        var query = new GetTechnicalSignalCategoriesQuery();
        var result = await _mediator.Send(query);
        
        return Ok(result);
    }

    /// <summary>
    /// Get all available indicators, optionally filtered by category
    /// </summary>
    [HttpGet("indicators")]
    public async Task<ActionResult<IEnumerable<string>>> GetIndicators(
        [FromQuery] string category = null)
    {
        var query = new GetTechnicalSignalIndicatorsQuery(category);
        var result = await _mediator.Send(query);
        
        return Ok(result);
    }

    /// <summary>
    /// Get signals for a specific symbol with latest data
    /// </summary>
    [HttpGet("symbol/{symbol}")]
    public async Task<ActionResult<IEnumerable<TechnicalSignalDto>>> GetSignalsBySymbol(
        string symbol,
        [FromQuery] string timeFrame = null,
        [FromQuery] SignalType? signalType = null)
    {
        var filter = new TechnicalSignalFilterDto
        {
            Symbol = symbol.ToUpperInvariant(),
            TimeFrame = timeFrame,
            SignalType = signalType,
            FromDate = DateTime.UtcNow.AddHours(-24), // Last 24 hours
            Page = 1,
            PageSize = 100
        };

        var query = new GetTechnicalSignalsQuery(filter);
        var result = await _mediator.Send(query);
        
        return Ok(result);
    }

    /// <summary>
    /// Get signals by indicator category
    /// </summary>
    [HttpGet("category/{category}")]
    public async Task<ActionResult<IEnumerable<TechnicalSignalDto>>> GetSignalsByCategory(
        string category,
        [FromQuery] string symbol = null,
        [FromQuery] string timeFrame = null,
        [FromQuery] SignalType? signalType = null)
    {
        var filter = new TechnicalSignalFilterDto
        {
            Symbol = symbol,
            IndicatorCategory = category,
            TimeFrame = timeFrame,
            SignalType = signalType,
            FromDate = DateTime.UtcNow.AddHours(-24), // Last 24 hours
            Page = 1,
            PageSize = 100
        };

        var query = new GetTechnicalSignalsQuery(filter);
        var result = await _mediator.Send(query);
        
        return Ok(result);
    }

    /// <summary>
    /// Get all signal categories with their detailed signal types
    /// دریافت تمام دسته‌بندی‌های سیگنال با انواع جزئی آن‌ها
    /// </summary>
    [HttpGet("signal-categories")]
    public async Task<ActionResult<IEnumerable<SignalCategoryDto>>> GetSignalCategories()
    {
        var query = new GetSignalCategoriesQuery();
        var result = await _mediator.Send(query);
        
        return Ok(result);
    }

    /// <summary>
    /// Get cryptocurrencies that have a specific signal type
    /// دریافت ارزهایی که سیگنال خاصی دارند
    /// </summary>
    [HttpGet("signals/{detailedSignalType:int}/symbols")]
    public async Task<ActionResult<IEnumerable<SignalSymbolDto>>> GetSymbolsBySignalType(
        int detailedSignalType,
        [FromQuery] string timeFrame = null,
        [FromQuery] SignalType? signalType = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        if (!Enum.IsDefined(typeof(DetailedSignalType), detailedSignalType))
        {
            return BadRequest($"Invalid signal type: {detailedSignalType}");
        }

        var query = new GetSymbolsBySignalTypeQuery(
            (DetailedSignalType)detailedSignalType,
            timeFrame,
            signalType,
            fromDate,
            toDate,
            page,
            Math.Min(pageSize, 100) // Limit max page size
        );

        var result = await _mediator.Send(query);
        
        return Ok(result);
    }
}