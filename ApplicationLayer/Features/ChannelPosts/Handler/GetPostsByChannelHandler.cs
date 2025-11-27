using ApplicationLayer.Extensions;
using ApplicationLayer.Features.ChannelPosts.Query;
using ApplicationLayer.Interfaces.Channels;
using MediatR;

namespace ApplicationLayer.Features.ChannelPosts.Handler;

public class GetPostsByChannelHandler(IChannelPostService _service) : IRequestHandler<GetPostsByChannelQuery, HandlerResult>
{
    public async Task<HandlerResult> Handle(GetPostsByChannelQuery request, CancellationToken cancellationToken)
    {
        var result = await _service.GetByChannelAsync(request.ChannelId, request.Model);
        return result.ToHandlerResult();
    }
}