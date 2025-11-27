using ApplicationLayer.Extensions;
using ApplicationLayer.Features.Watchlist.Commands;
using ApplicationLayer.Interfaces.Watchlist;
using MediatR;

namespace ApplicationLayer.Features.Watchlist.Handler;

public class RemoveWatchlistItemHandler(IWatchlistService _service) : IRequestHandler<RemoveWatchlistItemCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(RemoveWatchlistItemCommand request, CancellationToken cancellationToken)
    {
        var result = await _service.RemoveItemAsync(request.ItemId);
        return result.ToHandlerResult();
    }
}