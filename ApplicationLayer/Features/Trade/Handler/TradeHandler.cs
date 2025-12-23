using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.Dto.Trade;
using ApplicationLayer.Extensions;
using ApplicationLayer.Features.Trade.Commands;
using ApplicationLayer.Features.Trade.Query;
using ApplicationLayer.Interfaces.Services;
using MediatR;

namespace ApplicationLayer.Features.Trade.Handler;

public class TradeHandler(ITradeService _service) : 
    IRequestHandler<CreateTradeCommand, HandlerResult>,
    IRequestHandler<GetTradesByPeriodQuery, HandlerResult>,
    IRequestHandler<CloseTradeCommand, HandlerResult>,
    IRequestHandler<UpdateTradeCommand, HandlerResult>,
    IRequestHandler<CancelTradeCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(CreateTradeCommand request, CancellationToken cancellationToken)
    {
        var result = await _service.CreateTradeAsync(request.Model);
        return result.ToHandlerResult<TradeDto>();
    }

    public async Task<HandlerResult> Handle(GetTradesByPeriodQuery request, CancellationToken cancellationToken)
    {
        var result = await _service.GetTradesByPeriodAsync(request.PeriodId);
        return result.ToHandlerResult<List<TradeDto>>();
    }

    public async Task<HandlerResult> Handle(CloseTradeCommand request, CancellationToken cancellationToken)
    {
        var result = await _service.CloseTradeAsync(request.TradeId);
        return result.ToHandlerResult<TradeDto>();
    }

    public async Task<HandlerResult> Handle(UpdateTradeCommand request, CancellationToken cancellationToken)
    {
        var result = await _service.UpdateTradeAsync(request.Model);
        return result.ToHandlerResult<TradeDto>();
    }

    public async Task<HandlerResult> Handle(CancelTradeCommand request, CancellationToken cancellationToken)
    {
        var result = await _service.CancelTradeAsync(request.TradeId, request.Model.Reason);
        return result.ToHandlerResult<bool>();
    }
}
