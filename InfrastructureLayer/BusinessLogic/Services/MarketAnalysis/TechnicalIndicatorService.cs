using ApplicationLayer.Interfaces;
using ApplicationLayer.Interfaces.Services;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;

namespace InfrastructureLayer.BusinessLogic.Services.MarketAnalysis;

[InjectAsScoped]
public class TechnicalIndicatorService(
    IUnitOfWork unitOfWork,
    IRepository<Candle_1m> candles1m,
    IRepository<Candle_5m> candles5m,
    IRepository<Candle_1h> candles1h,
    IRepository<Candle_4h> candles4h,
    IRepository<Candle_1d> candles1d
) : ITechnicalIndicatorService
{
    public async Task<List<FairValueGap>> DetectFairValueGapsAsync(string symbol, string timeframe, CancellationToken cancellationToken = default)
    {
        var candles = await GetCandlesAsync(symbol, timeframe, 100, cancellationToken);
        var fvgs = new List<FairValueGap>();

        for (int i = 2; i < candles.Count; i++)
        {
            var prevCandle = candles[i - 2];
            var currentCandle = candles[i - 1];
            var nextCandle = candles[i];

            // Bullish FVG: Previous candle low > Next candle high
            if (prevCandle.Low > nextCandle.High)
            {
                fvgs.Add(new FairValueGap
                {
                    Symbol = symbol,
                    Timeframe = timeframe,
                    HighPrice = prevCandle.Low,
                    LowPrice = nextCandle.High,
                    MidPrice = (prevCandle.Low + nextCandle.High) / 2,
                    Direction = "bullish",
                    CandleIndex = i - 1,
                    DetectedAt = DateTime.UtcNow,
                    IsActive = true,
                    Type = "FVG"
                });
            }
            // Bearish FVG: Previous candle high < Next candle low
            else if (prevCandle.High < nextCandle.Low)
            {
                fvgs.Add(new FairValueGap
                {
                    Symbol = symbol,
                    Timeframe = timeframe,
                    HighPrice = nextCandle.Low,
                    LowPrice = prevCandle.High,
                    MidPrice = (nextCandle.Low + prevCandle.High) / 2,
                    Direction = "bearish",
                    CandleIndex = i - 1,
                    DetectedAt = DateTime.UtcNow,
                    IsActive = true,
                    Type = "FVG"
                });
            }
        }

        return fvgs;
    }

    public async Task<List<MarketStructureShift>> DetectMarketStructureShiftsAsync(string symbol, string timeframe, CancellationToken cancellationToken = default)
    {
        var candles = await GetCandlesAsync(symbol, timeframe, 200, cancellationToken);
        var mssShifts = new List<MarketStructureShift>();

        // Find swing highs and lows
        var swingPoints = FindSwingPoints(candles);

        for (int i = 1; i < swingPoints.Count; i++)
        {
            var prevPoint = swingPoints[i - 1];
            var currentPoint = swingPoints[i];

            // Bullish MSS: Break of previous swing high
            if (prevPoint.Type == "high" && currentPoint.Price > prevPoint.Price)
            {
                mssShifts.Add(new MarketStructureShift
                {
                    Symbol = symbol,
                    Timeframe = timeframe,
                    Direction = "bullish",
                    BreakPrice = currentPoint.Price,
                    PreviousStructurePrice = prevPoint.Price,
                    StructureLevel = DetermineStructureLevel(prevPoint.Price, currentPoint.Price),
                    BreakStrength = DetermineBreakStrength(prevPoint.Price, currentPoint.Price),
                    IsConfirmed = true,
                    DetectedAt = DateTime.UtcNow,
                    Type = "MSS"
                });
            }
            // Bearish MSS: Break of previous swing low
            else if (prevPoint.Type == "low" && currentPoint.Price < prevPoint.Price)
            {
                mssShifts.Add(new MarketStructureShift
                {
                    Symbol = symbol,
                    Timeframe = timeframe,
                    Direction = "bearish",
                    BreakPrice = currentPoint.Price,
                    PreviousStructurePrice = prevPoint.Price,
                    StructureLevel = DetermineStructureLevel(prevPoint.Price, currentPoint.Price),
                    BreakStrength = DetermineBreakStrength(prevPoint.Price, currentPoint.Price),
                    IsConfirmed = true,
                    DetectedAt = DateTime.UtcNow,
                    Type = "MSS"
                });
            }
        }

        return mssShifts;
    }

    public async Task<List<SupportResistanceLevel>> DetectSupportResistanceLevelsAsync(string symbol, string timeframe, CancellationToken cancellationToken = default)
    {
        var candles = await GetCandlesAsync(symbol, timeframe, 500, cancellationToken);
        var levels = new List<SupportResistanceLevel>();

        // Find significant price levels
        var swingPoints = FindSwingPoints(candles);
        var priceGroups = GroupNearbyPrices(swingPoints, 0.01m); // 1% tolerance

        foreach (var group in priceGroups.Where(g => g.Count >= 2))
        {
            var avgPrice = group.Average(p => p.Price);
            var touchCount = group.Count;
            var touchDates = group.Select(p => p.DateTime).ToList();

            var levelType = group.First().Type == "high" ? "resistance" : "support";
            var strength = touchCount >= 3 ? "strong" : touchCount == 2 ? "medium" : "weak";

            levels.Add(new SupportResistanceLevel
            {
                Symbol = symbol,
                Timeframe = timeframe,
                Price = avgPrice,
                LevelType = levelType,
                TouchCount = touchCount,
                Strength = strength,
                TouchDates = touchDates,
                DetectedAt = DateTime.UtcNow,
                Type = levelType.ToUpper()
            });
        }

        return levels;
    }

    public async Task<List<CandlestickPattern>> DetectCandlestickPatternsAsync(string symbol, string timeframe, CancellationToken cancellationToken = default)
    {
        var candles = await GetCandlesAsync(symbol, timeframe, 50, cancellationToken);
        var patterns = new List<CandlestickPattern>();

        for (int i = 1; i < candles.Count; i++)
        {
            var currentCandle = candles[i];
            var prevCandle = candles[i - 1];

            // Hammer pattern
            if (IsHammer(currentCandle))
            {
                patterns.Add(new CandlestickPattern
                {
                    Symbol = symbol,
                    Timeframe = timeframe,
                    PatternName = "Hammer",
                    Sentiment = "bullish",
                    Reliability = 70,
                    CandleCount = 1,
                    CandleIndexes = new List<int> { i },
                    DetectedAt = DateTime.UtcNow,
                    Type = "CANDLESTICK_PATTERN"
                });
            }

            // Doji pattern
            if (IsDoji(currentCandle))
            {
                patterns.Add(new CandlestickPattern
                {
                    Symbol = symbol,
                    Timeframe = timeframe,
                    PatternName = "Doji",
                    Sentiment = "neutral",
                    Reliability = 60,
                    CandleCount = 1,
                    CandleIndexes = new List<int> { i },
                    DetectedAt = DateTime.UtcNow,
                    Type = "CANDLESTICK_PATTERN"
                });
            }

            // Bullish Engulfing
            if (IsBullishEngulfing(prevCandle, currentCandle))
            {
                patterns.Add(new CandlestickPattern
                {
                    Symbol = symbol,
                    Timeframe = timeframe,
                    PatternName = "Bullish Engulfing",
                    Sentiment = "bullish",
                    Reliability = 80,
                    CandleCount = 2,
                    CandleIndexes = new List<int> { i - 1, i },
                    DetectedAt = DateTime.UtcNow,
                    Type = "CANDLESTICK_PATTERN"
                });
            }
        }

        return patterns;
    }

    public async Task<bool> ValidateBreakoutAsync(string symbol, string timeframe, decimal breakPrice, string direction, CancellationToken cancellationToken = default)
    {
        var candles = await GetCandlesAsync(symbol, timeframe, 20, cancellationToken);
        if (!candles.Any()) return false;

        var latestCandle = candles.Last();

        if (direction.ToLower() == "up")
        {
            return latestCandle.Close > breakPrice && latestCandle.High > breakPrice;
        }
        else if (direction.ToLower() == "down")
        {
            return latestCandle.Close < breakPrice && latestCandle.Low < breakPrice;
        }

        return false;
    }

    public async Task<bool> IsPriceInFVGZoneAsync(string symbol, decimal currentPrice, CancellationToken cancellationToken = default)
    {
        var fvgs = await DetectFairValueGapsAsync(symbol, "1h", cancellationToken);
        
        return fvgs.Any(fvg => 
            fvg.IsActive && 
            currentPrice >= fvg.LowPrice && 
            currentPrice <= fvg.HighPrice);
    }

    private async Task<List<CandleBase>> GetCandlesAsync(string symbol, string timeframe, int count, CancellationToken cancellationToken)
    {
        return await Task.Run(() =>
        {
            return timeframe.ToLower() switch
            {
                "1m" => candles1m.GetAllIncludeNavigation(c => c.Cryptocurrency)
                    .Where(c => c.Cryptocurrency.Symbol == symbol)
                    .OrderByDescending(c => c.OpenTime)
                    .Take(count)
                    .Cast<CandleBase>()
                    .ToList(),
                "5m" => candles5m.GetAllIncludeNavigation(c => c.Cryptocurrency)
                    .Where(c => c.Cryptocurrency.Symbol == symbol)
                    .OrderByDescending(c => c.OpenTime)
                    .Take(count)
                    .Cast<CandleBase>()
                    .ToList(),
                "1h" => candles1h.GetAllIncludeNavigation(c => c.Cryptocurrency)
                    .Where(c => c.Cryptocurrency.Symbol == symbol)
                    .OrderByDescending(c => c.OpenTime)
                    .Take(count)
                    .Cast<CandleBase>()
                    .ToList(),
                "4h" => candles4h.GetAllIncludeNavigation(c => c.Cryptocurrency)
                    .Where(c => c.Cryptocurrency.Symbol == symbol)
                    .OrderByDescending(c => c.OpenTime)
                    .Take(count)
                    .Cast<CandleBase>()
                    .ToList(),
                "1d" => candles1d.GetAllIncludeNavigation(c => c.Cryptocurrency)
                    .Where(c => c.Cryptocurrency.Symbol == symbol)
                    .OrderByDescending(c => c.OpenTime)
                    .Take(count)
                    .Cast<CandleBase>()
                    .ToList(),
                _ => new List<CandleBase>()
            };
        });
    }

    private List<SwingPoint> FindSwingPoints(List<CandleBase> candles)
    {
        var swingPoints = new List<SwingPoint>();
        const int lookback = 5;

        for (int i = lookback; i < candles.Count - lookback; i++)
        {
            var current = candles[i];
            bool isSwingHigh = true;
            bool isSwingLow = true;

            // Check if current candle is a swing high
            for (int j = i - lookback; j <= i + lookback; j++)
            {
                if (j != i && candles[j].High >= current.High)
                {
                    isSwingHigh = false;
                    break;
                }
            }

            // Check if current candle is a swing low
            for (int j = i - lookback; j <= i + lookback; j++)
            {
                if (j != i && candles[j].Low <= current.Low)
                {
                    isSwingLow = false;
                    break;
                }
            }

            if (isSwingHigh)
            {
                swingPoints.Add(new SwingPoint
                {
                    Price = current.High,
                    DateTime = current.OpenTime,
                    Type = "high"
                });
            }

            if (isSwingLow)
            {
                swingPoints.Add(new SwingPoint
                {
                    Price = current.Low,
                    DateTime = current.OpenTime,
                    Type = "low"
                });
            }
        }

        return swingPoints;
    }

    private List<List<SwingPoint>> GroupNearbyPrices(List<SwingPoint> points, decimal tolerance)
    {
        var groups = new List<List<SwingPoint>>();
        var processed = new HashSet<SwingPoint>();

        foreach (var point in points)
        {
            if (processed.Contains(point)) continue;

            var group = new List<SwingPoint> { point };
            processed.Add(point);

            foreach (var other in points)
            {
                if (processed.Contains(other)) continue;

                var priceDiff = Math.Abs(point.Price - other.Price) / point.Price;
                if (priceDiff <= tolerance && point.Type == other.Type)
                {
                    group.Add(other);
                    processed.Add(other);
                }
            }

            groups.Add(group);
        }

        return groups;
    }

    private string DetermineStructureLevel(decimal prevPrice, decimal currentPrice)
    {
        var percentChange = Math.Abs((currentPrice - prevPrice) / prevPrice) * 100;
        
        return percentChange switch
        {
            >= 5 => "major",
            >= 2 => "key",
            _ => "minor"
        };
    }

    private string DetermineBreakStrength(decimal prevPrice, decimal currentPrice)
    {
        var percentChange = Math.Abs((currentPrice - prevPrice) / prevPrice) * 100;
        
        return percentChange switch
        {
            >= 3 => "strong",
            >= 1 => "medium",
            _ => "weak"
        };
    }

    private bool IsHammer(CandleBase candle)
    {
        var bodySize = Math.Abs(candle.Close - candle.Open);
        var lowerShadow = Math.Min(candle.Open, candle.Close) - candle.Low;
        var upperShadow = candle.High - Math.Max(candle.Open, candle.Close);

        return lowerShadow >= 2 * bodySize && upperShadow <= bodySize * 0.1m;
    }

    private bool IsDoji(CandleBase candle)
    {
        var bodySize = Math.Abs(candle.Close - candle.Open);
        var totalRange = candle.High - candle.Low;

        return bodySize <= totalRange * 0.1m;
    }

    private bool IsBullishEngulfing(CandleBase prev, CandleBase current)
    {
        return prev.Close < prev.Open && // Previous candle is bearish
               current.Close > current.Open && // Current candle is bullish
               current.Open < prev.Close && // Current opens below previous close
               current.Close > prev.Open; // Current closes above previous open
    }

    private class SwingPoint
    {
        public decimal Price { get; set; }
        public DateTime DateTime { get; set; }
        public string Type { get; set; } = string.Empty; // "high" or "low"
    }
}