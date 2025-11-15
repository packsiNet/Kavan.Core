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
        IRepository<SignalCandle> _candles)
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
                        Volume = log.CandleVolume
                    },
                    Candles = candleList.Select(c => new CandleDto
                    {
                        Index = c.Index,
                        OpenTime = c.OpenTime,
                        CloseTime = c.CloseTime,
                        Open = c.Open,
                        High = c.High,
                        Low = c.Low,
                        Close = c.Close,
                        Volume = c.Volume
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