using ApplicationLayer.Extensions;
using ApplicationLayer.Features.Channels.Query;
using ApplicationLayer.Interfaces.Channels;
using MediatR;

namespace ApplicationLayer.Features.Channels.Handler;

public class GetMyChannelsHandler(IChannelService _service) : IRequestHandler<GetMyChannelsQuery, HandlerResult>
{
    public async Task<HandlerResult> Handle(GetMyChannelsQuery request, CancellationToken cancellationToken)
    {
        var result = await _service.GetMyAsync(request.Model);
        return result.ToHandlerResult();
    }
}