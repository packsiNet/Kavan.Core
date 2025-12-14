using MediatR;

namespace ApplicationLayer.Features.BitcoinETF.Query;

public record GetLatestBitcoinUserCountQuery() : IRequest<HandlerResult>;
