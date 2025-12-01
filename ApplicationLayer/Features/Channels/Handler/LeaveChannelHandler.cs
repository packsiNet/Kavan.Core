using ApplicationLayer.Extensions;
using ApplicationLayer.Features.Channels.Commands;
using ApplicationLayer.Interfaces.Channels;
using MediatR;

namespace ApplicationLayer.Features.Channels.Handler;

public class LeaveChannelHandler(IChannelService _service) : IRequestHandler<LeaveChannelCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(LeaveChannelCommand request, CancellationToken cancellationToken)
    {
        var result = await _service.LeaveAsync(request.ChannelId);
        return result.ToHandlerResult();
    }
}