using ApplicationLayer.Extensions;
using ApplicationLayer.Features.Channels.Commands;
using ApplicationLayer.Interfaces.Channels;
using MediatR;

namespace ApplicationLayer.Features.Channels.Handler;

public class DeleteChannelHandler(IChannelService _service) : IRequestHandler<DeleteChannelCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(DeleteChannelCommand request, CancellationToken cancellationToken)
    {
        var result = await _service.DeleteAsync(request.Id);
        return result.ToHandlerResult();
    }
}