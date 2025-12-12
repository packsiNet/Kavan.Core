using MediatR;

namespace ApplicationLayer.Features.BitcoinETF.Query;

public record GetLatestBitcoinTxCountQuery() : IRequest<HandlerResult>;
