using ApplicationLayer.Common.Extensions;
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
    [HttpPost("analyze")]
    public async Task<IActionResult> AnalyzeMarketAsync(AnalyzeMarketCommand model)
        => await ResultHelper.GetResultAsync(mediator, model);

    [HttpGet("signals")]
    public async Task<IActionResult> GetActiveSignalsAsync(
        [FromQuery] string symbol = null,
        [FromQuery] string timeframe = null,
        [FromQuery] string signalType = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
        => await ResultHelper.GetResultAsync(mediator, new GetActiveSignalsQuery(symbol, timeframe, signalType, pageNumber, pageSize));

    [HttpGet("analyze/{symbol}")]
    public async Task<IActionResult> GetSymbolAnalysisAsync([FromRoute] string symbol, [FromQuery] string timeframe = "1h")
    {
        var request = new MarketAnalysisRequestDto
        {
            Market = "crypto",
            Symbols = [symbol.ToUpper()],
            Timeframes = [timeframe],
            Conditions = [],
            Filters = new AnalysisFiltersDto
            {
                VolumeMin = "medium",
                Volatility = "medium",
                Liquidity = "high"
            },
            Preferences = new AnalysisPreferencesDto
            {
                RiskLevel = "medium",
                StrategyType = "price_action",
                SignalStrength = "medium"
            }
        };
        return await ResultHelper.GetResultAsync(mediator, request);
    }

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