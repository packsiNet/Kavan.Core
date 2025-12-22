using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.Interfaces;
using ApplicationLayer.Interfaces.CustomApis;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace InfrastructureLayer.BusinessLogic.Services.CustomApis;

[InjectAsScoped]
public class CustomApisService(IUnitOfWork _unitOfWork,
    HttpClient _http,
    IRepository<Cryptocurrency> _cryptocurrencyRepo,
    IRepository<Candle_1m> _candle_1mRepo) : ICustomApisService
{
    public async Task<Result> IngestAsync(DateTime StartDateUtc)
    {
        var cryptos = await _cryptocurrencyRepo.Query().ToListAsync();

        foreach (var crypto in cryptos)
        {
            var endUtc = DateTime.UtcNow;

            while (StartDateUtc < endUtc)
            {
                var candles = await GetAsync(crypto.Id, crypto.Symbol, StartDateUtc, endUtc);

                if (candles.Count == 0)
                    break;

                foreach (var c in candles)
                    c.CryptocurrencyId = crypto.Id;

                await _candle_1mRepo.AddRangeAsync(candles);
                await _unitOfWork.SaveChangesAsync();

                StartDateUtc = candles.Last().OpenTime.AddMilliseconds(1);

                await Task.Delay(300);
            }

            await Task.Delay(300);
        }

        return Result.Success();
    }

    public async Task<List<Candle_1m>> GetAsync(int cid,
            string symbol,
            DateTime startUtc,
            DateTime endUtc)
    {
        var url =
            $"https://api.binance.com/api/v3/klines" +
            $"?symbol={symbol}" +
            $"&interval=1m" +
            $"&startTime={ToMs(startUtc)}" +
            $"&endTime={ToMs(endUtc)}" +
            $"&limit=1000";

        var json = await _http.GetStringAsync(url);
        var raw = JsonSerializer.Deserialize<List<List<JsonElement>>>(json);

        return raw.Select(x => new Candle_1m
        {
            CryptocurrencyId = cid,
            OpenTime = FromMs(x[0].GetInt64()),
            Open = decimal.Parse(x[1].GetString()!),
            High = decimal.Parse(x[2].GetString()!),
            Low = decimal.Parse(x[3].GetString()!),
            Close = decimal.Parse(x[4].GetString()!),
            Volume = decimal.Parse(x[5].GetString()!),
            CloseTime = FromMs(x[6].GetInt64()),
        }).ToList();
    }

    private static long ToMs(DateTime dt)
        => new DateTimeOffset(dt).ToUnixTimeMilliseconds();

    private static DateTime FromMs(long ms)
        => DateTimeOffset.FromUnixTimeMilliseconds(ms).UtcDateTime;
}