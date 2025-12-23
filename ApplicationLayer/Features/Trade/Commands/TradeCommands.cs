using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.Dto.Trade;
using MediatR;

namespace ApplicationLayer.Features.Trade.Commands;

public record CreateTradeCommand(CreateTradeDto Model) : IRequest<HandlerResult>;
public record CloseTradeCommand(int TradeId) : IRequest<HandlerResult>;
public record UpdateTradeCommand(UpdateTradeDto Model) : IRequest<HandlerResult>;
public record CancelTradeCommand(int TradeId, CancelTradeDto Model) : IRequest<HandlerResult>;
