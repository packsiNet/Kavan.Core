using ApplicationLayer.DTOs.Watchlist;
using MediatR;

namespace ApplicationLayer.Features.Watchlist.Commands;

public record CreateWatchlistCommand(CreateWatchlistDto Model) : IRequest<HandlerResult>;