using ApplicationLayer.Extensions;
using ApplicationLayer.Features.Channels.Commands;
using ApplicationLayer.Interfaces.Channels;
using MediatR;

namespace ApplicationLayer.Features.Channels.Handler;

public class CreateChannelHandler(IChannelService _service) : IRequestHandler<CreateChannelCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(CreateChannelCommand request, CancellationToken cancellationToken)
    {
        var result = await _service.CreateAsync(request.Model);
        return result.ToHandlerResult();
    }
}