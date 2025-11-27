using ApplicationLayer.Extensions;
using ApplicationLayer.Features.ChannelPosts.Commands;
using ApplicationLayer.Interfaces.Channels;
using MediatR;

namespace ApplicationLayer.Features.ChannelPosts.Handler;

public class DeletePostHandler(IChannelPostService _service) : IRequestHandler<DeletePostCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(DeletePostCommand request, CancellationToken cancellationToken)
    {
        var result = await _service.DeleteAsync(request.Id);
        return result.ToHandlerResult();
    }
}