namespace DomainLayer.Entities;

/// <summary>
/// جزئیات انواع سیگنال‌های تکنیکال با دسته‌بندی کامل
/// </summary>
public enum DetailedSignalType
{
    // Ichimoku Cloud Signals - سیگنال‌های ایچیموکو
    IchimokuCloudBreakoutUp = 1001,           // شکست ابر کومو به بالا
    IchimokuCloudBreakoutDown = 1002,         // شکست ابر کومو به پایین
    IchimokuTenkanKijunCrossUp = 1003,        // تقاطع تنکان و کیجان به بالا
    IchimokuTenkanKijunCrossDown = 1004,      // تقاطع تنکان و کیجان به پایین
    IchimokuChikouSpanConfirmation = 1005,    // تایید چیکو اسپن
    IchimokuFutureCloudGreen = 1006,          // ابر آینده سبز
    IchimokuFutureCloudRed = 1007,            // ابر آینده قرمز

    // Bollinger Bands Signals - سیگنال‌های باند بولینگر
    BollingerBandsSqueeze = 2001,             // فشردگی باندها
    BollingerBandsExpansion = 2002,           // گسترش باندها
    BollingerUpperBandTouch = 2003,           // لمس باند بالایی
    BollingerLowerBandTouch = 2004,           // لمس باند پایینی
    BollingerMiddleBandCrossUp = 2005,        // عبور از میانگین متحرک به بالا
    BollingerMiddleBandCrossDown = 2006,      // عبور از میانگین متحرک به پایین
    BollingerBandBreakoutUp = 2007,           // شکست باند بالایی
    BollingerBandBreakoutDown = 2008,         // شکست باند پایینی

    // RSI Signals - سیگنال‌های RSI
    RSIOverBought = 3001,                     // اشباع خرید (بالای 70)
    RSIOverSold = 3002,                       // اشباع فروش (زیر 30)
    RSIDivergenceBullish = 3003,              // واگرایی صعودی
    RSIDivergenceBearish = 3004,              // واگرایی نزولی
    RSICenterLineCrossUp = 3005,              // عبور از خط 50 به بالا
    RSICenterLineCrossDown = 3006,            // عبور از خط 50 به پایین

    // MACD Signals - سیگنال‌های MACD
    MACDSignalLineCrossUp = 4001,             // تقاطع خط سیگنال به بالا
    MACDSignalLineCrossDown = 4002,           // تقاطع خط سیگنال به پایین
    MACDZeroLineCrossUp = 4003,               // عبور از خط صفر به بالا
    MACDZeroLineCrossDown = 4004,             // عبور از خط صفر به پایین
    MACDHistogramIncreasing = 4005,           // افزایش هیستوگرام
    MACDHistogramDecreasing = 4006,           // کاهش هیستوگرام
    MACDDivergenceBullish = 4007,             // واگرایی صعودی
    MACDDivergenceBearish = 4008,             // واگرایی نزولی

    // Moving Average Signals - سیگنال‌های میانگین متحرک
    SMAGoldenCross = 5001,                    // تقاطع طلایی (MA50 > MA200)
    SMADeathCross = 5002,                     // تقاطع مرگ (MA50 < MA200)
    EMACrossUp = 5003,                        // عبور قیمت از EMA به بالا
    EMACrossDown = 5004,                      // عبور قیمت از EMA به پایین
    MASlope = 5005,                           // شیب میانگین متحرک
    MASupport = 5006,                         // حمایت میانگین متحرک
    MAResistance = 5007,                      // مقاومت میانگین متحرک

    // Stochastic Signals - سیگنال‌های استوکاستیک
    StochasticOverBought = 6001,              // اشباع خرید (بالای 80)
    StochasticOverSold = 6002,                // اشباع فروش (زیر 20)
    StochasticKDCrossUp = 6003,               // تقاطع %K و %D به بالا
    StochasticKDCrossDown = 6004,             // تقاطع %K و %D به پایین
    StochasticDivergenceBullish = 6005,       // واگرایی صعودی
    StochasticDivergenceBearish = 6006,       // واگرایی نزولی

    // ADX Signals - سیگنال‌های ADX
    ADXTrendStrengthHigh = 7001,              // قدرت ترند بالا (ADX > 25)
    ADXTrendStrengthLow = 7002,               // قدرت ترند پایین (ADX < 20)
    ADXRising = 7003,                         // افزایش ADX
    ADXFalling = 7004,                        // کاهش ADX
    ADXDICrossoverBullish = 7005,             // تقاطع DI+ و DI- صعودی
    ADXDICrossoverBearish = 7006,             // تقاطع DI+ و DI- نزولی

