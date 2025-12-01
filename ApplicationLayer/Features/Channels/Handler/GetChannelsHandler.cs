using ApplicationLayer.Extensions;
using ApplicationLayer.Features.Channels.Query;
using ApplicationLayer.Interfaces.Channels;
using MediatR;

namespace ApplicationLayer.Features.Channels.Handler;

public class GetChannelsHandler(IChannelService _service) : IRequestHandler<GetChannelsQuery, HandlerResult>
{
    public async Task<HandlerResult> Handle(GetChannelsQuery request, CancellationToken cancellationToken)
    {
        var result = await _service.GetPublicAsync(request.Model);
        return result.ToHandlerResult();
    }
}