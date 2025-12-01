using ApplicationLayer.Extensions;
using ApplicationLayer.Features.Channels.Commands;
using ApplicationLayer.Interfaces.Channels;
using MediatR;

namespace ApplicationLayer.Features.Channels.Handler;

public class RateChannelHandler(IChannelService _service) : IRequestHandler<RateChannelCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(RateChannelCommand request, CancellationToken cancellationToken)
    {
        var result = await _service.RateAsync(request.Model);
        return result.ToHandlerResult();
    }
}