using ApplicationLayer.Extensions;
using ApplicationLayer.Features.Channels.Commands;
using ApplicationLayer.Interfaces.Channels;
using MediatR;

namespace ApplicationLayer.Features.Channels.Handler;

public class MuteChannelHandler(IChannelService _service) : IRequestHandler<MuteChannelCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(MuteChannelCommand request, CancellationToken cancellationToken)
    {
        var result = await _service.MuteAsync(request.ChannelId);
        return result.ToHandlerResult();
    }
}
