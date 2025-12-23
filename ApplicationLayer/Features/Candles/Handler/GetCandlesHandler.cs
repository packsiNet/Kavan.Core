using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.Dto.Candle;
using ApplicationLayer.Extensions;
using ApplicationLayer.Features.Candles.Query;
using ApplicationLayer.Interfaces;
using ApplicationLayer.Interfaces.Services.Candles;
using DomainLayer.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApplicationLayer.Features.Candles.Handler;

public class GetCandlesHandler(
    IRepository<Cryptocurrency> _cryptoRepo,
    ICandlesQueryService _candlesService) 
    : IRequestHandler<GetCandlesQuery, HandlerResult>
{
    public async Task<HandlerResult> Handle(GetCandlesQuery request, CancellationToken cancellationToken)
    {
        var exists = await _cryptoRepo.Query()
            .AsNoTracking()
            .AnyAsync(c => c.Id == request.CryptocurrencyId, cancellationToken);

        if (!exists)
            return Result.NotFound($"Cryptocurrency with ID '{request.CryptocurrencyId}' not found.").ToHandlerResult();

        // Get candles before NOW (latest candles)
        var candles = await _candlesService.GetBeforeAsync(
            request.CryptocurrencyId, 
            request.Timeframe, 
            DateTime.UtcNow, 
            request.Limit, 
            cancellationToken);

        var dtos = candles.Select(c => new CandleDto(
            new DateTimeOffset(c.OpenTime).ToUnixTimeMilliseconds(),
            new DateTimeOffset(c.CloseTime).ToUnixTimeMilliseconds(),
            c.Open,
            c.High,
            c.Low,
            c.Close,
            c.Volume,
            c.IsFinal
        )).ToList();

        return Result<List<CandleDto>>.Success(dtos).ToHandlerResult();
    }
}
