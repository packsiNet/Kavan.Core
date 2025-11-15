using ApplicationLayer.Features.SignalsCatalog.Commands;
using ApplicationLayer.Interfaces;
using DomainLayer.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApplicationLayer.Features.SignalsCatalog.Handler
{
    public class SeedSignalCatalogHandler : IRequestHandler<SeedSignalCatalogCommand, bool>
    {
        private readonly IUnitOfWork _uow;
        private readonly IRepository<SignalCatalogNode> _repo;

        public SeedSignalCatalogHandler(IUnitOfWork uow, IRepository<SignalCatalogNode> repo)
        {
            _uow = uow;
            _repo = repo;
        }

        public async Task<bool> Handle(SeedSignalCatalogCommand request, CancellationToken cancellationToken)
        {
            var any = await _repo.Query().AnyAsync(cancellationToken);
            if (any) return true;

            var nodes = new List<SignalCatalogNode>();

            var candlestick = new SignalCatalogNode { NameFa = "الگوهای کندلستیکی", NameEn = "Candlestick Patterns", Kind = "Category", Category = "Candlestick" };
            nodes.Add(candlestick);
            nodes.Add(new SignalCatalogNode { Parent = candlestick, NameFa = "چکش", NameEn = "Hammer", Kind = "Signal", Category = "Candlestick", SignalName = "BullishHammer" });
            nodes.Add(new SignalCatalogNode { Parent = candlestick, NameFa = "پوشای صعودی", NameEn = "Bullish Engulfing", Kind = "Signal", Category = "Candlestick", SignalName = "BullishEngulfing" });
            nodes.Add(new SignalCatalogNode { Parent = candlestick, NameFa = "سه سرباز سفید", NameEn = "Three White Soldiers", Kind = "Signal", Category = "Candlestick", SignalName = "ThreeWhiteSoldiers" });
            nodes.Add(new SignalCatalogNode { Parent = candlestick, NameFa = "ستاره صبحگاهی", NameEn = "Morning Star", Kind = "Signal", Category = "Candlestick", SignalName = "MorningStar" });
            nodes.Add(new SignalCatalogNode { Parent = candlestick, NameFa = "شوتینگ استار", NameEn = "Shooting Star", Kind = "Signal", Category = "Candlestick", SignalName = "ShootingStar" });
            nodes.Add(new SignalCatalogNode { Parent = candlestick, NameFa = "پوشای نزولی", NameEn = "Bearish Engulfing", Kind = "Signal", Category = "Candlestick", SignalName = "BearishEngulfing" });
            nodes.Add(new SignalCatalogNode { Parent = candlestick, NameFa = "سه کلاغ سیاه", NameEn = "Three Black Crows", Kind = "Signal", Category = "Candlestick", SignalName = "ThreeBlackCrows" });
            nodes.Add(new SignalCatalogNode { Parent = candlestick, NameFa = "مرد دارآویز", NameEn = "Hanging Man", Kind = "Signal", Category = "Candlestick", SignalName = "HangingMan" });
            nodes.Add(new SignalCatalogNode { Parent = candlestick, NameFa = "ستاره شامگاهی", NameEn = "Evening Star", Kind = "Signal", Category = "Candlestick", SignalName = "EveningStar" });
            nodes.Add(new SignalCatalogNode { Parent = candlestick, NameFa = "دوجی", NameEn = "Doji", Kind = "Signal", Category = "Candlestick", SignalName = "Doji" });

            var ichimoku = new SignalCatalogNode { NameFa = "ایچیموکو", NameEn = "Ichimoku", Kind = "Category", Category = "Ichimoku" };
            nodes.Add(ichimoku);
            nodes.Add(new SignalCatalogNode { Parent = ichimoku, NameFa = "قیمت بالای ابر", NameEn = "Price Above Cloud", Kind = "Signal", Category = "Ichimoku", SignalName = "IchimokuPriceAboveCloud" });
            nodes.Add(new SignalCatalogNode { Parent = ichimoku, NameFa = "قیمت پایین ابر", NameEn = "Price Below Cloud", Kind = "Signal", Category = "Ichimoku", SignalName = "IchimokuPriceBelowCloud" });
            nodes.Add(new SignalCatalogNode { Parent = ichimoku, NameFa = "کراس صعودی تنکان/کیجون", NameEn = "Bullish Tenkan/Kijun Cross", Kind = "Signal", Category = "Ichimoku", SignalName = "IchimokuTenkanKijunBullishCross" });
            nodes.Add(new SignalCatalogNode { Parent = ichimoku, NameFa = "کراس نزولی تنکان/کیجون", NameEn = "Bearish Tenkan/Kijun Cross", Kind = "Signal", Category = "Ichimoku", SignalName = "IchimokuTenkanKijunBearishCross" });
            nodes.Add(new SignalCatalogNode { Parent = ichimoku, NameFa = "ابر سبز", NameEn = "Green Kumo", Kind = "Signal", Category = "Ichimoku", SignalName = "IchimokuGreenKumo" });
            nodes.Add(new SignalCatalogNode { Parent = ichimoku, NameFa = "ابر قرمز", NameEn = "Red Kumo", Kind = "Signal", Category = "Ichimoku", SignalName = "IchimokuRedKumo" });
            nodes.Add(new SignalCatalogNode { Parent = ichimoku, NameFa = "چیکو بالای قیمت", NameEn = "Chikou Above Price", Kind = "Signal", Category = "Ichimoku", SignalName = "IchimokuChikouAbovePrice" });
            nodes.Add(new SignalCatalogNode { Parent = ichimoku, NameFa = "چیکو پایین قیمت", NameEn = "Chikou Below Price", Kind = "Signal", Category = "Ichimoku", SignalName = "IchimokuChikouBelowPrice" });

            var sma = new SignalCatalogNode { NameFa = "میانگین متحرک ساده", NameEn = "SMA", Kind = "Category", Category = "SMA" };
            nodes.Add(sma);
            nodes.Add(new SignalCatalogNode { Parent = sma, NameFa = "قیمت بالای SMA10", NameEn = "Price Above SMA10", Kind = "Signal", Category = "SMA", SignalName = "PriceAboveSMA10" });
            nodes.Add(new SignalCatalogNode { Parent = sma, NameFa = "قیمت پایین SMA10", NameEn = "Price Below SMA10", Kind = "Signal", Category = "SMA", SignalName = "PriceBelowSMA10" });
            nodes.Add(new SignalCatalogNode { Parent = sma, NameFa = "قیمت بالای SMA50", NameEn = "Price Above SMA50", Kind = "Signal", Category = "SMA", SignalName = "PriceAboveSMA50" });
            nodes.Add(new SignalCatalogNode { Parent = sma, NameFa = "قیمت پایین SMA50", NameEn = "Price Below SMA50", Kind = "Signal", Category = "SMA", SignalName = "PriceBelowSMA50" });
            nodes.Add(new SignalCatalogNode { Parent = sma, NameFa = "قیمت بالای SMA200", NameEn = "Price Above SMA200", Kind = "Signal", Category = "SMA", SignalName = "PriceAboveSMA200" });
            nodes.Add(new SignalCatalogNode { Parent = sma, NameFa = "قیمت پایین SMA200", NameEn = "Price Below SMA200", Kind = "Signal", Category = "SMA", SignalName = "PriceBelowSMA200" });
            nodes.Add(new SignalCatalogNode { Parent = sma, NameFa = "شکست SMA20 رو به بالا", NameEn = "SMA20 Breakout Up", Kind = "Signal", Category = "SMA", SignalName = "SMA20BreakoutUp" });
            nodes.Add(new SignalCatalogNode { Parent = sma, NameFa = "شکست SMA20 رو به پایین", NameEn = "SMA20 Breakout Down", Kind = "Signal", Category = "SMA", SignalName = "SMA20BreakoutDown" });
            nodes.Add(new SignalCatalogNode { Parent = sma, NameFa = "شکست SMA100 رو به بالا", NameEn = "SMA100 Breakout Up", Kind = "Signal", Category = "SMA", SignalName = "SMA100BreakoutUp" });
            nodes.Add(new SignalCatalogNode { Parent = sma, NameFa = "شکست SMA100 رو به پایین", NameEn = "SMA100 Breakout Down", Kind = "Signal", Category = "SMA", SignalName = "SMA100BreakoutDown" });
            nodes.Add(new SignalCatalogNode { Parent = sma, NameFa = "کراس صعودی SMA50/200", NameEn = "SMA50/200 Bullish Cross", Kind = "Signal", Category = "SMA", SignalName = "SMA50_200BullishCross" });
            nodes.Add(new SignalCatalogNode { Parent = sma, NameFa = "کراس نزولی SMA50/200", NameEn = "SMA50/200 Bearish Cross", Kind = "Signal", Category = "SMA", SignalName = "SMA50_200BearishCross" });

            var ema = new SignalCatalogNode { NameFa = "میانگین متحرک نمایی", NameEn = "EMA", Kind = "Category", Category = "EMA" };
            nodes.Add(ema);
            nodes.Add(new SignalCatalogNode { Parent = ema, NameFa = "قیمت بالای EMA10", NameEn = "Price Above EMA10", Kind = "Signal", Category = "EMA", SignalName = "PriceAboveEMA10" });
            nodes.Add(new SignalCatalogNode { Parent = ema, NameFa = "قیمت پایین EMA10", NameEn = "Price Below EMA10", Kind = "Signal", Category = "EMA", SignalName = "PriceBelowEMA10" });
            nodes.Add(new SignalCatalogNode { Parent = ema, NameFa = "قیمت بالای EMA50", NameEn = "Price Above EMA50", Kind = "Signal", Category = "EMA", SignalName = "PriceAboveEMA50" });
            nodes.Add(new SignalCatalogNode { Parent = ema, NameFa = "قیمت پایین EMA50", NameEn = "Price Below EMA50", Kind = "Signal", Category = "EMA", SignalName = "PriceBelowEMA50" });
            nodes.Add(new SignalCatalogNode { Parent = ema, NameFa = "قیمت بالای EMA200", NameEn = "Price Above EMA200", Kind = "Signal", Category = "EMA", SignalName = "PriceAboveEMA200" });
            nodes.Add(new SignalCatalogNode { Parent = ema, NameFa = "قیمت پایین EMA200", NameEn = "Price Below EMA200", Kind = "Signal", Category = "EMA", SignalName = "PriceBelowEMA200" });
            nodes.Add(new SignalCatalogNode { Parent = ema, NameFa = "شکست EMA20 رو به بالا", NameEn = "EMA20 Breakout Up", Kind = "Signal", Category = "EMA", SignalName = "EMA20BreakoutUp" });
            nodes.Add(new SignalCatalogNode { Parent = ema, NameFa = "شکست EMA20 رو به پایین", NameEn = "EMA20 Breakout Down", Kind = "Signal", Category = "EMA", SignalName = "EMA20BreakoutDown" });
            nodes.Add(new SignalCatalogNode { Parent = ema, NameFa = "شکست EMA100 رو به بالا", NameEn = "EMA100 Breakout Up", Kind = "Signal", Category = "EMA", SignalName = "EMA100BreakoutUp" });
            nodes.Add(new SignalCatalogNode { Parent = ema, NameFa = "شکست EMA100 رو به پایین", NameEn = "EMA100 Breakout Down", Kind = "Signal", Category = "EMA", SignalName = "EMA100BreakoutDown" });
            nodes.Add(new SignalCatalogNode { Parent = ema, NameFa = "کراس صعودی EMA50/200", NameEn = "EMA50/200 Bullish Cross", Kind = "Signal", Category = "EMA", SignalName = "EMA50_200BullishCross" });
            nodes.Add(new SignalCatalogNode { Parent = ema, NameFa = "کراس نزولی EMA50/200", NameEn = "EMA50/200 Bearish Cross", Kind = "Signal", Category = "EMA", SignalName = "EMA50_200BearishCross" });

            var boll = new SignalCatalogNode { NameFa = "باندهای بولینگر", NameEn = "Bollinger Bands", Kind = "Category", Category = "Bollinger" };
            nodes.Add(boll);
            nodes.Add(new SignalCatalogNode { Parent = boll, NameFa = "قیمت بالای باند بالایی", NameEn = "Price Above Upper Band", Kind = "Signal", Category = "Bollinger", SignalName = "PriceAboveBollingerUpper" });
            nodes.Add(new SignalCatalogNode { Parent = boll, NameFa = "قیمت پایین باند پایینی", NameEn = "Price Below Lower Band", Kind = "Signal", Category = "Bollinger", SignalName = "PriceBelowBollingerLower" });
            nodes.Add(new SignalCatalogNode { Parent = boll, NameFa = "شکست باند بالایی", NameEn = "Upper Band Breakout", Kind = "Signal", Category = "Bollinger", SignalName = "BollingerUpperBreakout" });
            nodes.Add(new SignalCatalogNode { Parent = boll, NameFa = "شکست باند پایینی", NameEn = "Lower Band Breakout", Kind = "Signal", Category = "Bollinger", SignalName = "BollingerLowerBreakout" });
            nodes.Add(new SignalCatalogNode { Parent = boll, NameFa = "فشردگی باندها", NameEn = "Band Squeeze", Kind = "Signal", Category = "Bollinger", SignalName = "BollingerSqueeze" });

            var rsi = new SignalCatalogNode { NameFa = "شاخص RSI", NameEn = "RSI", Kind = "Category", Category = "RSI" };
            nodes.Add(rsi);
            nodes.Add(new SignalCatalogNode { Parent = rsi, NameFa = "اشباع خرید بالای 70", NameEn = "Overbought >70", Kind = "Signal", Category = "RSI", SignalName = "RSIOverbought70" });
            nodes.Add(new SignalCatalogNode { Parent = rsi, NameFa = "اشباع فروش زیر 30", NameEn = "Oversold <30", Kind = "Signal", Category = "RSI", SignalName = "RSIOversold30" });
            nodes.Add(new SignalCatalogNode { Parent = rsi, NameFa = "عبور از 50 به بالا", NameEn = "Cross Above 50", Kind = "Signal", Category = "RSI", SignalName = "RSICrossAbove50" });
            nodes.Add(new SignalCatalogNode { Parent = rsi, NameFa = "عبور از 50 به پایین", NameEn = "Cross Below 50", Kind = "Signal", Category = "RSI", SignalName = "RSICrossBelow50" });
            nodes.Add(new SignalCatalogNode { Parent = rsi, NameFa = "واگرایی مثبت", NameEn = "Bullish Divergence", Kind = "Signal", Category = "RSI", SignalName = "RSIBullishDivergence" });
            nodes.Add(new SignalCatalogNode { Parent = rsi, NameFa = "واگرایی منفی", NameEn = "Bearish Divergence", Kind = "Signal", Category = "RSI", SignalName = "RSIBearishDivergence" });

            var adx = new SignalCatalogNode { NameFa = "شاخص ADX/DI", NameEn = "ADX/DI", Kind = "Category", Category = "ADX" };
            nodes.Add(adx);
            nodes.Add(new SignalCatalogNode { Parent = adx, NameFa = "ADX بالای 25", NameEn = "ADX Above 25", Kind = "Signal", Category = "ADX", SignalName = "ADXAboveThreshold" });
            nodes.Add(new SignalCatalogNode { Parent = adx, NameFa = "ADX پایین 20", NameEn = "ADX Below 20", Kind = "Signal", Category = "ADX", SignalName = "ADXBelowThreshold" });
            nodes.Add(new SignalCatalogNode { Parent = adx, NameFa = "DI+ بالای DI-", NameEn = "+DI Above -DI", Kind = "Signal", Category = "ADX", SignalName = "DIPlusAboveDIMinus" });
            nodes.Add(new SignalCatalogNode { Parent = adx, NameFa = "DI- بالای DI+", NameEn = "-DI Above +DI", Kind = "Signal", Category = "ADX", SignalName = "DIMinusAboveDIPlus" });

            var cci = new SignalCatalogNode { NameFa = "شاخص CCI", NameEn = "CCI", Kind = "Category", Category = "CCI" };
            nodes.Add(cci);
            nodes.Add(new SignalCatalogNode { Parent = cci, NameFa = "بالاتر از +100", NameEn = "> +100", Kind = "Signal", Category = "CCI", SignalName = "CCIAbove100" });
            nodes.Add(new SignalCatalogNode { Parent = cci, NameFa = "پایین‌تر از −100", NameEn = "< −100", Kind = "Signal", Category = "CCI", SignalName = "CCIBelowMinus100" });
            nodes.Add(new SignalCatalogNode { Parent = cci, NameFa = "عبور از صفر به بالا", NameEn = "Cross Above Zero", Kind = "Signal", Category = "CCI", SignalName = "CCICrossAboveZero" });
            nodes.Add(new SignalCatalogNode { Parent = cci, NameFa = "عبور از صفر به پایین", NameEn = "Cross Below Zero", Kind = "Signal", Category = "CCI", SignalName = "CCICrossBelowZero" });

            await _repo.AddRangeAsync(nodes);
            await _uow.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}