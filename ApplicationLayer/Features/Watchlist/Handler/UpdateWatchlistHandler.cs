using ApplicationLayer.Extensions;
using ApplicationLayer.Features.Watchlist.Commands;
using ApplicationLayer.Interfaces.Watchlist;
using MediatR;

namespace ApplicationLayer.Features.Watchlist.Handler;

public class UpdateWatchlistHandler(IWatchlistService _service) : IRequestHandler<UpdateWatchlistCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(UpdateWatchlistCommand request, CancellationToken cancellationToken)
    {
        var result = await _service.UpdateListAsync(request.Id, request.Model);
        return result.ToHandlerResult();
    }
}