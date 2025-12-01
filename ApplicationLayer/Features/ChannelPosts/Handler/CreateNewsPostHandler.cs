using ApplicationLayer.Extensions;
using ApplicationLayer.Features.ChannelPosts.Commands;
using ApplicationLayer.Interfaces.Channels;
using MediatR;

namespace ApplicationLayer.Features.ChannelPosts.Handler;

public class CreateNewsPostHandler(IChannelPostService _service) : IRequestHandler<CreateNewsPostCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(CreateNewsPostCommand request, CancellationToken cancellationToken)
    {
        var result = await _service.CreateNewsAsync(request.Model);
        return result.ToHandlerResult();
    }
}