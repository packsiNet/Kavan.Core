using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.Dto.MarketAnalysis;
using ApplicationLayer.Extensions;
using ApplicationLayer.Features.MarketAnalysis.Query;
using ApplicationLayer.Interfaces;
using ApplicationLayer.Interfaces.Services;
using AutoMapper;
using MediatR;

namespace ApplicationLayer.Features.MarketAnalysis.Handler;

public class GetActiveSignalsHandler : IRequestHandler<GetActiveSignalsQuery, HandlerResult>
{
    private readonly ISignalRepository _signalRepository;
    private readonly IMapper _mapper;

    public GetActiveSignalsHandler(ISignalRepository signalRepository, IMapper mapper)
    {
        _signalRepository = signalRepository;
        _mapper = mapper;
    }

    public async Task<HandlerResult> Handle(GetActiveSignalsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var signals = await _signalRepository.GetActiveSignalsAsync(
                request.Symbol,
                request.Timeframe,
                request.SignalType,
                request.PageNumber,
                request.PageSize,
                cancellationToken);

            var signalDtos = _mapper.Map<List<TradingSignalDto>>(signals);
            
            return Result<List<TradingSignalDto>>.Success(signalDtos).ToHandlerResult();
        }
        catch (Exception ex)
        {
            return Result<List<TradingSignalDto>>.GeneralFailure($"Error retrieving signals: {ex.Message}").ToHandlerResult();
        }
    }
}