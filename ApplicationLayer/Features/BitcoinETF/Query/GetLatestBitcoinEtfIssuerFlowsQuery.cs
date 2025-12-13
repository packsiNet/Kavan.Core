using ApplicationLayer;
using MediatR;

namespace ApplicationLayer.Features.BitcoinETF.Query;

public record GetLatestBitcoinEtfIssuerFlowsQuery() : IRequest<HandlerResult>;
