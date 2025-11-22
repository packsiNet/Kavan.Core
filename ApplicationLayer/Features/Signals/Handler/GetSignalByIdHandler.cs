using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.Dto.Signals;
using ApplicationLayer.Extensions;
using ApplicationLayer.Features.Signals.Query;
using ApplicationLayer.Interfaces;
using DomainLayer.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApplicationLayer.Features.Signals.Handler
{
    public class GetSignalByIdHandler(
        IRepository<Signal> _signals,
        IRepository<SignalCandle> _candles,
        ApplicationLayer.Interfaces.Services.Candles.ICandlesQueryService _candlesQuery)
        : IRequestHandler<GetSignalByIdQuery, HandlerResult>
    {
        public async Task<HandlerResult> Handle(GetSignalByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var log = await _signals.Query()
                    .Where(x => x.Id == request.Id)
                    .FirstOrDefaultAsync(cancellationToken);

                if (log == null)
                    return Result.NotFound("سیگنال یافت نشد").ToHandlerResult();

            var candleList = await _candles.Query()
                .Where(c => c.SignalId == request.Id)
                .OrderBy(c => c.Index)
                .ToListAsync(cancellationToken);

            var before = await _candlesQuery.GetBeforeAsync(log.CryptocurrencyId, log.Timeframe, log.CandleCloseTime, 0, cancellationToken);
            var after = await _candlesQuery.GetAfterAsync(log.CryptocurrencyId, log.Timeframe, log.CandleCloseTime, 0, cancellationToken);

            var map = new Dictionary<long, ApplicationLayer.Dto.Signals.CandleDto>();

            foreach (var c in before)
            {
                var key = c.CloseTime.Ticks;
                if (!map.ContainsKey(key))
                {
                    map[key] = new ApplicationLayer.Dto.Signals.CandleDto
                    {
                        OpenTime = c.OpenTime,
                        CloseTime = c.CloseTime,
                        Open = c.Open,
                        High = c.High,
                        Low = c.Low,
                        Close = c.Close,
                        Volume = c.Volume,
                        IsTrigger = false
                    };
                }
            }

            foreach (var sc in candleList)
            {
                var key = sc.CloseTime.Ticks;
                map[key] = new ApplicationLayer.Dto.Signals.CandleDto
                {
                    OpenTime = sc.OpenTime,
                    CloseTime = sc.CloseTime,
                    Open = sc.Open,
                    High = sc.High,
                    Low = sc.Low,
                    Close = sc.Close,
                    Volume = sc.Volume,
                    IsTrigger = sc.IsTrigger
                };
            }

            foreach (var c in after)
            {
                var key = c.CloseTime.Ticks;
                if (!map.ContainsKey(key))
                {
                    map[key] = new ApplicationLayer.Dto.Signals.CandleDto
                    {
                        OpenTime = c.OpenTime,
                        CloseTime = c.CloseTime,
                        Open = c.Open,
                        High = c.High,
                        Low = c.Low,
                        Close = c.Close,
                        Volume = c.Volume,
                        IsTrigger = false
                    };
                }
            }

            if (!map.ContainsKey(log.CandleCloseTime.Ticks))
            {
                map[log.CandleCloseTime.Ticks] = new ApplicationLayer.Dto.Signals.CandleDto
                {
                    OpenTime = log.CandleOpenTime,
                    CloseTime = log.CandleCloseTime,
                    Open = log.CandleOpen,
                    High = log.CandleHigh,
                    Low = log.CandleLow,
                    Close = log.CandleClose,
                    Volume = log.CandleVolume,
                    IsTrigger = true
                };
            }

            var combined = map.Values.OrderBy(c => c.CloseTime).ToList();

                var dto = new SignalDto
                {
                    Id = log.Id,
                    Symbol = log.Symbol,
                    Timeframe = log.Timeframe,
                    SignalTime = log.SignalTime,
                    SignalCategory = log.SignalCategory,
                    SignalName = log.SignalName,
                    Direction = log.Direction,
                    BreakoutLevel = log.BreakoutLevel,
                    NearestResistance = log.NearestResistance,
                    NearestSupport = log.NearestSupport,
                    PivotR1 = log.PivotR1,
                    PivotR2 = log.PivotR2,
                    PivotR3 = log.PivotR3,
                    PivotS1 = log.PivotS1,
                    PivotS2 = log.PivotS2,
                    PivotS3 = log.PivotS3,
                    Atr = log.Atr,
                    Tolerance = log.Tolerance,
                    VolumeRatio = log.VolumeRatio,
                    BodySize = log.BodySize,
                    LastCandle = new CandleDto
                    {
                        Index = candleList.Count > 0 ? candleList.Count - 1 : 0,
                        OpenTime = log.CandleOpenTime,
                        CloseTime = log.CandleCloseTime,
                        Open = log.CandleOpen,
                        High = log.CandleHigh,
                        Low = log.CandleLow,
                        Close = log.CandleClose,
                        Volume = log.CandleVolume,
                        IsTrigger = true
                    },
                    Candles = combined.Select((c, idx) => new CandleDto
                    {
                        Index = idx,
                        OpenTime = c.OpenTime,
                        CloseTime = c.CloseTime,
                        Open = c.Open,
                        High = c.High,
                        Low = c.Low,
                        Close = c.Close,
                        Volume = c.Volume,
                        IsTrigger = c.IsTrigger
                    }).ToList()
                };

                return Result<SignalDto>.Success(dto).ToHandlerResult();
            }
            catch (Exception ex)
            {
                return Result.GeneralFailure($"بازیابی سیگنال با خطا مواجه شد: {ex.Message}").ToHandlerResult();
            }
        }
    }
}