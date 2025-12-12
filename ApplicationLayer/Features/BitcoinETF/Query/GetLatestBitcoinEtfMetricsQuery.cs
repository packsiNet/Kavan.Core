using MediatR;

namespace ApplicationLayer.Features.BitcoinETF.Query;

public record GetLatestBitcoinEtfMetricsQuery() : IRequest<HandlerResult>;