    // CCI Signals - سیگنال‌های CCI
    CCIOverBought = 8001,                     // اشباع خرید (بالای +100)
    CCIOverSold = 8002,                       // اشباع فروش (زیر -100)
    CCIZeroCrossUp = 8003,                    // عبور از خط صفر به بالا
    CCIZeroCrossDown = 8004,                  // عبور از خط صفر به پایین
    CCIDivergenceBullish = 8005,              // واگرایی صعودی
    CCIDivergenceBearish = 8006,              // واگرایی نزولی

    // Williams %R Signals - سیگنال‌های ویلیامز %R
    WilliamsROverBought = 9001,               // اشباع خرید (بالای -20)
    WilliamsROverSold = 9002,                 // اشباع فروش (زیر -80)
    WilliamsRCrossUp = 9003,                  // عبور از سطح -50 به بالا
    WilliamsRCrossDown = 9004,                // عبور از سطح -50 به پایین

    // Volume Signals - سیگنال‌های حجم
    VolumeSpike = 10001,                      // افزایش ناگهانی حجم
    VolumeBreakout = 10002,                   // شکست با حجم بالا
    VolumeConfirmation = 10003,               // تایید حجمی
    VolumeDivergence = 10004,                 // واگرایی حجمی

    // Support/Resistance Signals - سیگنال‌های حمایت/مقاومت
    SupportBreakdown = 11001,                 // شکست سطح حمایت
    ResistanceBreakout = 11002,               // شکست سطح مقاومت
    SupportBounce = 11003,                    // بازگشت از سطح حمایت
    ResistanceRejection = 11004,              // رد شدن از سطح مقاومت

    // Pattern Signals - سیگنال‌های الگو
    DoubleTop = 12001,                        // دوقله
    DoubleBottom = 12002,                     // دوکف
    HeadAndShoulders = 12003,                 // سر و شانه
    InverseHeadAndShoulders = 12004,          // سر و شانه معکوس
    Triangle = 12005,                         // مثلث
    Flag = 12006,                             // پرچم
    Pennant = 12007,                          // بادبان
    Wedge = 12008,                            // گوه

    // Fibonacci Signals - سیگنال‌های فیبوناچی
    FibonacciRetracement382 = 13001,          // بازگشت 38.2% فیبوناچی
    FibonacciRetracement500 = 13002,          // بازگشت 50% فیبوناچی
    FibonacciRetracement618 = 13003,          // بازگشت 61.8% فیبوناچی
    FibonacciExtension161 = 13004,            // تمدید 161.8% فیبوناچی
    FibonacciExtension261 = 13005,            // تمدید 261.8% فیبوناچی

    // Pivot Point Signals - سیگنال‌های نقطه محوری
    PivotPointBreakout = 14001,               // شکست نقطه محوری
    PivotPointSupport = 14002,                // حمایت نقطه محوری
    PivotPointResistance = 14003,             // مقاومت نقطه محوری

    // Additional Ichimoku Signals - سیگنال‌های اضافی ایچیموکو
    IchimokuTenkanSenBullishCross = 1008,     // تقاطع صعودی تنکان سن
    IchimokuTenkanSenBearishCross = 1009,     // تقاطع نزولی تنکان سن
    IchimokuKijunSenBullishCross = 1010,      // تقاطع صعودی کیجان سن
    IchimokuKijunSenBearishCross = 1011,      // تقاطع نزولی کیجان سن
    IchimokuTenkanKijunBullishCross = 1012,   // تقاطع صعودی تنکان-کیجان
    IchimokuTenkanKijunBearishCross = 1013,   // تقاطع نزولی تنکان-کیجان
    IchimokuKumoBullishBreakout = 1014,       // شکست صعودی کومو
    IchimokuKumoBearishBreakdown = 1015,      // شکست نزولی کومو
    IchimokuChikouSpanBullishConfirmation = 1016, // تایید صعودی چیکو اسپن
    IchimokuChikouSpanBearishConfirmation = 1017, // تایید نزولی چیکو اسپن
    IchimokuBullishCloudFormation = 1018,     // تشکیل ابر صعودی
    IchimokuBearishCloudFormation = 1019,     // تشکیل ابر نزولی
    IchimokuFlatCloudFormation = 1020,        // تشکیل ابر تخت

