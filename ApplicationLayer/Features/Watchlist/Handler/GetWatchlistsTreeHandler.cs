using ApplicationLayer.Extensions;
using ApplicationLayer.Features.Watchlist.Query;
using ApplicationLayer.Interfaces.Watchlist;
using MediatR;

namespace ApplicationLayer.Features.Watchlist.Handler;

public class GetWatchlistsTreeHandler(IWatchlistService _service) : IRequestHandler<GetWatchlistsTreeQuery, HandlerResult>
{
    public async Task<HandlerResult> Handle(GetWatchlistsTreeQuery request, CancellationToken cancellationToken)
    {
        var result = await _service.GetListsAsync();
        return result.ToHandlerResult();
    }
}