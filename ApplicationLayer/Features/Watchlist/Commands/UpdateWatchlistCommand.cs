using ApplicationLayer.DTOs.Watchlist;
using MediatR;

namespace ApplicationLayer.Features.Watchlist.Commands;

public record UpdateWatchlistCommand(int Id, UpdateWatchlistDto Model) : IRequest<HandlerResult>;