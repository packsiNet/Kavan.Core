## هدف
- پیاده‌سازی کامل `AnalyzeAsync` برای پشتیبانی از هر نوع استراتژی/سیگنال کاربر (متنی یا ساختاری)، ارزیابی چند‌تایم‌فریمی و تولید خروجی دقیق شامل توضیحات، شرایط تطبیق‌یافته، امتیاز/اعتماد و ویژگی‌های سیگنال.

## محل و قرارداد
- سرویس: `InfrastructureLayer/BusinessLogic/Services/Signals/SignalAnalysisService.cs`
- اینترفیس: `ApplicationLayer/Interfaces/Services/Signals/ISignalAnalysisService.cs`
- امضا: `Task<IReadOnlyList<SignalResultDto>> AnalyzeAsync(SignalRequestDto request, CancellationToken cancellationToken)`

## ورودی/خروجی
- ورودی: `SignalRequestDto` شامل `Symbols`, `Timeframes`, `Conditions`, `Groups`, `Filters`, `Preferences` (+ پشتیبانی متن استراتژی به‌صورت اختیاری)
- خروجی: `IReadOnlyList<SignalResultDto>` شامل `Symbol`, `TimeFrame`, `Timestamp`, `SignalType`, `Strength`, `Confidence`, `Explanation`, `MatchedConditions`, `MatchedTypes`, `MatchedDetailedTypes`, `Attributes`

## طراحی سطح‌بالا AnalyzeAsync
1. دریافت و اعتبارسنجی ورودی: پر بودن `Symbols` یا قابلیت `ResolveSymbols`, وجود حداقل یک `Timeframe`, صحت `Groups/Conditions`, اعمال `Preferences`.
2. نگاشت متن استراتژی (اختیاری): اگر `StrategyText` در درخواست باشد، آن را با سرویس NLP به ساختار `Conditions/Groups` تبدیل می‌کنیم.
3. تعیین نمادها و تایم‌فریم‌ها: `ResolveSymbolsAsync` با لحاظ `Market/Exclude` و صحت وجود داده در جداول `Cryptocurrency` و `Candle_*`.
4. پیش‌بارگذاری دیتای کندل‌ها: برای هر نماد/تایم‌فریم پنجره کافی از کندل‌ها (`Candle_1m/5m/1h/4h/1d`) با `Task.WhenAll` و کمینه‌سازی کوئری‌ها.
5. اعمال فیلترها: `PassesFiltersAsync` شامل حداقل حجم/نوسان/رنج قیمت و ترتیب اعمال (`ApplyBefore`).
6. ارزیابی گروه‌ها و شرط‌ها: با `IConditionEvaluatorFactory` هر `ConditionNode` را روی تایم‌فریم مربوطه ارزیابی؛ پشتیبانی منطق `AND/OR`، توالی مراحل (Sequence) و وابستگی‌ها.
7. چند‌تایم‌فریم و توالی: ابتدا نواحی ماکرو (Daily/H4/H1: روند، حمایت/مقاومت، OrderBlock، FVG/BOS)، سپس میکرو‌تأییدها (M15/M5/M1: CHOCH، FVG داخلی، شکست سقف/کف، HL/LH، EMA50 پولبک).
8. ساخت سیگنال‌ها: در صورت موفقیت گروه‌ها، تولید `SignalResultDto` شامل نوع سیگنال، شدت، اعتماد، توضیح قابل‌فهم، محدوده‌های POI/FVG، قیمت‌های ورود/SL/TP (در صورت موجود).
9. رتبه‌بندی و جمع‌بندی: محاسبه `Strength/Confidence` بر اساس تعداد و نوع شرایط تطبیق‌یافته، تایم‌فریم‌ها، و کیفیت الگوها.
10. خروجی: لیست سیگنال‌ها با مرتب‌سازی/فیلتر نهایی مطابق `Preferences`.

