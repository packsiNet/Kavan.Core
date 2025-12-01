using MediatR;

namespace ApplicationLayer.Features.Watchlist.Commands;

public record DeleteWatchlistCommand(int Id) : IRequest<HandlerResult>;