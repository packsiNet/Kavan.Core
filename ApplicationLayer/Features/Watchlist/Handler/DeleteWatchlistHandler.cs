using ApplicationLayer.Extensions;
using ApplicationLayer.Features.Watchlist.Commands;
using ApplicationLayer.Interfaces.Watchlist;
using MediatR;

namespace ApplicationLayer.Features.Watchlist.Handler;

public class DeleteWatchlistHandler(IWatchlistService _service) : IRequestHandler<DeleteWatchlistCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(DeleteWatchlistCommand request, CancellationToken cancellationToken)
    {
        var result = await _service.DeleteListAsync(request.Id);
        return result.ToHandlerResult();
    }
}