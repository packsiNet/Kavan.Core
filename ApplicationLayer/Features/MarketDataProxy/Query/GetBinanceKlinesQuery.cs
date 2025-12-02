using MediatR;

namespace ApplicationLayer.Features.MarketDataProxy.Query;

public record GetBinanceKlinesQuery(string Symbol, string Interval) : IRequest<HandlerResult>;