## ارزیاب‌های شرط (Condition Evaluators)
- TrendEvaluator: تشخیص روند در H1/H4 با شیب/ساختار HL/LH یا EMA.
- SupportResistanceEvaluator: نواحی Demand/Supply/OrderBlock/FVG در Daily/H4 با تست‌های چندباره.
- BOSSwingBreakEvaluator: شکست ساختار (BOS) بر اساس Swing High/Low.
- CHOCHEvaluator: تغییر کاراکتر در M15/M5.
- FVGEvaluator: تشخیص FVG واضح، داخلی و کوچک؛ حدود، وسعت و اعتبار.
- OrderBlockEvaluator: بلوک نهایی قبل از حرکت؛ اعتبارسنجی با FVG/BOS.
- EMAEvaluator: پولبک عمیق تا EMA50 یا میانگین‌های مشابه.
- StructureSequenceEvaluator: الگوی «کف → سقف → اصلاح → شکست با قدرت» و ایجاد FVG داخلی.

## نمونه نگاشت از متن به ساختار
- مثال 2 (خلاصه):
  - Group(AND):
    - Condition(H1.Trend=Up)
    - Condition(H1.BOS=Up, EntryZone=Last RTO or FVG)
    - Condition(M15.CHOCH=Confirm)
    - Condition(M15.FVG=Confirm, Set POI)
    - Condition(M1.Sequence=Low→High→Pullback→StrongBreak + InternalFVG, Entry=Touch FVG)
    - Fallback: اگر CHOCH معکوس، رد سیگنال

## امتیازدهی و توضیحات خروجی
- Strength: وزن‌دهی به شروط ماکرو (Trend/Zone/BOS) و میکرو (CHOCH/FVG/Sequence).
- Confidence: کیفیت شکست (کندل‌های قوی، حجم)، وضوح FVG، تعداد تست‌های ناحیه، هم‌راستایی تایم‌فریم‌ها.
- Explanation: تولید متن فارسی خلاصه مراحل تطبیق و دلیل ورود/عدم ورود.

## عملکرد و مقیاس‌پذیری
- پیش‌واکشی دسته‌ای کندل‌ها، کش محلی در محدوده اجرای درخواست، کاهش ضربدری کوئری‌ها.
- ارزیابی موازی بر نمادها/تایم‌فریم‌ها با کنترل `CancellationToken`.
- محدودسازی پنجره داده بر اساس نیاز هر ارزیاب (مثلاً 400–2000 کندل).

## انطباق معماری
- استفاده از `IUnitOfWork` و `IRepository<TEntity>` برای `Cryptocurrency`, `Candle_1m`, `Candle_5m`, `Candle_1h`, `Candle_4h`, `Candle_1d`.
- تزریق سرویس تحلیل با `[InjectAsScoped]` و استفاده صرف از `IMediator` در کنترلر.
- تعریف انواع شرط‌ها با SmartEnum و نگاشت در `IConditionEvaluatorFactory`.

## گام‌های پیاده‌سازی جزئی در AnalyzeAsync
1. اعتبارسنجی ورودی و نرمال‌سازی `Symbols/Timeframes/Groups/Conditions`.
2. اگر متن استراتژی وجود دارد: فراخوانی Parser و ادغام با `Conditions/Groups`.
3. پیش‌بارگذاری داده کندل‌ها برای تمام جفت‌های نماد/تایم‌فریم.
4. اعمال فیلترها و حذف نمادهای نامعتبر در این فاز.
5. ارزیابی گروه‌های سطح‌بالا (ماکرو)؛ ذخیره POI/زون‌ها.
6. ارزیابی میکرو‌تأییدها با استفاده از خروجی ماکرو (وابستگی توالی).
7. ساخت `SignalResultDto`، محاسبه امتیاز/اعتماد و تولید `Explanation`.
8. مرتب‌سازی/محدودسازی نتایج طبق `Preferences` و بازگشت خروجی.

## تست و اعتبارسنجی
- تست واحد برای هر Evaluator با داده کندل نمونه.
- تست یکپارچه برای نمونه‌های 1–3 کاربر با نتیجه‌ی قابل‌توضیح.
- بررسی کارایی و حافظه برای چند نماد و تایم‌فریم.

## نتیجه
- با این پیاده‌سازی، `AnalyzeAsync` قادر خواهد بود هر استراتژی شخصی کاربر (متنی یا ساختاری) را به شکل دقیق در تایم‌فریم‌های مختلف ارزیابی و سیگنال‌های قابل‌اعتماد تولید کند. لطفاً تأیید کنید تا شروع به پیاده‌سازی کنیم.