using ApplicationLayer.Interfaces.Binance;

namespace ApplicationLayer.Services;

public class CandleUpdaterService : ICandleUpdaterService
{
    private readonly ICandleDataProvider _dataProvider;
    private readonly ICandleStorageService _storage;
    private readonly ICryptocurrencyRepository _repository;

    public CandleUpdaterService(ICandleDataProvider dataProvider, ICandleStorageService storage, ICryptocurrencyRepository repository)
    {
        _dataProvider = dataProvider;
        _storage = storage;
        _repository = repository;
    }

    public async Task UpdateCandlesAsync(string symbol, string interval, CancellationToken cancellationToken = default)
    {
        // 1️⃣ پیدا کردن ارز مربوطه از دیتابیس
        var crypto = await _repository.GetBySymbolAsync(symbol, cancellationToken);
        if (crypto == null)
            throw new InvalidOperationException($"Cryptocurrency with symbol '{symbol}' not found.");

        // 2️⃣ گرفتن داده‌ها از provider
        var candles = await _dataProvider.GetKlinesAsync(symbol, interval, limit: 1000);

        // 3️⃣ ذخیره در دیتابیس با cryptocurrencyId
        await _storage.SaveCandlesAsync(crypto.Id, interval, candles, cancellationToken);
    }
}