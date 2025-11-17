using ApplicationLayer.Interfaces;
using ApplicationLayer.Interfaces.Services.Candles;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using InfrastructureLayer.Context;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureLayer.BusinessLogic.Services.Candles;

[InjectAsScoped]
public class CandlesQueryService(IRepository<Candle_1m> candle1m_repo, IRepository<Candle_5m> candle5m_repo, IRepository<Candle_1h> candle1h_repo, IRepository<Candle_4h> candle4h_repo, IRepository<Candle_1d> candle1d_repo) : ICandlesQueryService
{
    public async Task<List<CandleBase>> GetBeforeAsync(int cryptocurrencyId, string timeframe, DateTime pivotCloseTime, int count, CancellationToken ct = default)
    {
        switch (timeframe)
        {
            case "1m":
                {
                    var q = candle1m_repo.Query()
                        .Where(c => c.CryptocurrencyId == cryptocurrencyId && c.CloseTime < pivotCloseTime)
                        .OrderByDescending(c => c.CloseTime).AsQueryable();

                    if (count > 0) q = q.Take(count);
                    var list = await q.ToListAsync(ct);
                    return list.OrderBy(c => c.CloseTime).Cast<CandleBase>().ToList();
                }
            case "5m":
                {
                    var q = candle5m_repo.Query()
                        .Where(c => c.CryptocurrencyId == cryptocurrencyId && c.CloseTime < pivotCloseTime)
                        .OrderByDescending(c => c.CloseTime).AsQueryable();

                    if (count > 0) q = q.Take(count);
                    var list = await q.ToListAsync(ct);
                    return list.OrderBy(c => c.CloseTime).Cast<CandleBase>().ToList();
                }
            case "1h":
                {
                    var q = candle1h_repo.Query()
                        .Where(c => c.CryptocurrencyId == cryptocurrencyId && c.CloseTime < pivotCloseTime)
                        .OrderByDescending(c => c.CloseTime).AsQueryable();
                    if (count > 0) q = q.Take(count);
                    var list = await q.ToListAsync(ct);
                    return list.OrderBy(c => c.CloseTime).Cast<CandleBase>().ToList();
                }
            case "4h":
                {
                    var q = candle4h_repo.Query()
                        .Where(c => c.CryptocurrencyId == cryptocurrencyId && c.CloseTime < pivotCloseTime)
                        .OrderByDescending(c => c.CloseTime).AsQueryable();
                    if (count > 0) q = q.Take(count);
                    var list = await q.ToListAsync(ct);
                    return list.OrderBy(c => c.CloseTime).Cast<CandleBase>().ToList();
                }
            case "1d":
                {
                    var q = candle1d_repo.Query()
                        .Where(c => c.CryptocurrencyId == cryptocurrencyId && c.CloseTime < pivotCloseTime)
                        .OrderByDescending(c => c.CloseTime).AsQueryable();
                    if (count > 0) q = q.Take(count);
                    var list = await q.ToListAsync(ct);
                    return list.OrderBy(c => c.CloseTime).Cast<CandleBase>().ToList();
                }
            default:
                {
                    var q = candle1m_repo.Query()
                        .Where(c => c.CryptocurrencyId == cryptocurrencyId && c.CloseTime < pivotCloseTime)
                        .OrderByDescending(c => c.CloseTime).AsQueryable();
                    if (count > 0) q = q.Take(count);
                    var list = await q.ToListAsync(ct);
                    return list.OrderBy(c => c.CloseTime).Cast<CandleBase>().ToList();
                }
        }
    }

    public async Task<List<CandleBase>> GetAfterAsync(int cryptocurrencyId, string timeframe, DateTime pivotCloseTime, int count, CancellationToken ct = default)
    {
        switch (timeframe)
        {
            case "1m":
                {
                    var q = candle1m_repo.Query()
                        .Where(c => c.CryptocurrencyId == cryptocurrencyId && c.CloseTime > pivotCloseTime)
                        .OrderBy(c => c.CloseTime).AsQueryable();

                    if (count > 0) q = q.Take(count);
                    var list = await q.ToListAsync(ct);
                    return list.Cast<CandleBase>().ToList();
                }
            case "5m":
                {
                    var q = candle5m_repo.Query()
                        .Where(c => c.CryptocurrencyId == cryptocurrencyId && c.CloseTime > pivotCloseTime)
                        .OrderBy(c => c.CloseTime).AsQueryable();
                    if (count > 0) q = q.Take(count);
                    var list = await q.ToListAsync(ct);
                    return list.Cast<CandleBase>().ToList();
                }
            case "1h":
                {
                    var q = candle1h_repo.Query()
                        .Where(c => c.CryptocurrencyId == cryptocurrencyId && c.CloseTime > pivotCloseTime)
                        .OrderBy(c => c.CloseTime).AsQueryable();
                    if (count > 0) q = q.Take(count);
                    var list = await q.ToListAsync(ct);
                    return list.Cast<CandleBase>().ToList();
                }
            case "4h":
                {
                    var q = candle4h_repo.Query()
                        .Where(c => c.CryptocurrencyId == cryptocurrencyId && c.CloseTime > pivotCloseTime)
                        .OrderBy(c => c.CloseTime).AsQueryable();
                    if (count > 0) q = q.Take(count);
                    var list = await q.ToListAsync(ct);
                    return list.Cast<CandleBase>().ToList();
                }
            case "1d":
                {
                    var q = candle1d_repo.Query()
                        .Where(c => c.CryptocurrencyId == cryptocurrencyId && c.CloseTime > pivotCloseTime)
                        .OrderBy(c => c.CloseTime).AsQueryable();
                    if (count > 0) q = q.Take(count);
                    var list = await q.ToListAsync(ct);
                    return list.Cast<CandleBase>().ToList();
                }
            default:
                {
                    var q = candle1m_repo.Query()
                        .Where(c => c.CryptocurrencyId == cryptocurrencyId && c.CloseTime > pivotCloseTime)
                        .OrderBy(c => c.CloseTime).AsQueryable();
                    if (count > 0) q = q.Take(count);
                    var list = await q.ToListAsync(ct);
                    return list.Cast<CandleBase>().ToList();
                }
        }
    }
}