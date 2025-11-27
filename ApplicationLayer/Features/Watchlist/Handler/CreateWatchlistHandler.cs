using ApplicationLayer.Extensions;
using ApplicationLayer.Features.Watchlist.Commands;
using ApplicationLayer.Interfaces.Watchlist;
using MediatR;

namespace ApplicationLayer.Features.Watchlist.Handler;

public class CreateWatchlistHandler(IWatchlistService _service) : IRequestHandler<CreateWatchlistCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(CreateWatchlistCommand request, CancellationToken cancellationToken)
    {
        var result = await _service.CreateListAsync(request.Model);
        return result.ToHandlerResult();
    }
}