    // Additional Moving Average Signals - سیگنال‌های اضافی میانگین متحرک
    MovingAverageGoldenCross = 5008,          // تقاطع طلایی میانگین متحرک
    MovingAverageDeathCross = 5009,           // تقاطع مرگ میانگین متحرک
    MovingAverageSupportBounce = 5010,        // بازگشت از حمایت میانگین متحرک
    MovingAverageResistanceRejection = 5011,  // رد از مقاومت میانگین متحرک
    MovingAverageBullishBreakout = 5012,      // شکست صعودی میانگین متحرک
    MovingAverageBearishBreakdown = 5013,     // شکست نزولی میانگین متحرک

    // EMA Signals - سیگنال‌های EMA
    EMABullishCross = 5014,                   // تقاطع صعودی EMA
    EMABearishCross = 5015,                   // تقاطع نزولی EMA
    EMAGoldenCross = 5016,                    // تقاطع طلایی EMA
    EMADeathCross = 5017,                     // تقاطع مرگ EMA
    EMASupportBounce = 5018,                  // بازگشت از حمایت EMA
    EMAResistanceRejection = 5019,            // رد از مقاومت EMA
    EmaBullishTrendConfirmation = 5020,       // تایید ترند صعودی EMA
    EmaBearishTrendConfirmation = 5021,       // تایید ترند نزولی EMA
    EmaBullishBounce = 5022,                  // بازگشت صعودی EMA
    EmaBearishBounce = 5023,                  // بازگشت نزولی EMA
    EmaBullishBreakout = 5024,                // شکست صعودی EMA
    EmaBearishBreakout = 5025,                // شکست نزولی EMA
    EmaConvergence = 5026,                    // همگرایی EMA
    EmaDivergence = 5027,                     // واگرایی EMA
    EmaCrossAbove = 5028,                     // عبور EMA از بالا
    EmaCrossBelow = 5029,                     // عبور EMA از پایین
    EmaGoldenCross = 5030,                    // تقاطع طلایی EMA
    EmaDeathCross = 5031,                     // تقاطع مرگ EMA

    // ADX Additional Signals - سیگنال‌های اضافی ADX
    ADXTrendStrengthIncreasing = 7007,        // افزایش قدرت ترند
    ADXTrendStrengthDecreasing = 7008,        // کاهش قدرت ترند
    ADXBullishTrend = 7009,                   // ترند صعودی
    ADXBearishTrend = 7010,                   // ترند نزولی
    AdxFalling = 7011,                        // کاهش ADX
    AdxDiPlusCrossAbove = 7012,               // عبور DI+ از بالا
    AdxDiMinusCrossAbove = 7013,              // عبور DI- از بالا
    AdxTrendStrength = 7014,                  // قدرت ترند ADX
    AdxWeakTrend = 7015,                      // ترند ضعیف ADX
    AdxRising = 7016,                         // افزایش ADX

    // CCI Additional Signals - سیگنال‌های اضافی CCI
    CCIBullishDivergence = 8007,              // واگرایی صعودی CCI
    CCIBearishDivergence = 8008,              // واگرایی نزولی CCI
    CciZeroCrossBelow = 8009,                 // عبور CCI از زیر صفر
    CciOverbought = 8010,                     // اشباع خرید CCI
    CciOversold = 8011,                       // اشباع فروش CCI
    CciZeroCrossAbove = 8012,                 // عبور CCI از بالای صفر

    // Candlestick Pattern Signals - سیگنال‌های الگوی کندل
    CandlestickDoji = 15001,                  // دوجی
    CandlestickHammer = 15002,                // چکش
    CandlestickShootingStar = 15003,          // ستاره دنباله‌دار
    CandlestickEngulfingBullish = 15004,      // بلعیدن صعودی
    CandlestickEngulfingBearish = 15005,      // بلعیدن نزولی
    CandlestickMorningStar = 15006,           // ستاره صبح
    CandlestickEveningStar = 15007,           // ستاره عصر
    CandlestickThreeWhiteSoldiers = 15008,    // سه سرباز سفید
    CandlestickThreeBlackCrows = 15009,       // سه کلاغ سیاه
    CandlestickBullishHarami = 15010,         // الگوی حرامی صعودی
    CandlestickBearishHarami = 15011,         // الگوی حرامی نزولی
}

