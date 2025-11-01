using ApplicationLayer.Interfaces.Binance;
using ApplicationLayer.Interfaces;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApplicationLayer.Services;

public class CandleUpdaterService : ICandleUpdaterService
{
    private readonly ICandleDataProvider _dataProvider;
    private readonly ICandleStorageService _storage;
    private readonly IRepository<Cryptocurrency> _repository;

    public CandleUpdaterService(ICandleDataProvider dataProvider, ICandleStorageService storage, IRepository<Cryptocurrency> repository)
    {
        _dataProvider = dataProvider;
        _storage = storage;
        _repository = repository;
    }

    public async Task UpdateCandlesAsync(string symbol, string interval, CancellationToken cancellationToken = default)
    {
        // 1️⃣ پیدا کردن ارز مربوطه از دیتابیس
        var crypto = await _repository.Query().FirstOrDefaultAsync(x => x.Symbol == symbol, cancellationToken);
        if (crypto == null)
            throw new InvalidOperationException($"Cryptocurrency with symbol '{symbol}' not found.");

        // 2️⃣ گرفتن فقط یک رکورد (آخرین کندل) از provider
        var candles = await _dataProvider.GetKlinesAsync(symbol, interval, limit: 1);

        // اطمینان از ذخیره فقط یک رکورد در هر بار اجرا
        candles = candles
            .OrderByDescending(c => c.OpenTime)
            .Take(1)
            .ToList();

        // 3️⃣ ذخیره در دیتابیس با cryptocurrencyId
        await _storage.SaveCandlesAsync(crypto.Id, interval, candles, cancellationToken);
    }
}