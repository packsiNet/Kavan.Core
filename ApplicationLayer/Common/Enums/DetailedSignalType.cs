namespace ApplicationLayer.Common.Enums
{
    // SmartEnum-style representation for detailed signal types
    public sealed class DetailedSignalType : IEquatable<DetailedSignalType>
    {
        public string Name { get; }

        public int Value { get; }

        private DetailedSignalType(string name, int value)
        {
            Name = name;
            Value = value;
        }

        // Ichimoku Cloud Signals
        public static readonly DetailedSignalType IchimokuCloudBreakoutUp = new(nameof(IchimokuCloudBreakoutUp), 1001);

        public static readonly DetailedSignalType IchimokuCloudBreakoutDown = new(nameof(IchimokuCloudBreakoutDown), 1002);
        public static readonly DetailedSignalType IchimokuTenkanKijunCrossUp = new(nameof(IchimokuTenkanKijunCrossUp), 1003);
        public static readonly DetailedSignalType IchimokuTenkanKijunCrossDown = new(nameof(IchimokuTenkanKijunCrossDown), 1004);
        public static readonly DetailedSignalType IchimokuChikouSpanConfirmation = new(nameof(IchimokuChikouSpanConfirmation), 1005);
        public static readonly DetailedSignalType IchimokuFutureCloudGreen = new(nameof(IchimokuFutureCloudGreen), 1006);
        public static readonly DetailedSignalType IchimokuFutureCloudRed = new(nameof(IchimokuFutureCloudRed), 1007);

        // Bollinger Bands Signals
        public static readonly DetailedSignalType BollingerBandsSqueeze = new(nameof(BollingerBandsSqueeze), 2001);

        public static readonly DetailedSignalType BollingerBandsExpansion = new(nameof(BollingerBandsExpansion), 2002);
        public static readonly DetailedSignalType BollingerUpperBandTouch = new(nameof(BollingerUpperBandTouch), 2003);
        public static readonly DetailedSignalType BollingerLowerBandTouch = new(nameof(BollingerLowerBandTouch), 2004);
        public static readonly DetailedSignalType BollingerMiddleBandCrossUp = new(nameof(BollingerMiddleBandCrossUp), 2005);
        public static readonly DetailedSignalType BollingerMiddleBandCrossDown = new(nameof(BollingerMiddleBandCrossDown), 2006);
        public static readonly DetailedSignalType BollingerBandBreakoutUp = new(nameof(BollingerBandBreakoutUp), 2007);
        public static readonly DetailedSignalType BollingerBandBreakoutDown = new(nameof(BollingerBandBreakoutDown), 2008);

        // RSI Signals
        public static readonly DetailedSignalType RSIOverBought = new(nameof(RSIOverBought), 3001);

        public static readonly DetailedSignalType RSIOverSold = new(nameof(RSIOverSold), 3002);
        public static readonly DetailedSignalType RSIDivergenceBullish = new(nameof(RSIDivergenceBullish), 3003);
        public static readonly DetailedSignalType RSIDivergenceBearish = new(nameof(RSIDivergenceBearish), 3004);
        public static readonly DetailedSignalType RSICenterLineCrossUp = new(nameof(RSICenterLineCrossUp), 3005);
        public static readonly DetailedSignalType RSICenterLineCrossDown = new(nameof(RSICenterLineCrossDown), 3006);

        // MACD Signals
        public static readonly DetailedSignalType MACDSignalLineCrossUp = new(nameof(MACDSignalLineCrossUp), 4001);

        public static readonly DetailedSignalType MACDSignalLineCrossDown = new(nameof(MACDSignalLineCrossDown), 4002);
        public static readonly DetailedSignalType MACDZeroLineCrossUp = new(nameof(MACDZeroLineCrossUp), 4003);
        public static readonly DetailedSignalType MACDZeroLineCrossDown = new(nameof(MACDZeroLineCrossDown), 4004);
        public static readonly DetailedSignalType MACDHistogramIncreasing = new(nameof(MACDHistogramIncreasing), 4005);
        public static readonly DetailedSignalType MACDHistogramDecreasing = new(nameof(MACDHistogramDecreasing), 4006);
        public static readonly DetailedSignalType MACDDivergenceBullish = new(nameof(MACDDivergenceBullish), 4007);
        public static readonly DetailedSignalType MACDDivergenceBearish = new(nameof(MACDDivergenceBearish), 4008);

        // Moving Average Signals
        public static readonly DetailedSignalType SMAGoldenCross = new(nameof(SMAGoldenCross), 5001);

        public static readonly DetailedSignalType SMADeathCross = new(nameof(SMADeathCross), 5002);
        public static readonly DetailedSignalType EMACrossUp = new(nameof(EMACrossUp), 5003);
        public static readonly DetailedSignalType EMACrossDown = new(nameof(EMACrossDown), 5004);
        public static readonly DetailedSignalType MASlope = new(nameof(MASlope), 5005);
        public static readonly DetailedSignalType MASupport = new(nameof(MASupport), 5006);
        public static readonly DetailedSignalType MAResistance = new(nameof(MAResistance), 5007);

        // Stochastic Signals
        public static readonly DetailedSignalType StochasticOverBought = new(nameof(StochasticOverBought), 6001);

        public static readonly DetailedSignalType StochasticOverSold = new(nameof(StochasticOverSold), 6002);
        public static readonly DetailedSignalType StochasticKDCrossUp = new(nameof(StochasticKDCrossUp), 6003);
        public static readonly DetailedSignalType StochasticKDCrossDown = new(nameof(StochasticKDCrossDown), 6004);
        public static readonly DetailedSignalType StochasticDivergenceBullish = new(nameof(StochasticDivergenceBullish), 6005);
        public static readonly DetailedSignalType StochasticDivergenceBearish = new(nameof(StochasticDivergenceBearish), 6006);

        // ADX Signals
        public static readonly DetailedSignalType ADXTrendStrengthHigh = new(nameof(ADXTrendStrengthHigh), 7001);

        public static readonly DetailedSignalType ADXTrendStrengthLow = new(nameof(ADXTrendStrengthLow), 7002);
        public static readonly DetailedSignalType ADXRising = new(nameof(ADXRising), 7003);
        public static readonly DetailedSignalType ADXFalling = new(nameof(ADXFalling), 7004);
        public static readonly DetailedSignalType ADXDICrossoverBullish = new(nameof(ADXDICrossoverBullish), 7005);
        public static readonly DetailedSignalType ADXDICrossoverBearish = new(nameof(ADXDICrossoverBearish), 7006);

        // CCI Signals
        public static readonly DetailedSignalType CCIOverBought = new(nameof(CCIOverBought), 8001);

        public static readonly DetailedSignalType CCIOverSold = new(nameof(CCIOverSold), 8002);
        public static readonly DetailedSignalType CCIZeroCrossUp = new(nameof(CCIZeroCrossUp), 8003);
        public static readonly DetailedSignalType CCIZeroCrossDown = new(nameof(CCIZeroCrossDown), 8004);
        public static readonly DetailedSignalType CCIDivergenceBullish = new(nameof(CCIDivergenceBullish), 8005);
        public static readonly DetailedSignalType CCIDivergenceBearish = new(nameof(CCIDivergenceBearish), 8006);

        // Williams %R Signals
        public static readonly DetailedSignalType WilliamsROverBought = new(nameof(WilliamsROverBought), 9001);

        public static readonly DetailedSignalType WilliamsROverSold = new(nameof(WilliamsROverSold), 9002);
        public static readonly DetailedSignalType WilliamsRCrossUp = new(nameof(WilliamsRCrossUp), 9003);
        public static readonly DetailedSignalType WilliamsRCrossDown = new(nameof(WilliamsRCrossDown), 9004);

        // Volume Signals
        public static readonly DetailedSignalType VolumeSpike = new(nameof(VolumeSpike), 10001);

        public static readonly DetailedSignalType VolumeBreakout = new(nameof(VolumeBreakout), 10002);
        public static readonly DetailedSignalType VolumeConfirmation = new(nameof(VolumeConfirmation), 10003);
        public static readonly DetailedSignalType VolumeDivergence = new(nameof(VolumeDivergence), 10004);

        // Support/Resistance Signals
        public static readonly DetailedSignalType SupportBreakdown = new(nameof(SupportBreakdown), 11001);

        public static readonly DetailedSignalType ResistanceBreakout = new(nameof(ResistanceBreakout), 11002);
        public static readonly DetailedSignalType SupportBounce = new(nameof(SupportBounce), 11003);
        public static readonly DetailedSignalType ResistanceRejection = new(nameof(ResistanceRejection), 11004);

        // Pattern Signals
        public static readonly DetailedSignalType DoubleTop = new(nameof(DoubleTop), 12001);

        public static readonly DetailedSignalType DoubleBottom = new(nameof(DoubleBottom), 12002);
        public static readonly DetailedSignalType HeadAndShoulders = new(nameof(HeadAndShoulders), 12003);
        public static readonly DetailedSignalType InverseHeadAndShoulders = new(nameof(InverseHeadAndShoulders), 12004);
        public static readonly DetailedSignalType Triangle = new(nameof(Triangle), 12005);
        public static readonly DetailedSignalType Flag = new(nameof(Flag), 12006);
        public static readonly DetailedSignalType Pennant = new(nameof(Pennant), 12007);
        public static readonly DetailedSignalType Wedge = new(nameof(Wedge), 12008);

        // Fibonacci Signals
        public static readonly DetailedSignalType FibonacciRetracement382 = new(nameof(FibonacciRetracement382), 13001);

        public static readonly DetailedSignalType FibonacciRetracement500 = new(nameof(FibonacciRetracement500), 13002);
        public static readonly DetailedSignalType FibonacciRetracement618 = new(nameof(FibonacciRetracement618), 13003);
        public static readonly DetailedSignalType FibonacciExtension161 = new(nameof(FibonacciExtension161), 13004);
        public static readonly DetailedSignalType FibonacciExtension261 = new(nameof(FibonacciExtension261), 13005);

        // Pivot Point Signals
        public static readonly DetailedSignalType PivotPointBreakout = new(nameof(PivotPointBreakout), 14001);

        public static readonly DetailedSignalType PivotPointSupport = new(nameof(PivotPointSupport), 14002);
        public static readonly DetailedSignalType PivotPointResistance = new(nameof(PivotPointResistance), 14003);

        // Additional Ichimoku Signals
        public static readonly DetailedSignalType IchimokuTenkanSenBullishCross = new(nameof(IchimokuTenkanSenBullishCross), 1008);

        public static readonly DetailedSignalType IchimokuTenkanSenBearishCross = new(nameof(IchimokuTenkanSenBearishCross), 1009);
        public static readonly DetailedSignalType IchimokuKijunSenBullishCross = new(nameof(IchimokuKijunSenBullishCross), 1010);
        public static readonly DetailedSignalType IchimokuKijunSenBearishCross = new(nameof(IchimokuKijunSenBearishCross), 1011);
        public static readonly DetailedSignalType IchimokuTenkanKijunBullishCross = new(nameof(IchimokuTenkanKijunBullishCross), 1012);
        public static readonly DetailedSignalType IchimokuTenkanKijunBearishCross = new(nameof(IchimokuTenkanKijunBearishCross), 1013);
        public static readonly DetailedSignalType IchimokuKumoBullishBreakout = new(nameof(IchimokuKumoBullishBreakout), 1014);
        public static readonly DetailedSignalType IchimokuKumoBearishBreakdown = new(nameof(IchimokuKumoBearishBreakdown), 1015);
        public static readonly DetailedSignalType IchimokuChikouSpanBullishConfirmation = new(nameof(IchimokuChikouSpanBullishConfirmation), 1016);
        public static readonly DetailedSignalType IchimokuChikouSpanBearishConfirmation = new(nameof(IchimokuChikouSpanBearishConfirmation), 1017);
        public static readonly DetailedSignalType IchimokuBullishCloudFormation = new(nameof(IchimokuBullishCloudFormation), 1018);
        public static readonly DetailedSignalType IchimokuBearishCloudFormation = new(nameof(IchimokuBearishCloudFormation), 1019);
        public static readonly DetailedSignalType IchimokuFlatCloudFormation = new(nameof(IchimokuFlatCloudFormation), 1020);

        // Additional Moving Average Signals
        public static readonly DetailedSignalType MovingAverageGoldenCross = new(nameof(MovingAverageGoldenCross), 5008);

        public static readonly DetailedSignalType MovingAverageDeathCross = new(nameof(MovingAverageDeathCross), 5009);
        public static readonly DetailedSignalType MovingAverageSupportBounce = new(nameof(MovingAverageSupportBounce), 5010);
        public static readonly DetailedSignalType MovingAverageResistanceRejection = new(nameof(MovingAverageResistanceRejection), 5011);
        public static readonly DetailedSignalType MovingAverageBullishBreakout = new(nameof(MovingAverageBullishBreakout), 5012);
        public static readonly DetailedSignalType MovingAverageBearishBreakdown = new(nameof(MovingAverageBearishBreakdown), 5013);

        // EMA Signals
        public static readonly DetailedSignalType EMABullishCross = new(nameof(EMABullishCross), 5014);

        public static readonly DetailedSignalType EMABearishCross = new(nameof(EMABearishCross), 5015);
        public static readonly DetailedSignalType EMAGoldenCross = new(nameof(EMAGoldenCross), 5016);
        public static readonly DetailedSignalType EMADeathCross = new(nameof(EMADeathCross), 5017);
        public static readonly DetailedSignalType EMASupportBounce = new(nameof(EMASupportBounce), 5018);
        public static readonly DetailedSignalType EMAResistanceRejection = new(nameof(EMAResistanceRejection), 5019);
        public static readonly DetailedSignalType EmaBullishTrendConfirmation = new(nameof(EmaBullishTrendConfirmation), 5020);
        public static readonly DetailedSignalType EmaBearishTrendConfirmation = new(nameof(EmaBearishTrendConfirmation), 5021);
        public static readonly DetailedSignalType EmaBullishBounce = new(nameof(EmaBullishBounce), 5022);
        public static readonly DetailedSignalType EmaBearishBounce = new(nameof(EmaBearishBounce), 5023);
        public static readonly DetailedSignalType EmaBullishBreakout = new(nameof(EmaBullishBreakout), 5024);
        public static readonly DetailedSignalType EmaBearishBreakout = new(nameof(EmaBearishBreakout), 5025);
        public static readonly DetailedSignalType EmaConvergence = new(nameof(EmaConvergence), 5026);
        public static readonly DetailedSignalType EmaDivergence = new(nameof(EmaDivergence), 5027);
        public static readonly DetailedSignalType EmaCrossAbove = new(nameof(EmaCrossAbove), 5028);
        public static readonly DetailedSignalType EmaCrossBelow = new(nameof(EmaCrossBelow), 5029);
        // Note: Avoid name collisions with EMAGoldenCross/EMADeathCross (case-insensitive map)

        // ADX Additional Signals
        public static readonly DetailedSignalType ADXTrendStrengthIncreasing = new(nameof(ADXTrendStrengthIncreasing), 7007);

        public static readonly DetailedSignalType ADXTrendStrengthDecreasing = new(nameof(ADXTrendStrengthDecreasing), 7008);
        public static readonly DetailedSignalType ADXBullishTrend = new(nameof(ADXBullishTrend), 7009);
        public static readonly DetailedSignalType ADXBearishTrend = new(nameof(ADXBearishTrend), 7010);

        // Note: Avoid name collisions with ADXFalling (case-insensitive map)
        public static readonly DetailedSignalType AdxDiPlusCrossAbove = new(nameof(AdxDiPlusCrossAbove), 7012);

        public static readonly DetailedSignalType AdxDiMinusCrossAbove = new(nameof(AdxDiMinusCrossAbove), 7013);
        public static readonly DetailedSignalType AdxTrendStrength = new(nameof(AdxTrendStrength), 7014);
        public static readonly DetailedSignalType AdxWeakTrend = new(nameof(AdxWeakTrend), 7015);
        // Note: Avoid name collisions with ADXRising (case-insensitive map)

        // CCI Additional Signals
        public static readonly DetailedSignalType CCIBullishDivergence = new(nameof(CCIBullishDivergence), 8007);

        public static readonly DetailedSignalType CCIBearishDivergence = new(nameof(CCIBearishDivergence), 8008);
        public static readonly DetailedSignalType CciZeroCrossBelow = new(nameof(CciZeroCrossBelow), 8009);

        // Note: Avoid name collisions with CCIOverBought/CCIOverSold (case-insensitive map)
        public static readonly DetailedSignalType CciZeroCrossAbove = new(nameof(CciZeroCrossAbove), 8012);

        // Candlestick Pattern Signals
        public static readonly DetailedSignalType CandlestickDoji = new(nameof(CandlestickDoji), 15001);

        public static readonly DetailedSignalType CandlestickHammer = new(nameof(CandlestickHammer), 15002);
        public static readonly DetailedSignalType CandlestickShootingStar = new(nameof(CandlestickShootingStar), 15003);
        public static readonly DetailedSignalType CandlestickEngulfingBullish = new(nameof(CandlestickEngulfingBullish), 15004);
        public static readonly DetailedSignalType CandlestickEngulfingBearish = new(nameof(CandlestickEngulfingBearish), 15005);
        public static readonly DetailedSignalType CandlestickMorningStar = new(nameof(CandlestickMorningStar), 15006);
        public static readonly DetailedSignalType CandlestickEveningStar = new(nameof(CandlestickEveningStar), 15007);
        public static readonly DetailedSignalType CandlestickThreeWhiteSoldiers = new(nameof(CandlestickThreeWhiteSoldiers), 15008);
        public static readonly DetailedSignalType CandlestickThreeBlackCrows = new(nameof(CandlestickThreeBlackCrows), 15009);
        public static readonly DetailedSignalType CandlestickBullishHarami = new(nameof(CandlestickBullishHarami), 15010);
        public static readonly DetailedSignalType CandlestickBearishHarami = new(nameof(CandlestickBearishHarami), 15011);

        private static readonly Dictionary<string, DetailedSignalType> _byName = new(StringComparer.OrdinalIgnoreCase)
        {
            { nameof(IchimokuCloudBreakoutUp), IchimokuCloudBreakoutUp },
            { nameof(IchimokuCloudBreakoutDown), IchimokuCloudBreakoutDown },
            { nameof(IchimokuTenkanKijunCrossUp), IchimokuTenkanKijunCrossUp },
            { nameof(IchimokuTenkanKijunCrossDown), IchimokuTenkanKijunCrossDown },
            { nameof(IchimokuChikouSpanConfirmation), IchimokuChikouSpanConfirmation },
            { nameof(IchimokuFutureCloudGreen), IchimokuFutureCloudGreen },
            { nameof(IchimokuFutureCloudRed), IchimokuFutureCloudRed },

            { nameof(BollingerBandsSqueeze), BollingerBandsSqueeze },
            { nameof(BollingerBandsExpansion), BollingerBandsExpansion },
            { nameof(BollingerUpperBandTouch), BollingerUpperBandTouch },
            { nameof(BollingerLowerBandTouch), BollingerLowerBandTouch },
            { nameof(BollingerMiddleBandCrossUp), BollingerMiddleBandCrossUp },
            { nameof(BollingerMiddleBandCrossDown), BollingerMiddleBandCrossDown },
            { nameof(BollingerBandBreakoutUp), BollingerBandBreakoutUp },
            { nameof(BollingerBandBreakoutDown), BollingerBandBreakoutDown },

            { nameof(RSIOverBought), RSIOverBought },
            { nameof(RSIOverSold), RSIOverSold },
            { nameof(RSIDivergenceBullish), RSIDivergenceBullish },
            { nameof(RSIDivergenceBearish), RSIDivergenceBearish },
            { nameof(RSICenterLineCrossUp), RSICenterLineCrossUp },
            { nameof(RSICenterLineCrossDown), RSICenterLineCrossDown },

            { nameof(MACDSignalLineCrossUp), MACDSignalLineCrossUp },
            { nameof(MACDSignalLineCrossDown), MACDSignalLineCrossDown },
            { nameof(MACDZeroLineCrossUp), MACDZeroLineCrossUp },
            { nameof(MACDZeroLineCrossDown), MACDZeroLineCrossDown },
            { nameof(MACDHistogramIncreasing), MACDHistogramIncreasing },
            { nameof(MACDHistogramDecreasing), MACDHistogramDecreasing },
            { nameof(MACDDivergenceBullish), MACDDivergenceBullish },
            { nameof(MACDDivergenceBearish), MACDDivergenceBearish },

            { nameof(SMAGoldenCross), SMAGoldenCross },
            { nameof(SMADeathCross), SMADeathCross },
            { nameof(EMACrossUp), EMACrossUp },
            { nameof(EMACrossDown), EMACrossDown },
            { nameof(MASlope), MASlope },
            { nameof(MASupport), MASupport },
            { nameof(MAResistance), MAResistance },

            { nameof(StochasticOverBought), StochasticOverBought },
            { nameof(StochasticOverSold), StochasticOverSold },
            { nameof(StochasticKDCrossUp), StochasticKDCrossUp },
            { nameof(StochasticKDCrossDown), StochasticKDCrossDown },
            { nameof(StochasticDivergenceBullish), StochasticDivergenceBullish },
            { nameof(StochasticDivergenceBearish), StochasticDivergenceBearish },

            { nameof(ADXTrendStrengthHigh), ADXTrendStrengthHigh },
            { nameof(ADXTrendStrengthLow), ADXTrendStrengthLow },
            { nameof(ADXRising), ADXRising },
            { nameof(ADXFalling), ADXFalling },
            { nameof(ADXDICrossoverBullish), ADXDICrossoverBullish },
            { nameof(ADXDICrossoverBearish), ADXDICrossoverBearish },

            { nameof(CCIOverBought), CCIOverBought },
            { nameof(CCIOverSold), CCIOverSold },
            { nameof(CCIZeroCrossUp), CCIZeroCrossUp },
            { nameof(CCIZeroCrossDown), CCIZeroCrossDown },
            { nameof(CCIDivergenceBullish), CCIDivergenceBullish },
            { nameof(CCIDivergenceBearish), CCIDivergenceBearish },

            { nameof(WilliamsROverBought), WilliamsROverBought },
            { nameof(WilliamsROverSold), WilliamsROverSold },
            { nameof(WilliamsRCrossUp), WilliamsRCrossUp },
            { nameof(WilliamsRCrossDown), WilliamsRCrossDown },

            { nameof(VolumeSpike), VolumeSpike },
            { nameof(VolumeBreakout), VolumeBreakout },
            { nameof(VolumeConfirmation), VolumeConfirmation },
            { nameof(VolumeDivergence), VolumeDivergence },

            { nameof(SupportBreakdown), SupportBreakdown },
            { nameof(ResistanceBreakout), ResistanceBreakout },
            { nameof(SupportBounce), SupportBounce },
            { nameof(ResistanceRejection), ResistanceRejection },

            { nameof(DoubleTop), DoubleTop },
            { nameof(DoubleBottom), DoubleBottom },
            { nameof(HeadAndShoulders), HeadAndShoulders },
            { nameof(InverseHeadAndShoulders), InverseHeadAndShoulders },
            { nameof(Triangle), Triangle },
            { nameof(Flag), Flag },
            { nameof(Pennant), Pennant },
            { nameof(Wedge), Wedge },

            { nameof(FibonacciRetracement382), FibonacciRetracement382 },
            { nameof(FibonacciRetracement500), FibonacciRetracement500 },
            { nameof(FibonacciRetracement618), FibonacciRetracement618 },
            { nameof(FibonacciExtension161), FibonacciExtension161 },
            { nameof(FibonacciExtension261), FibonacciExtension261 },

            { nameof(PivotPointBreakout), PivotPointBreakout },
            { nameof(PivotPointSupport), PivotPointSupport },
            { nameof(PivotPointResistance), PivotPointResistance },

            { nameof(IchimokuTenkanSenBullishCross), IchimokuTenkanSenBullishCross },
            { nameof(IchimokuTenkanSenBearishCross), IchimokuTenkanSenBearishCross },
            { nameof(IchimokuKijunSenBullishCross), IchimokuKijunSenBullishCross },
            { nameof(IchimokuKijunSenBearishCross), IchimokuKijunSenBearishCross },
            { nameof(IchimokuTenkanKijunBullishCross), IchimokuTenkanKijunBullishCross },
            { nameof(IchimokuTenkanKijunBearishCross), IchimokuTenkanKijunBearishCross },
            { nameof(IchimokuKumoBullishBreakout), IchimokuKumoBullishBreakout },
            { nameof(IchimokuKumoBearishBreakdown), IchimokuKumoBearishBreakdown },
            { nameof(IchimokuChikouSpanBullishConfirmation), IchimokuChikouSpanBullishConfirmation },
            { nameof(IchimokuChikouSpanBearishConfirmation), IchimokuChikouSpanBearishConfirmation },
            { nameof(IchimokuBullishCloudFormation), IchimokuBullishCloudFormation },
            { nameof(IchimokuBearishCloudFormation), IchimokuBearishCloudFormation },
            { nameof(IchimokuFlatCloudFormation), IchimokuFlatCloudFormation },

            { nameof(MovingAverageGoldenCross), MovingAverageGoldenCross },
            { nameof(MovingAverageDeathCross), MovingAverageDeathCross },
            { nameof(MovingAverageSupportBounce), MovingAverageSupportBounce },
            { nameof(MovingAverageResistanceRejection), MovingAverageResistanceRejection },
            { nameof(MovingAverageBullishBreakout), MovingAverageBullishBreakout },
            { nameof(MovingAverageBearishBreakdown), MovingAverageBearishBreakdown },

            { nameof(EMABullishCross), EMABullishCross },
            { nameof(EMABearishCross), EMABearishCross },
            { nameof(EMAGoldenCross), EMAGoldenCross },
            { nameof(EMADeathCross), EMADeathCross },
            { nameof(EMASupportBounce), EMASupportBounce },
            { nameof(EMAResistanceRejection), EMAResistanceRejection },
            { nameof(EmaBullishTrendConfirmation), EmaBullishTrendConfirmation },
            { nameof(EmaBearishTrendConfirmation), EmaBearishTrendConfirmation },
            { nameof(EmaBullishBounce), EmaBullishBounce },
            { nameof(EmaBearishBounce), EmaBearishBounce },
            { nameof(EmaBullishBreakout), EmaBullishBreakout },
            { nameof(EmaBearishBreakout), EmaBearishBreakout },
            { nameof(EmaConvergence), EmaConvergence },
            { nameof(EmaDivergence), EmaDivergence },
            { nameof(EmaCrossAbove), EmaCrossAbove },
            { nameof(EmaCrossBelow), EmaCrossBelow },
            // removed: EmaGoldenCross/EmaDeathCross to avoid collision with EMAGoldenCross/EMADeathCross

            { nameof(ADXTrendStrengthIncreasing), ADXTrendStrengthIncreasing },
            { nameof(ADXTrendStrengthDecreasing), ADXTrendStrengthDecreasing },
            { nameof(ADXBullishTrend), ADXBullishTrend },
            { nameof(ADXBearishTrend), ADXBearishTrend },
            // removed: AdxFalling to avoid collision with ADXFalling
            { nameof(AdxDiPlusCrossAbove), AdxDiPlusCrossAbove },
            { nameof(AdxDiMinusCrossAbove), AdxDiMinusCrossAbove },
            { nameof(AdxTrendStrength), AdxTrendStrength },
            { nameof(AdxWeakTrend), AdxWeakTrend },
            // removed: AdxRising to avoid collision with ADXRising

            { nameof(CCIBullishDivergence), CCIBullishDivergence },
            { nameof(CCIBearishDivergence), CCIBearishDivergence },
            { nameof(CciZeroCrossBelow), CciZeroCrossBelow },
            // removed: CciOverbought/CciOversold to avoid collision with CCIOverBought/CCIOverSold
            { nameof(CciZeroCrossAbove), CciZeroCrossAbove },

            { nameof(CandlestickDoji), CandlestickDoji },
            { nameof(CandlestickHammer), CandlestickHammer },
            { nameof(CandlestickShootingStar), CandlestickShootingStar },
            { nameof(CandlestickEngulfingBullish), CandlestickEngulfingBullish },
            { nameof(CandlestickEngulfingBearish), CandlestickEngulfingBearish },
            { nameof(CandlestickMorningStar), CandlestickMorningStar },
            { nameof(CandlestickEveningStar), CandlestickEveningStar },
            { nameof(CandlestickThreeWhiteSoldiers), CandlestickThreeWhiteSoldiers },
            { nameof(CandlestickThreeBlackCrows), CandlestickThreeBlackCrows },
            { nameof(CandlestickBullishHarami), CandlestickBullishHarami },
            { nameof(CandlestickBearishHarami), CandlestickBearishHarami },
        };

        private static readonly Dictionary<int, DetailedSignalType> _byValue = new()
        {
            { IchimokuCloudBreakoutUp.Value, IchimokuCloudBreakoutUp },
            { IchimokuCloudBreakoutDown.Value, IchimokuCloudBreakoutDown },
            { IchimokuTenkanKijunCrossUp.Value, IchimokuTenkanKijunCrossUp },
            { IchimokuTenkanKijunCrossDown.Value, IchimokuTenkanKijunCrossDown },
            { IchimokuChikouSpanConfirmation.Value, IchimokuChikouSpanConfirmation },
            { IchimokuFutureCloudGreen.Value, IchimokuFutureCloudGreen },
            { IchimokuFutureCloudRed.Value, IchimokuFutureCloudRed },

            { BollingerBandsSqueeze.Value, BollingerBandsSqueeze },
            { BollingerBandsExpansion.Value, BollingerBandsExpansion },
            { BollingerUpperBandTouch.Value, BollingerUpperBandTouch },
            { BollingerLowerBandTouch.Value, BollingerLowerBandTouch },
            { BollingerMiddleBandCrossUp.Value, BollingerMiddleBandCrossUp },
            { BollingerMiddleBandCrossDown.Value, BollingerMiddleBandCrossDown },
            { BollingerBandBreakoutUp.Value, BollingerBandBreakoutUp },
            { BollingerBandBreakoutDown.Value, BollingerBandBreakoutDown },

            { RSIOverBought.Value, RSIOverBought },
            { RSIOverSold.Value, RSIOverSold },
            { RSIDivergenceBullish.Value, RSIDivergenceBullish },
            { RSIDivergenceBearish.Value, RSIDivergenceBearish },
            { RSICenterLineCrossUp.Value, RSICenterLineCrossUp },
            { RSICenterLineCrossDown.Value, RSICenterLineCrossDown },

            { MACDSignalLineCrossUp.Value, MACDSignalLineCrossUp },
            { MACDSignalLineCrossDown.Value, MACDSignalLineCrossDown },
            { MACDZeroLineCrossUp.Value, MACDZeroLineCrossUp },
            { MACDZeroLineCrossDown.Value, MACDZeroLineCrossDown },
            { MACDHistogramIncreasing.Value, MACDHistogramIncreasing },
            { MACDHistogramDecreasing.Value, MACDHistogramDecreasing },
            { MACDDivergenceBullish.Value, MACDDivergenceBullish },
            { MACDDivergenceBearish.Value, MACDDivergenceBearish },

            { SMAGoldenCross.Value, SMAGoldenCross },
            { SMADeathCross.Value, SMADeathCross },
            { EMACrossUp.Value, EMACrossUp },
            { EMACrossDown.Value, EMACrossDown },
            { MASlope.Value, MASlope },
            { MASupport.Value, MASupport },
            { MAResistance.Value, MAResistance },

            { StochasticOverBought.Value, StochasticOverBought },
            { StochasticOverSold.Value, StochasticOverSold },
            { StochasticKDCrossUp.Value, StochasticKDCrossUp },
            { StochasticKDCrossDown.Value, StochasticKDCrossDown },
            { StochasticDivergenceBullish.Value, StochasticDivergenceBullish },
            { StochasticDivergenceBearish.Value, StochasticDivergenceBearish },

            { ADXTrendStrengthHigh.Value, ADXTrendStrengthHigh },
            { ADXTrendStrengthLow.Value, ADXTrendStrengthLow },
            { ADXRising.Value, ADXRising },
            { ADXFalling.Value, ADXFalling },
            { ADXDICrossoverBullish.Value, ADXDICrossoverBullish },
            { ADXDICrossoverBearish.Value, ADXDICrossoverBearish },

            { CCIOverBought.Value, CCIOverBought },
            { CCIOverSold.Value, CCIOverSold },
            { CCIZeroCrossUp.Value, CCIZeroCrossUp },
            { CCIZeroCrossDown.Value, CCIZeroCrossDown },
            { CCIDivergenceBullish.Value, CCIDivergenceBullish },
            { CCIDivergenceBearish.Value, CCIDivergenceBearish },

            { WilliamsROverBought.Value, WilliamsROverBought },
            { WilliamsROverSold.Value, WilliamsROverSold },
            { WilliamsRCrossUp.Value, WilliamsRCrossUp },
            { WilliamsRCrossDown.Value, WilliamsRCrossDown },

            { VolumeSpike.Value, VolumeSpike },
            { VolumeBreakout.Value, VolumeBreakout },
            { VolumeConfirmation.Value, VolumeConfirmation },
            { VolumeDivergence.Value, VolumeDivergence },

            { SupportBreakdown.Value, SupportBreakdown },
            { ResistanceBreakout.Value, ResistanceBreakout },
            { SupportBounce.Value, SupportBounce },
            { ResistanceRejection.Value, ResistanceRejection },

            { DoubleTop.Value, DoubleTop },
            { DoubleBottom.Value, DoubleBottom },
            { HeadAndShoulders.Value, HeadAndShoulders },
            { InverseHeadAndShoulders.Value, InverseHeadAndShoulders },
            { Triangle.Value, Triangle },
            { Flag.Value, Flag },
            { Pennant.Value, Pennant },
            { Wedge.Value, Wedge },

            { FibonacciRetracement382.Value, FibonacciRetracement382 },
            { FibonacciRetracement500.Value, FibonacciRetracement500 },
            { FibonacciRetracement618.Value, FibonacciRetracement618 },
            { FibonacciExtension161.Value, FibonacciExtension161 },
            { FibonacciExtension261.Value, FibonacciExtension261 },

            { PivotPointBreakout.Value, PivotPointBreakout },
            { PivotPointSupport.Value, PivotPointSupport },
            { PivotPointResistance.Value, PivotPointResistance },

            { IchimokuTenkanSenBullishCross.Value, IchimokuTenkanSenBullishCross },
            { IchimokuTenkanSenBearishCross.Value, IchimokuTenkanSenBearishCross },
            { IchimokuKijunSenBullishCross.Value, IchimokuKijunSenBullishCross },
            { IchimokuKijunSenBearishCross.Value, IchimokuKijunSenBearishCross },
            { IchimokuTenkanKijunBullishCross.Value, IchimokuTenkanKijunBullishCross },
            { IchimokuTenkanKijunBearishCross.Value, IchimokuTenkanKijunBearishCross },
            { IchimokuKumoBullishBreakout.Value, IchimokuKumoBullishBreakout },
            { IchimokuKumoBearishBreakdown.Value, IchimokuKumoBearishBreakdown },
            { IchimokuChikouSpanBullishConfirmation.Value, IchimokuChikouSpanBullishConfirmation },
            { IchimokuChikouSpanBearishConfirmation.Value, IchimokuChikouSpanBearishConfirmation },
            { IchimokuBullishCloudFormation.Value, IchimokuBullishCloudFormation },
            { IchimokuBearishCloudFormation.Value, IchimokuBearishCloudFormation },
            { IchimokuFlatCloudFormation.Value, IchimokuFlatCloudFormation },

            { MovingAverageGoldenCross.Value, MovingAverageGoldenCross },
            { MovingAverageDeathCross.Value, MovingAverageDeathCross },
            { MovingAverageSupportBounce.Value, MovingAverageSupportBounce },
            { MovingAverageResistanceRejection.Value, MovingAverageResistanceRejection },
            { MovingAverageBullishBreakout.Value, MovingAverageBullishBreakout },
            { MovingAverageBearishBreakdown.Value, MovingAverageBearishBreakdown },

            { EMABullishCross.Value, EMABullishCross },
            { EMABearishCross.Value, EMABearishCross },
            { EMAGoldenCross.Value, EMAGoldenCross },
            { EMADeathCross.Value, EMADeathCross },
            { EMASupportBounce.Value, EMASupportBounce },
            { EMAResistanceRejection.Value, EMAResistanceRejection },
            { EmaBullishTrendConfirmation.Value, EmaBullishTrendConfirmation },
            { EmaBearishTrendConfirmation.Value, EmaBearishTrendConfirmation },
            { EmaBullishBounce.Value, EmaBullishBounce },
            { EmaBearishBounce.Value, EmaBearishBounce },
            { EmaBullishBreakout.Value, EmaBullishBreakout },
            { EmaBearishBreakout.Value, EmaBearishBreakout },
            { EmaConvergence.Value, EmaConvergence },
            { EmaDivergence.Value, EmaDivergence },
            { EmaCrossAbove.Value, EmaCrossAbove },
            { EmaCrossBelow.Value, EmaCrossBelow },
            // removed: EmaGoldenCross/EmaDeathCross to avoid collision with EMAGoldenCross/EMADeathCross

            { ADXTrendStrengthIncreasing.Value, ADXTrendStrengthIncreasing },
            { ADXTrendStrengthDecreasing.Value, ADXTrendStrengthDecreasing },
            { ADXBullishTrend.Value, ADXBullishTrend },
            { ADXBearishTrend.Value, ADXBearishTrend },
            // removed: AdxFalling to avoid collision with ADXFalling
            { AdxDiPlusCrossAbove.Value, AdxDiPlusCrossAbove },
            { AdxDiMinusCrossAbove.Value, AdxDiMinusCrossAbove },
            { AdxTrendStrength.Value, AdxTrendStrength },
            { AdxWeakTrend.Value, AdxWeakTrend },
            // removed: AdxRising to avoid collision with ADXRising

            { CCIBullishDivergence.Value, CCIBullishDivergence },
            { CCIBearishDivergence.Value, CCIBearishDivergence },
            { CciZeroCrossBelow.Value, CciZeroCrossBelow },
            // removed: CciOverbought/CciOversold to avoid collision with CCIOverBought/CCIOverSold
            { CciZeroCrossAbove.Value, CciZeroCrossAbove },

            { CandlestickDoji.Value, CandlestickDoji },
            { CandlestickHammer.Value, CandlestickHammer },
            { CandlestickShootingStar.Value, CandlestickShootingStar },
            { CandlestickEngulfingBullish.Value, CandlestickEngulfingBullish },
            { CandlestickEngulfingBearish.Value, CandlestickEngulfingBearish },
            { CandlestickMorningStar.Value, CandlestickMorningStar },
            { CandlestickEveningStar.Value, CandlestickEveningStar },
            { CandlestickThreeWhiteSoldiers.Value, CandlestickThreeWhiteSoldiers },
            { CandlestickThreeBlackCrows.Value, CandlestickThreeBlackCrows },
            { CandlestickBullishHarami.Value, CandlestickBullishHarami },
            { CandlestickBearishHarami.Value, CandlestickBearishHarami },
        };

        public static bool TryFromName(string name, out DetailedSignalType type)
        {
            if (_byName.TryGetValue(name, out var t)) { type = t; return true; }
            type = null; return false;
        }

        public static bool TryFromValue(int value, out DetailedSignalType type)
        {
            if (_byValue.TryGetValue(value, out var t)) { type = t; return true; }
            type = null; return false;
        }

        public override string ToString() => Name;

        public bool Equals(DetailedSignalType other)
            => other is not null && other.Value == Value;

        public override bool Equals(object obj)
            => obj is DetailedSignalType other && Equals(other);

        public override int GetHashCode() => Value.GetHashCode();
    }
}