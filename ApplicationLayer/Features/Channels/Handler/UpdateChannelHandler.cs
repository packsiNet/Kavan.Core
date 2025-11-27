using ApplicationLayer.Extensions;
using ApplicationLayer.Features.Channels.Commands;
using ApplicationLayer.Interfaces.Channels;
using MediatR;

namespace ApplicationLayer.Features.Channels.Handler;

public class UpdateChannelHandler(IChannelService _service) : IRequestHandler<UpdateChannelCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(UpdateChannelCommand request, CancellationToken cancellationToken)
    {
        var result = await _service.UpdateAsync(request.Id, request.Model);
        return result.ToHandlerResult();
    }
}