/// <summary>
/// دسته‌بندی سیگنال‌ها
/// </summary>
public static class SignalCategories
{
    public static readonly Dictionary<string, List<DetailedSignalType>> Categories = new()
    {
        ["Ichimoku"] = new List<DetailedSignalType>
        {
            DetailedSignalType.IchimokuCloudBreakoutUp,
            DetailedSignalType.IchimokuCloudBreakoutDown,
            DetailedSignalType.IchimokuTenkanKijunCrossUp,
            DetailedSignalType.IchimokuTenkanKijunCrossDown,
            DetailedSignalType.IchimokuChikouSpanConfirmation,
            DetailedSignalType.IchimokuFutureCloudGreen,
            DetailedSignalType.IchimokuFutureCloudRed,
            DetailedSignalType.IchimokuTenkanSenBullishCross,
            DetailedSignalType.IchimokuTenkanSenBearishCross,
            DetailedSignalType.IchimokuKijunSenBullishCross,
            DetailedSignalType.IchimokuKijunSenBearishCross,
            DetailedSignalType.IchimokuTenkanKijunBullishCross,
            DetailedSignalType.IchimokuTenkanKijunBearishCross,
            DetailedSignalType.IchimokuKumoBullishBreakout,
            DetailedSignalType.IchimokuKumoBearishBreakdown,
            DetailedSignalType.IchimokuChikouSpanBullishConfirmation,
            DetailedSignalType.IchimokuChikouSpanBearishConfirmation,
            DetailedSignalType.IchimokuBullishCloudFormation,
            DetailedSignalType.IchimokuBearishCloudFormation,
            DetailedSignalType.IchimokuFlatCloudFormation
        },
        ["Bollinger Bands"] = new List<DetailedSignalType>
        {
            DetailedSignalType.BollingerBandsSqueeze,
            DetailedSignalType.BollingerBandsExpansion,
            DetailedSignalType.BollingerUpperBandTouch,
            DetailedSignalType.BollingerLowerBandTouch,
            DetailedSignalType.BollingerMiddleBandCrossUp,
            DetailedSignalType.BollingerMiddleBandCrossDown,
            DetailedSignalType.BollingerBandBreakoutUp,
            DetailedSignalType.BollingerBandBreakoutDown
        },
        ["RSI"] = new List<DetailedSignalType>
        {
            DetailedSignalType.RSIOverBought,
            DetailedSignalType.RSIOverSold,
            DetailedSignalType.RSIDivergenceBullish,
            DetailedSignalType.RSIDivergenceBearish,
            DetailedSignalType.RSICenterLineCrossUp,
            DetailedSignalType.RSICenterLineCrossDown
        },
        ["MACD"] = new List<DetailedSignalType>
        {
            DetailedSignalType.MACDSignalLineCrossUp,
            DetailedSignalType.MACDSignalLineCrossDown,
            DetailedSignalType.MACDZeroLineCrossUp,
            DetailedSignalType.MACDZeroLineCrossDown,
            DetailedSignalType.MACDHistogramIncreasing,
            DetailedSignalType.MACDHistogramDecreasing,
            DetailedSignalType.MACDDivergenceBullish,
            DetailedSignalType.MACDDivergenceBearish
        },
        ["Moving Averages"] = new List<DetailedSignalType>
        {
            DetailedSignalType.SMAGoldenCross,
            DetailedSignalType.SMADeathCross,
            DetailedSignalType.EMACrossUp,
            DetailedSignalType.EMACrossDown,
            DetailedSignalType.MASlope,
            DetailedSignalType.MASupport,
            DetailedSignalType.MAResistance,
            DetailedSignalType.MovingAverageGoldenCross,
            DetailedSignalType.MovingAverageDeathCross,
            DetailedSignalType.MovingAverageSupportBounce,
            DetailedSignalType.MovingAverageResistanceRejection,
            DetailedSignalType.MovingAverageBullishBreakout,
            DetailedSignalType.MovingAverageBearishBreakdown,
            DetailedSignalType.EMABullishCross,
            DetailedSignalType.EMABearishCross,
            DetailedSignalType.EMAGoldenCross,
            DetailedSignalType.EMADeathCross,
            DetailedSignalType.EMASupportBounce,
            DetailedSignalType.EMAResistanceRejection,
            DetailedSignalType.EmaBullishTrendConfirmation,
            DetailedSignalType.EmaBearishTrendConfirmation,
            DetailedSignalType.EmaBullishBounce,
            DetailedSignalType.EmaBearishBounce,
            DetailedSignalType.EmaBullishBreakout,
            DetailedSignalType.EmaBearishBreakout,
            DetailedSignalType.EmaConvergence,
            DetailedSignalType.EmaDivergence,
            DetailedSignalType.EmaCrossAbove,
            DetailedSignalType.EmaCrossBelow,
            DetailedSignalType.EmaGoldenCross,
            DetailedSignalType.EmaDeathCross
        },
        ["Stochastic"] = new List<DetailedSignalType>
        {
            DetailedSignalType.StochasticOverBought,
            DetailedSignalType.StochasticOverSold,
            DetailedSignalType.StochasticKDCrossUp,
            DetailedSignalType.StochasticKDCrossDown,
            DetailedSignalType.StochasticDivergenceBullish,
            DetailedSignalType.StochasticDivergenceBearish
        },
        ["ADX"] = new List<DetailedSignalType>
        {
            DetailedSignalType.ADXTrendStrengthHigh,
            DetailedSignalType.ADXTrendStrengthLow,
            DetailedSignalType.ADXRising,
            DetailedSignalType.ADXFalling,
            DetailedSignalType.ADXDICrossoverBullish,
            DetailedSignalType.ADXDICrossoverBearish,
            DetailedSignalType.ADXTrendStrengthIncreasing,
            DetailedSignalType.ADXTrendStrengthDecreasing,
            DetailedSignalType.ADXBullishTrend,
            DetailedSignalType.ADXBearishTrend
        },
        ["CCI"] = new List<DetailedSignalType>
        {
            DetailedSignalType.CCIOverBought,
            DetailedSignalType.CCIOverSold,
            DetailedSignalType.CCIZeroCrossUp,
            DetailedSignalType.CCIZeroCrossDown,
            DetailedSignalType.CCIDivergenceBullish,
            DetailedSignalType.CCIDivergenceBearish,
            DetailedSignalType.CCIBullishDivergence,
            DetailedSignalType.CCIBearishDivergence
        },
        ["Williams %R"] = new List<DetailedSignalType>
        {
            DetailedSignalType.WilliamsROverBought,
            DetailedSignalType.WilliamsROverSold,
            DetailedSignalType.WilliamsRCrossUp,
            DetailedSignalType.WilliamsRCrossDown
        },
        ["Volume"] = new List<DetailedSignalType>
        {
            DetailedSignalType.VolumeSpike,
            DetailedSignalType.VolumeBreakout,
            DetailedSignalType.VolumeConfirmation,
            DetailedSignalType.VolumeDivergence
        },
        ["Support/Resistance"] = new List<DetailedSignalType>
        {
            DetailedSignalType.SupportBreakdown,
            DetailedSignalType.ResistanceBreakout,
            DetailedSignalType.SupportBounce,
            DetailedSignalType.ResistanceRejection
        },
        ["Patterns"] = new List<DetailedSignalType>
        {
            DetailedSignalType.DoubleTop,
            DetailedSignalType.DoubleBottom,
            DetailedSignalType.HeadAndShoulders,
            DetailedSignalType.InverseHeadAndShoulders,
            DetailedSignalType.Triangle,
            DetailedSignalType.Flag,
            DetailedSignalType.Pennant,
            DetailedSignalType.Wedge
        },
        ["Fibonacci"] = new List<DetailedSignalType>
        {
            DetailedSignalType.FibonacciRetracement382,
            DetailedSignalType.FibonacciRetracement500,
            DetailedSignalType.FibonacciRetracement618,
            DetailedSignalType.FibonacciExtension161,
            DetailedSignalType.FibonacciExtension261
        },
        ["Pivot Points"] = new List<DetailedSignalType>
        {
            DetailedSignalType.PivotPointBreakout,
            DetailedSignalType.PivotPointSupport,
            DetailedSignalType.PivotPointResistance
        },
        ["Candlestick Patterns"] = new List<DetailedSignalType>
        {
            DetailedSignalType.CandlestickDoji,
            DetailedSignalType.CandlestickHammer,
            DetailedSignalType.CandlestickShootingStar,
            DetailedSignalType.CandlestickEngulfingBullish,
            DetailedSignalType.CandlestickEngulfingBearish,
            DetailedSignalType.CandlestickMorningStar,
            DetailedSignalType.CandlestickEveningStar,
            DetailedSignalType.CandlestickThreeWhiteSoldiers,
            DetailedSignalType.CandlestickThreeBlackCrows,
            DetailedSignalType.CandlestickBullishHarami,
            DetailedSignalType.CandlestickBearishHarami
        }
    };

