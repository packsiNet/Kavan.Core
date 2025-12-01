using MediatR;

namespace ApplicationLayer.Features.Watchlist.Commands;

public record RemoveWatchlistItemCommand(int ItemId) : IRequest<HandlerResult>;