using MediatR;

namespace ApplicationLayer.Features.BitcoinETF.Query;

public record GetBitcoinUserCountsQuery() : IRequest<HandlerResult>;
