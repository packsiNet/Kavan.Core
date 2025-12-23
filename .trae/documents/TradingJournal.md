این مستندات کامل سیستم ژورنال معاملاتی شامل آدرس Endpointها، توضیحات، مدل‌های ورودی و نمونه‌های خروجی JSON است.

### 📚 مستندات API ژورنال معاملاتی (Trading Journal)
این سرویس‌ها برای مدیریت دوره‌های مالی، ثبت و مدیریت معاملات، و دریافت گزارش‌های تحلیلی طراحی شده‌اند.
 1️⃣ مدیریت دوره‌های مالی (Financial Periods)
قبل از ثبت هر معامله، باید یک دوره مالی فعال وجود داشته باشد.

📌 ایجاد دوره مالی جدید

- آدرس: POST /api/FinancialPeriod
- توضیحات: ایجاد یک دوره مالی جدید (مثلاً "دی ماه ۱۴۰۲") با موجودی اولیه.
- مدل ورودی:
{
  "title": "Trading January 2025",
  "startDate": "2025-01-01T00:00:00Z",
  "endDate": "2025-01-31T23:59:59Z",
  "initialBalance": 10000
}
- خروجی JSON:
{
  "id": 1,
  "userId": 101,
  "title": "Trading January 2025",
  "startDate": "2025-01-01T00:00:00Z",
  "endDate": "2025-01-31T23:59:59Z",
  "initialBalance": 10000,
  "currentBalance": 10000,
  "status": 1, // 1: Active, 2: Closed
  "statusName": "Active"
}

📌 بستن دوره مالی

- آدرس: POST /api/FinancialPeriod/{id}/close
- توضیحات: بستن یک دوره مالی و محاسبه سود/زیان نهایی.
- مدل ورودی: (بدون بدنه - ID در URL ارسال می‌شود)
- خروجی JSON: (مشابه مدل دوره مالی با Status: 2 و StatusName: Closed ) 2️⃣ مدیریت معاملات (Trade Management)
📌 ثبت معامله جدید

- آدرس: POST /api/Trade
- توضیحات: ثبت دستی یک معامله باز.
- مدل ورودی:
{
  "symbol": "BTCUSDT",
  "side": 1, // 1: Long, 2: Short
  "entryPrice": 45000.50,
  "quantity": 0.1,
  "stopLoss": 44000.00,
  "takeProfits": [
    { "price": 46000.00, "percentage": 50 },
    { "price": 48000.00, "percentage": 50 }
  ],
  "leverage": 10,
  "financialPeriodId": 1,
  "entryReason": "Breakout of resistance",
  "setupType": "Breakout",
  "emotion": {
    "emotionType": "Neutral",
    "note": "Feeling confident"
  }
}
- خروجی JSON:
{
  "id": 50,
  "symbol": "BTCUSDT",
  "side": 1,
  "sideName": "Long",
  "entryPrice": 45000.50,
  "quantity": 0.1,
  "status": 1, // 1: Open
  "statusName": "Open",
  "stopLoss": 44000.00,
  "takeProfits": [
    { "price": 46000.00, "isHit": false },
    { "price": 48000.00, "isHit": false }
  ],
  "openedAtUtc": "2025-01-15T10:30:00Z"
}

📌 بستن دستی معامله (Market Close)

- آدرس: POST /api/Trade/{id}/close
- توضیحات: بستن کامل معامله در قیمت لحظه‌ای بازار (قیمت از سرویس مارکت خوانده می‌شود).
- مدل ورودی: (بدون بدنه)
- خروجی JSON:

{
  "id": 50,
  "status": 2, // 2: Closed
  "statusName": "Closed",
  "closedAtUtc": "2025-01-15T14:00:00Z",
  "result": {
    "exitPrice": 45500.00,
    "pnlAmount": 50.00,
    "pnlPercent": 1.11,
    "exitReason": "ManualExit"
  }
}
📌 لغو معامله (Cancel)

- آدرس: POST /api/Trade/{id}/cancel
- توضیحات: لغو یک سفارش یا معامله (معمولاً قبل از ورود یا Pending).
- مدل ورودی:
{
  "reason": "Price action changed"
}
- خروجی JSON: (مشابه مدل معامله با Status: 4 و StatusName: Cancelled )
📌 ویرایش معامله

- آدرس: PUT /api/Trade
- توضیحات: به‌روزرسانی پارامترهای مدیریت ریسک یا روانشناسی معامله.
- مدل ورودی:
{
  "tradeId": 50,
  "stopLoss": 44500.00, // Trailing SL
  "confidenceLevel": 90,
  "emotionBeforeEntry": "Excited"
}
3️⃣ تحلیل و گزارش‌دهی (Analytics)
📌 خلاصه عملکرد دوره (Period Summary)

- آدرس: GET /api/TradingAnalytics/period/{periodId}/summary
- توضیحات: دریافت آمار کلی مثل Win Rate، Profit Factor و غیره.
- مدل ورودی: (پارامتر در URL)
- خروجی JSON:
{
  "financialPeriodId": 1,
  "winRate": 65.5,          // درصد برد
  "profitFactor": 2.1,      // نسبت سود به زیان
  "expectancy": 15.4,       // امید ریاضی هر معامله (دلار)
  "maxDrawdown": 120.50,    // حداکثر افت سرمایه
  "totalTrades": 20,
  "totalPnL": 350.00,
  "avgWin": 50.00,
  "avgLoss": 25.00
}
📌 بینش‌های معاملاتی (Insights)

- آدرس: GET /api/TradingAnalytics/period/{periodId}/insights
- توضیحات: دریافت تحلیل‌های هوشمند درباره رفتار معاملاتی.
- خروجی JSON:
{
  "strengths": [
    "High win rate on Long positions (75%)",
    "Good discipline in following Stop Loss"
  ],
  "weaknesses": [
    "Over-trading on weekends",
    "Average loss is higher than Average win on Short positions"
  ],
  "suggestions": [
    "Consider reducing leverage on Short trades",
    "Avoid trading during low volatility hours"
  ]
}
