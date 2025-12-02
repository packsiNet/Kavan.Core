using MediatR;

namespace ApplicationLayer.Features.MarketDataProxy.Query;

public record GetCoinbaseCandlesQuery(string Symbol, int Granularity) : IRequest<HandlerResult>;

