using ApplicationLayer.Extensions;
using ApplicationLayer.Features.ChannelPosts.Commands;
using ApplicationLayer.Interfaces.Channels;
using MediatR;

namespace ApplicationLayer.Features.ChannelPosts.Handler;

public class ReactPostHandler(IChannelPostService _service) : IRequestHandler<ReactPostCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(ReactPostCommand request, CancellationToken cancellationToken)
    {
        var result = await _service.ReactAsync(request.Model);
        return result.ToHandlerResult();
    }
}