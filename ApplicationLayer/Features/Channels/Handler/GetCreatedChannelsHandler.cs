using ApplicationLayer.Extensions;
using ApplicationLayer.Features.Channels.Query;
using ApplicationLayer.Interfaces.Channels;
using MediatR;

namespace ApplicationLayer.Features.Channels.Handler;

public class GetCreatedChannelsHandler(IChannelService _service) : IRequestHandler<GetCreatedChannelsQuery, HandlerResult>
{
    public async Task<HandlerResult> Handle(GetCreatedChannelsQuery request, CancellationToken cancellationToken)
    {
        var result = await _service.GetCreatedAsync(request.Model);
        return result.ToHandlerResult();
    }
}
