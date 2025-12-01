using ApplicationLayer.Extensions;
using ApplicationLayer.Features.Watchlist.Commands;
using ApplicationLayer.Interfaces.Watchlist;
using MediatR;

namespace ApplicationLayer.Features.Watchlist.Handler;

public class AddWatchlistItemHandler(IWatchlistService _service) : IRequestHandler<AddWatchlistItemCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(AddWatchlistItemCommand request, CancellationToken cancellationToken)
    {
        var result = await _service.AddItemAsync(request.WatchlistId, request.Symbol);
        return result.ToHandlerResult();
    }
}