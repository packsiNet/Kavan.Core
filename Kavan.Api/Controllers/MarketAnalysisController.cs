using ApplicationLayer.Dto.MarketAnalysis;
using ApplicationLayer.Features.MarketAnalysis.Commands;
using ApplicationLayer.Features.MarketAnalysis.Query;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Kavan.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MarketAnalysisController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Analyzes cryptocurrency market based on provided conditions and filters
    /// </summary>
    /// <param name="command">Market analysis request with conditions, filters, and preferences</param>
    /// <returns>Market analysis results with trading signals</returns>
    [HttpPost("analyze")]
    public async Task<IActionResult> AnalyzeMarketAsync(AnalyzeMarketCommand command)
    {
        try
        {
            var result = await mediator.Send(command);
            return Ok(new
            {
                success = true,
                data = result,
                timestamp = DateTime.UtcNow,
                message = "Market analysis completed successfully"
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                success = false,
                error = "Invalid request parameters",
                details = ex.Message,
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                error = "Internal server error during market analysis",
                details = ex.Message,
                timestamp = DateTime.UtcNow
            });
        }
    }

    /// <summary>
    /// Gets active trading signals with optional filtering
    /// </summary>
    /// <param name="symbol">Optional symbol filter</param>
    /// <param name="timeframe">Optional timeframe filter</param>
    /// <param name="signalType">Optional signal type filter (BUY/SELL/HOLD)</param>
    /// <param name="pageNumber">Page number for pagination (default: 1)</param>
    /// <param name="pageSize">Page size for pagination (default: 20)</param>
    /// <returns>List of active trading signals</returns>
    [HttpGet("signals")]
    public async Task<IActionResult> GetActiveSignalsAsync(
        [FromQuery] string symbol = null,
        [FromQuery] string timeframe = null,
        [FromQuery] string signalType = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var query = new GetActiveSignalsQuery(symbol, timeframe, signalType, pageNumber, pageSize);
            var result = await mediator.Send(query);
            
            return Ok(new
            {
                success = true,
                data = result,
                pagination = new
                {
                    pageNumber,
                    pageSize,
                    totalCount = result?.Count ?? 0
                },
                timestamp = DateTime.UtcNow,
                message = "Active signals retrieved successfully"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                error = "Internal server error while retrieving signals",
                details = ex.Message,
                timestamp = DateTime.UtcNow
            });
        }
    }

    /// <summary>
    /// Gets market analysis for a specific symbol and timeframe
    /// </summary>
    /// <param name="symbol">Cryptocurrency symbol (e.g., BTCUSDT)</param>
    /// <param name="timeframe">Timeframe for analysis (1m, 5m, 1h, 4h, 1d)</param>
    /// <returns>Detailed market analysis for the specified symbol</returns>
    [HttpGet("analyze/{symbol}")]
    public async Task<IActionResult> GetSymbolAnalysisAsync(
        [FromRoute] string symbol,
        [FromQuery] string timeframe = "1h")
    {
        try
        {
            // Create a basic analysis command for single symbol
            var request = new MarketAnalysisRequestDto
            {
                Market = "crypto",
                Symbols = new List<string> { symbol.ToUpper() },
                Timeframes = new List<string> { timeframe },
                Conditions = new List<ApplicationLayer.Dto.MarketAnalysis.AnalysisConditionDto>(),
                Filters = new ApplicationLayer.Dto.MarketAnalysis.AnalysisFiltersDto
                {
                    VolumeMin = "medium",
                    Volatility = "medium",
                    Liquidity = "high"
                },
                Preferences = new ApplicationLayer.Dto.MarketAnalysis.AnalysisPreferencesDto
                {
                    RiskLevel = "medium",
                    StrategyType = "price_action",
                    SignalStrength = "medium"
                }
            };

            var command = new AnalyzeMarketCommand(request);
            var result = await mediator.Send(command);
            return Ok(new
            {
                success = true,
                data = result,
                symbol = symbol.ToUpper(),
                timeframe,
                timestamp = DateTime.UtcNow,
                message = $"Analysis for {symbol.ToUpper()} completed successfully"
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                success = false,
                error = "Invalid symbol or timeframe",
                details = ex.Message,
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                error = "Internal server error during symbol analysis",
                details = ex.Message,
                timestamp = DateTime.UtcNow
            });
        }
    }

    /// <summary>
    /// Gets available cryptocurrencies for analysis
    /// </summary>
    /// <returns>List of available cryptocurrency symbols</returns>
    [HttpGet("symbols")]
    public async Task<IActionResult> GetAvailableSymbolsAsync()
    {
        try
        {
            // This would typically get symbols from the cryptocurrency repository
            // For now, return common crypto symbols
            var symbols = new List<object>
            {
                new { symbol = "BTCUSDT", name = "Bitcoin", category = "major" },
                new { symbol = "ETHUSDT", name = "Ethereum", category = "major" },
                new { symbol = "BNBUSDT", name = "Binance Coin", category = "major" },
                new { symbol = "ADAUSDT", name = "Cardano", category = "altcoin" },
                new { symbol = "SOLUSDT", name = "Solana", category = "altcoin" },
                new { symbol = "XRPUSDT", name = "Ripple", category = "altcoin" },
                new { symbol = "DOTUSDT", name = "Polkadot", category = "altcoin" },
                new { symbol = "AVAXUSDT", name = "Avalanche", category = "altcoin" }
            };

            return Ok(new
            {
                success = true,
                data = symbols,
                count = symbols.Count,
                timestamp = DateTime.UtcNow,
                message = "Available symbols retrieved successfully"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                error = "Internal server error while retrieving symbols",
                details = ex.Message,
                timestamp = DateTime.UtcNow
            });
        }
    }

    /// <summary>
    /// Gets supported timeframes for analysis
    /// </summary>
    /// <returns>List of supported timeframes</returns>
    [HttpGet("timeframes")]
    public IActionResult GetSupportedTimeframes()
    {
        try
        {
            var timeframes = new List<object>
            {
                new { value = "1m", name = "1 Minute", category = "short_term" },
                new { value = "5m", name = "5 Minutes", category = "short_term" },
                new { value = "1h", name = "1 Hour", category = "medium_term" },
                new { value = "4h", name = "4 Hours", category = "medium_term" },
                new { value = "1d", name = "1 Day", category = "long_term" }
            };

            return Ok(new
            {
                success = true,
                data = timeframes,
                count = timeframes.Count,
                timestamp = DateTime.UtcNow,
                message = "Supported timeframes retrieved successfully"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                error = "Internal server error while retrieving timeframes",
                details = ex.Message,
                timestamp = DateTime.UtcNow
            });
        }
    }

    /// <summary>
    /// Gets available analysis conditions and indicators
    /// </summary>
    /// <returns>List of supported analysis conditions</returns>
    [HttpGet("conditions")]
    public IActionResult GetAvailableConditions()
    {
        try
        {
            var conditions = new
            {
                types = new[]
                {
                    new { value = "breakout", name = "Breakout Analysis", description = "Detects price breakouts from key levels" },
                    new { value = "pattern", name = "Pattern Recognition", description = "Identifies chart patterns and formations" },
                    new { value = "volume", name = "Volume Analysis", description = "Analyzes volume-based signals" },
                    new { value = "price_action", name = "Price Action", description = "Pure price movement analysis" }
                },
                indicators = new[]
                {
                    new { value = "structure_break", name = "Structure Break", type = "breakout", description = "Market structure breakout detection" },
                    new { value = "mss_break", name = "Market Structure Shift", type = "breakout", description = "MSS breakout with confirmation" },
                    new { value = "fvg_entry", name = "FVG Entry", type = "pattern", description = "Fair Value Gap entry signal" },
                    new { value = "fvg_retest", name = "FVG Retest", type = "pattern", description = "Fair Value Gap retest opportunity" },
                    new { value = "support_level", name = "Support Level", type = "pattern", description = "Support level formation" },
                    new { value = "resistance_level", name = "Resistance Level", type = "pattern", description = "Resistance level formation" }
                },
                confirmations = new[]
                {
                    new { value = "structure_break", name = "Structure Break Confirmation", required = true },
                    new { value = "volume", name = "Volume Confirmation", required = false },
                    new { value = "price_action", name = "Price Action Confirmation", required = true },
                    new { value = "mss_confirmed", name = "MSS Confirmation", required = true }
                }
            };

            return Ok(new
            {
                success = true,
                data = conditions,
                timestamp = DateTime.UtcNow,
                message = "Available conditions retrieved successfully"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                error = "Internal server error while retrieving conditions",
                details = ex.Message,
                timestamp = DateTime.UtcNow
            });
        }
    }
}