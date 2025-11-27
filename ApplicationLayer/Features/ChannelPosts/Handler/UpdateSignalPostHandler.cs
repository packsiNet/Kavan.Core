using ApplicationLayer.Extensions;
using ApplicationLayer.Features.ChannelPosts.Commands;
using ApplicationLayer.Interfaces.Channels;
using MediatR;

namespace ApplicationLayer.Features.ChannelPosts.Handler;

public class UpdateSignalPostHandler(IChannelPostService _service) : IRequestHandler<UpdateSignalPostCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(UpdateSignalPostCommand request, CancellationToken cancellationToken)
    {
        var result = await _service.UpdateSignalAsync(request.Id, request.Model);
        return result.ToHandlerResult();
    }
}