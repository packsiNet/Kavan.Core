# Hamyar Trade - Market Analysis API Documentation

## Overview
This API provides comprehensive cryptocurrency market analysis with advanced technical indicators, pattern recognition, and signal generation capabilities.

## Base URL
```
http://localhost:5000/api/MarketAnalysis
```

## Endpoints

### 1. Analyze Market
**POST** `/analyze`

Performs comprehensive market analysis based on provided conditions and filters.

#### Request Body
```json
{
    "market": "crypto",
    "symbols": [],  // Empty array = analyze all symbols
    "timeframes": ["1h", "4h"],
    "conditions": [
        {
            "type": "breakout",
            "indicator": "structure_break",
            "description": "Initial ceiling break with FVG formation",
            "parameters": {
                "break_direction": "up",
                "structure_level": "major",
                "break_strength": "strong",
                "fvg_zone": true,
                "zone_type": "fvg"
            },
            "timeframe": "1h",
            "logical_operator": "AND",
            "confirmation": [
                {
                    "type": "structure_break",
                    "condition": "ceiling_break_with_fvg",
                    "required": true
                }
            ]
        }
    ],
    "filters": {
        "volume_min": "medium",
        "volatility": "medium",
        "liquidity": "high"
    },
    "preferences": {
        "risk_level": "medium",
        "strategy_type": "price_action",
        "signal_strength": "strong"
    }
}
```

#### Response
```json
{
    "success": true,
    "data": {
        "signals": [
            {
                "symbol": "BTCUSDT",
                "timeframe": "1h",
                "signalType": "BUY",
                "price": 50000.00,
                "confidence": 85.5,
                "strength": "strong",
                "riskLevel": "medium",
                "reasons": [
                    {
                        "conditionType": "breakout",
                        "indicator": "structure_break",
                        "description": "Initial ceiling break with FVG formation",
                        "isPrimary": true,
                        "weight": 30
                    }
                ],
                "confirmedIndicators": [
                    {
                        "name": "Market Structure Shift",
                        "type": "MSS",
                        "status": "confirmed",
                        "isRequired": true,
                        "values": {
                            "direction": "up",
                            "strength": "strong"
                        }
                    }
                ],
                "targets": {
                    "entryPrice": 50000.00,
                    "stopLoss": 49000.00,
                    "takeProfits": [51000.00, 52000.00, 53000.00],
                    "riskRewardRatio": 3.0
                },
                "generatedAt": "2024-01-15T10:30:00Z",
                "isActive": true
            }
        ],
        "summary": {
            "totalSignals": 1,
            "buySignals": 1,
            "sellSignals": 0,
            "averageConfidence": 85.5,
            "analysisTimeframe": "1h-4h",
            "marketCondition": "bullish"
        }
    },
    "timestamp": "2024-01-15T10:30:00Z",
    "message": "Market analysis completed successfully"
}
```

### 2. Get Active Signals
**GET** `/signals`

Retrieves currently active trading signals with optional filtering.

#### Query Parameters
- `symbol` (optional): Filter by cryptocurrency symbol (e.g., BTCUSDT)
- `timeframe` (optional): Filter by timeframe (1m, 5m, 1h, 4h, 1d)
- `signalType` (optional): Filter by signal type (BUY, SELL, HOLD)
- `pageNumber` (optional): Page number for pagination (default: 1)
- `pageSize` (optional): Page size for pagination (default: 20)

#### Example Request
```
GET /signals?symbol=BTCUSDT&timeframe=1h&signalType=BUY&pageNumber=1&pageSize=10
```

### 3. Get Symbol Analysis
**GET** `/analyze/{symbol}`

Gets detailed market analysis for a specific cryptocurrency symbol.

#### Parameters
- `symbol` (path): Cryptocurrency symbol (e.g., BTCUSDT)
- `timeframe` (query, optional): Analysis timeframe (default: 1h)

#### Example Request
```
GET /analyze/BTCUSDT?timeframe=4h
```

### 4. Get Available Symbols
**GET** `/symbols`

Returns list of available cryptocurrency symbols for analysis.

#### Response
```json
{
    "success": true,
    "data": [
        {
            "symbol": "BTCUSDT",
            "name": "Bitcoin",
            "category": "major"
        },
        {
            "symbol": "ETHUSDT",
            "name": "Ethereum",
            "category": "major"
        }
    ],
    "count": 8,
    "timestamp": "2024-01-15T10:30:00Z",
    "message": "Available symbols retrieved successfully"
}
```

### 5. Get Supported Timeframes
**GET** `/timeframes`

Returns list of supported timeframes for analysis.

### 6. Get Available Conditions
**GET** `/conditions`

Returns available analysis conditions, indicators, and confirmation types.

## Condition Types

### Breakout Conditions
- `structure_break`: Market structure breakout detection
- `mss_break`: Market Structure Shift with confirmation

### Pattern Conditions
- `fvg_entry`: Fair Value Gap entry signal
- `fvg_retest`: Fair Value Gap retest opportunity
- `support_level`: Support level formation
- `resistance_level`: Resistance level formation

### Confirmation Types
- `structure_break`: Structure break confirmation
- `volume`: Volume-based confirmation
- `price_action`: Price action confirmation
- `mss_confirmed`: MSS confirmation

## Technical Indicators

### Fair Value Gap (FVG)
Detects price gaps that need to be filled, providing entry and retest opportunities.

### Market Structure Shift (MSS)
Identifies significant changes in market structure indicating trend reversals.

### Support/Resistance Levels
Detects key price levels where price has historically found support or resistance.

### Candlestick Patterns
- Hammer
- Doji
- Bullish/Bearish Engulfing

## Error Handling

All endpoints return standardized error responses:

```json
{
    "success": false,
    "error": "Error description",
    "details": "Detailed error message",
    "timestamp": "2024-01-15T10:30:00Z"
}
```

## HTTP Status Codes
- `200`: Success
- `400`: Bad Request (invalid parameters)
- `500`: Internal Server Error

## Usage Examples

### cURL Example
```bash
curl -X POST "http://localhost:5000/api/MarketAnalysis/analyze" \
  -H "Content-Type: application/json" \
  -d @sample-request.json
```

### JavaScript Example
```javascript
const response = await fetch('http://localhost:5000/api/MarketAnalysis/analyze', {
    method: 'POST',
    headers: {
        'Content-Type': 'application/json'
    },
    body: JSON.stringify({
        market: "crypto",
        symbols: ["BTCUSDT", "ETHUSDT"],
        timeframes: ["1h"],
        conditions: [
            {
                type: "breakout",
                indicator: "structure_break",
                description: "Structure break analysis",
                timeframe: "1h",
                logical_operator: "AND"
            }
        ],
        filters: {
            volume_min: "medium",
            volatility: "medium",
            liquidity: "high"
        },
        preferences: {
            risk_level: "medium",
            strategy_type: "price_action",
            signal_strength: "strong"
        }
    })
});

const data = await response.json();
console.log(data);
```

## Notes
- Empty `symbols` array will analyze all available cryptocurrencies
- Multiple timeframes can be specified for multi-timeframe analysis
- Conditions are evaluated using logical operators (AND, OR, NOT)
- Signal confidence is calculated based on matched conditions and confirmations
- All timestamps are in UTC format