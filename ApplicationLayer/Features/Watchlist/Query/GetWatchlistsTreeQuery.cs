using MediatR;

namespace ApplicationLayer.Features.Watchlist.Query;

public record GetWatchlistsTreeQuery() : IRequest<HandlerResult>;