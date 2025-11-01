using ApplicationLayer.Dto.MarketAnalysis;
using ApplicationLayer.Features.MarketAnalysis.Query;
using ApplicationLayer.Interfaces;
using ApplicationLayer.Interfaces.Services;
using AutoMapper;
using MediatR;

namespace ApplicationLayer.Features.MarketAnalysis.Handler;

public class GetActiveSignalsHandler : IRequestHandler<GetActiveSignalsQuery, List<TradingSignalDto>>
{
    private readonly ISignalRepository _signalRepository;
    private readonly IMapper _mapper;

    public GetActiveSignalsHandler(ISignalRepository signalRepository, IMapper mapper)
    {
        _signalRepository = signalRepository;
        _mapper = mapper;
    }

    public async Task<List<TradingSignalDto>> Handle(GetActiveSignalsQuery request, CancellationToken cancellationToken)
    {
        var signals = await _signalRepository.GetActiveSignalsAsync(
            request.Symbol,
            request.Timeframe,
            request.SignalType,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        return _mapper.Map<List<TradingSignalDto>>(signals);
    }
}