    /// <summary>
    /// دریافت نام فارسی سیگنال
    /// </summary>
    public static string GetPersianName(DetailedSignalType signalType)
    {
        return signalType switch
        {
            // Ichimoku
            DetailedSignalType.IchimokuCloudBreakoutUp => "شکست ابر کومو به بالا",
            DetailedSignalType.IchimokuCloudBreakoutDown => "شکست ابر کومو به پایین",
            DetailedSignalType.IchimokuTenkanKijunCrossUp => "تقاطع تنکان و کیجان به بالا",
            DetailedSignalType.IchimokuTenkanKijunCrossDown => "تقاطع تنکان و کیجان به پایین",
            DetailedSignalType.IchimokuChikouSpanConfirmation => "تایید چیکو اسپن",
            DetailedSignalType.IchimokuFutureCloudGreen => "ابر آینده سبز",
            DetailedSignalType.IchimokuFutureCloudRed => "ابر آینده قرمز",

            // Bollinger Bands
            DetailedSignalType.BollingerBandsSqueeze => "فشردگی باندها",
            DetailedSignalType.BollingerBandsExpansion => "گسترش باندها",
            DetailedSignalType.BollingerUpperBandTouch => "لمس باند بالایی",
            DetailedSignalType.BollingerLowerBandTouch => "لمس باند پایینی",
            DetailedSignalType.BollingerMiddleBandCrossUp => "عبور از میانگین متحرک به بالا",
            DetailedSignalType.BollingerMiddleBandCrossDown => "عبور از میانگین متحرک به پایین",
            DetailedSignalType.BollingerBandBreakoutUp => "شکست باند بالایی",
            DetailedSignalType.BollingerBandBreakoutDown => "شکست باند پایینی",

            // RSI
            DetailedSignalType.RSIOverBought => "اشباع خرید",
            DetailedSignalType.RSIOverSold => "اشباع فروش",
            DetailedSignalType.RSIDivergenceBullish => "واگرایی صعودی",
            DetailedSignalType.RSIDivergenceBearish => "واگرایی نزولی",
            DetailedSignalType.RSICenterLineCrossUp => "عبور از خط 50 به بالا",
            DetailedSignalType.RSICenterLineCrossDown => "عبور از خط 50 به پایین",

            // MACD
            DetailedSignalType.MACDSignalLineCrossUp => "تقاطع خط سیگنال به بالا",
            DetailedSignalType.MACDSignalLineCrossDown => "تقاطع خط سیگنال به پایین",
            DetailedSignalType.MACDZeroLineCrossUp => "عبور از خط صفر به بالا",
            DetailedSignalType.MACDZeroLineCrossDown => "عبور از خط صفر به پایین",
            DetailedSignalType.MACDHistogramIncreasing => "افزایش هیستوگرام",
            DetailedSignalType.MACDHistogramDecreasing => "کاهش هیستوگرام",
            DetailedSignalType.MACDDivergenceBullish => "واگرایی صعودی",
            DetailedSignalType.MACDDivergenceBearish => "واگرایی نزولی",

            // Moving Averages
            DetailedSignalType.SMAGoldenCross => "تقاطع طلایی",
            DetailedSignalType.SMADeathCross => "تقاطع مرگ",
            DetailedSignalType.EMACrossUp => "عبور قیمت از EMA به بالا",
            DetailedSignalType.EMACrossDown => "عبور قیمت از EMA به پایین",
            DetailedSignalType.MASlope => "شیب میانگین متحرک",
            DetailedSignalType.MASupport => "حمایت میانگین متحرک",
            DetailedSignalType.MAResistance => "مقاومت میانگین متحرک",

            // Stochastic
            DetailedSignalType.StochasticOverBought => "اشباع خرید",
            DetailedSignalType.StochasticOverSold => "اشباع فروش",
            DetailedSignalType.StochasticKDCrossUp => "تقاطع %K و %D به بالا",
            DetailedSignalType.StochasticKDCrossDown => "تقاطع %K و %D به پایین",
            DetailedSignalType.StochasticDivergenceBullish => "واگرایی صعودی",
            DetailedSignalType.StochasticDivergenceBearish => "واگرایی نزولی",

            // ADX
            DetailedSignalType.ADXTrendStrengthHigh => "قدرت ترند بالا",
            DetailedSignalType.ADXTrendStrengthLow => "قدرت ترند پایین",
            DetailedSignalType.ADXRising => "افزایش ADX",
            DetailedSignalType.ADXFalling => "کاهش ADX",
            DetailedSignalType.ADXDICrossoverBullish => "تقاطع DI+ و DI- صعودی",
            DetailedSignalType.ADXDICrossoverBearish => "تقاطع DI+ و DI- نزولی",

            // CCI
            DetailedSignalType.CCIOverBought => "اشباع خرید",
            DetailedSignalType.CCIOverSold => "اشباع فروش",
            DetailedSignalType.CCIZeroCrossUp => "عبور از خط صفر به بالا",
            DetailedSignalType.CCIZeroCrossDown => "عبور از خط صفر به پایین",
            DetailedSignalType.CCIDivergenceBullish => "واگرایی صعودی",
            DetailedSignalType.CCIDivergenceBearish => "واگرایی نزولی",

            // Williams %R
            DetailedSignalType.WilliamsROverBought => "اشباع خرید",
            DetailedSignalType.WilliamsROverSold => "اشباع فروش",
            DetailedSignalType.WilliamsRCrossUp => "عبور از سطح -50 به بالا",
            DetailedSignalType.WilliamsRCrossDown => "عبور از سطح -50 به پایین",

            // Volume
            DetailedSignalType.VolumeSpike => "افزایش ناگهانی حجم",
            DetailedSignalType.VolumeBreakout => "شکست با حجم بالا",
            DetailedSignalType.VolumeConfirmation => "تایید حجمی",
            DetailedSignalType.VolumeDivergence => "واگرایی حجمی",

            // Support/Resistance
            DetailedSignalType.SupportBreakdown => "شکست سطح حمایت",
            DetailedSignalType.ResistanceBreakout => "شکست سطح مقاومت",
            DetailedSignalType.SupportBounce => "بازگشت از سطح حمایت",
            DetailedSignalType.ResistanceRejection => "رد شدن از سطح مقاومت",

            // Patterns
            DetailedSignalType.DoubleTop => "دوقله",
            DetailedSignalType.DoubleBottom => "دوکف",
            DetailedSignalType.HeadAndShoulders => "سر و شانه",
            DetailedSignalType.InverseHeadAndShoulders => "سر و شانه معکوس",
            DetailedSignalType.Triangle => "مثلث",
            DetailedSignalType.Flag => "پرچم",
            DetailedSignalType.Pennant => "بادبان",
            DetailedSignalType.Wedge => "گوه",

            // Fibonacci
            DetailedSignalType.FibonacciRetracement382 => "بازگشت 38.2% فیبوناچی",
            DetailedSignalType.FibonacciRetracement500 => "بازگشت 50% فیبوناچی",
            DetailedSignalType.FibonacciRetracement618 => "بازگشت 61.8% فیبوناچی",
            DetailedSignalType.FibonacciExtension161 => "تمدید 161.8% فیبوناچی",
            DetailedSignalType.FibonacciExtension261 => "تمدید 261.8% فیبوناچی",

            // Pivot Points
            DetailedSignalType.PivotPointBreakout => "شکست نقطه محوری",
            DetailedSignalType.PivotPointSupport => "حمایت نقطه محوری",
            DetailedSignalType.PivotPointResistance => "مقاومت نقطه محوری",

            // Additional Ichimoku
            DetailedSignalType.IchimokuTenkanSenBullishCross => "تقاطع صعودی تنکان سن",
            DetailedSignalType.IchimokuTenkanSenBearishCross => "تقاطع نزولی تنکان سن",
            DetailedSignalType.IchimokuKijunSenBullishCross => "تقاطع صعودی کیجان سن",
            DetailedSignalType.IchimokuKijunSenBearishCross => "تقاطع نزولی کیجان سن",
            DetailedSignalType.IchimokuTenkanKijunBullishCross => "تقاطع صعودی تنکان-کیجان",
            DetailedSignalType.IchimokuTenkanKijunBearishCross => "تقاطع نزولی تنکان-کیجان",
            DetailedSignalType.IchimokuKumoBullishBreakout => "شکست صعودی کومو",
            DetailedSignalType.IchimokuKumoBearishBreakdown => "شکست نزولی کومو",
            DetailedSignalType.IchimokuChikouSpanBullishConfirmation => "تایید صعودی چیکو اسپن",
            DetailedSignalType.IchimokuChikouSpanBearishConfirmation => "تایید نزولی چیکو اسپن",
            DetailedSignalType.IchimokuBullishCloudFormation => "تشکیل ابر صعودی",
            DetailedSignalType.IchimokuBearishCloudFormation => "تشکیل ابر نزولی",
            DetailedSignalType.IchimokuFlatCloudFormation => "تشکیل ابر تخت",

            // Additional Moving Averages
            DetailedSignalType.MovingAverageGoldenCross => "تقاطع طلایی میانگین متحرک",
            DetailedSignalType.MovingAverageDeathCross => "تقاطع مرگ میانگین متحرک",
            DetailedSignalType.MovingAverageSupportBounce => "بازگشت از حمایت میانگین متحرک",
            DetailedSignalType.MovingAverageResistanceRejection => "رد از مقاومت میانگین متحرک",
            DetailedSignalType.MovingAverageBullishBreakout => "شکست صعودی میانگین متحرک",
            DetailedSignalType.MovingAverageBearishBreakdown => "شکست نزولی میانگین متحرک",

            // EMA
            DetailedSignalType.EMABullishCross => "تقاطع صعودی EMA",
            DetailedSignalType.EMABearishCross => "تقاطع نزولی EMA",
            DetailedSignalType.EMAGoldenCross => "تقاطع طلایی EMA",
            DetailedSignalType.EMADeathCross => "تقاطع مرگ EMA",
            DetailedSignalType.EMASupportBounce => "بازگشت از حمایت EMA",
            DetailedSignalType.EMAResistanceRejection => "رد از مقاومت EMA",
            DetailedSignalType.EmaBullishTrendConfirmation => "تایید ترند صعودی EMA",
            DetailedSignalType.EmaBearishTrendConfirmation => "تایید ترند نزولی EMA",
            DetailedSignalType.EmaBullishBounce => "بازگشت صعودی EMA",
            DetailedSignalType.EmaBearishBounce => "بازگشت نزولی EMA",
            DetailedSignalType.EmaBullishBreakout => "شکست صعودی EMA",
            DetailedSignalType.EmaBearishBreakout => "شکست نزولی EMA",
            DetailedSignalType.EmaConvergence => "همگرایی EMA",
            DetailedSignalType.EmaDivergence => "واگرایی EMA",
            DetailedSignalType.EmaCrossAbove => "عبور EMA از بالا",
            DetailedSignalType.EmaCrossBelow => "عبور EMA از پایین",
            DetailedSignalType.EmaGoldenCross => "تقاطع طلایی EMA",
            DetailedSignalType.EmaDeathCross => "تقاطع مرگ EMA",

            // Additional ADX
            DetailedSignalType.ADXTrendStrengthIncreasing => "افزایش قدرت ترند",
            DetailedSignalType.ADXTrendStrengthDecreasing => "کاهش قدرت ترند",
            DetailedSignalType.ADXBullishTrend => "ترند صعودی",
            DetailedSignalType.ADXBearishTrend => "ترند نزولی",
            DetailedSignalType.AdxFalling => "کاهش ADX",
            DetailedSignalType.AdxDiPlusCrossAbove => "عبور DI+ از بالا",
            DetailedSignalType.AdxDiMinusCrossAbove => "عبور DI- از بالا",
            DetailedSignalType.AdxTrendStrength => "قدرت ترند ADX",
            DetailedSignalType.AdxWeakTrend => "ترند ضعیف ADX",
            DetailedSignalType.AdxRising => "افزایش ADX",

            // Additional CCI
            DetailedSignalType.CCIBullishDivergence => "واگرایی صعودی CCI",
            DetailedSignalType.CCIBearishDivergence => "واگرایی نزولی CCI",
            DetailedSignalType.CciZeroCrossBelow => "عبور CCI از زیر صفر",
            DetailedSignalType.CciOverbought => "اشباع خرید CCI",
            DetailedSignalType.CciOversold => "اشباع فروش CCI",
            DetailedSignalType.CciZeroCrossAbove => "عبور CCI از بالای صفر",

            // Candlestick Patterns
            DetailedSignalType.CandlestickDoji => "دوجی",
            DetailedSignalType.CandlestickHammer => "چکش",
            DetailedSignalType.CandlestickShootingStar => "ستاره دنباله‌دار",
            DetailedSignalType.CandlestickEngulfingBullish => "بلعیدن صعودی",
            DetailedSignalType.CandlestickEngulfingBearish => "بلعیدن نزولی",
            DetailedSignalType.CandlestickMorningStar => "ستاره صبح",
            DetailedSignalType.CandlestickEveningStar => "ستاره عصر",
            DetailedSignalType.CandlestickThreeWhiteSoldiers => "سه سرباز سفید",
            DetailedSignalType.CandlestickThreeBlackCrows => "سه کلاغ سیاه",
            DetailedSignalType.CandlestickBullishHarami => "الگوی حرامی صعودی",
            DetailedSignalType.CandlestickBearishHarami => "الگوی حرامی نزولی",

            _ => signalType.ToString()
        };
    }
}