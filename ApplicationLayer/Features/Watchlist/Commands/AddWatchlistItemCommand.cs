using MediatR;

namespace ApplicationLayer.Features.Watchlist.Commands;

public record AddWatchlistItemCommand(int WatchlistId, string Symbol) : IRequest<HandlerResult>;