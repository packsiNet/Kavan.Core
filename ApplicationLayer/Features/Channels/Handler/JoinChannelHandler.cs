using ApplicationLayer.Extensions;
using ApplicationLayer.Features.Channels.Commands;
using ApplicationLayer.Interfaces.Channels;
using MediatR;

namespace ApplicationLayer.Features.Channels.Handler;

public class JoinChannelHandler(IChannelService _service) : IRequestHandler<JoinChannelCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(JoinChannelCommand request, CancellationToken cancellationToken)
    {
        var result = await _service.JoinAsync(request.ChannelId);
        return result.ToHandlerResult();
    }
}