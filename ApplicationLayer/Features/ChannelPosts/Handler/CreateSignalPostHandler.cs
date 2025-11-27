using ApplicationLayer.Extensions;
using ApplicationLayer.Features.ChannelPosts.Commands;
using ApplicationLayer.Interfaces.Channels;
using MediatR;

namespace ApplicationLayer.Features.ChannelPosts.Handler;

public class CreateSignalPostHandler(IChannelPostService _service) : IRequestHandler<CreateSignalPostCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(CreateSignalPostCommand request, CancellationToken cancellationToken)
    {
        var result = await _service.CreateSignalAsync(request.Model);
        return result.ToHandlerResult();
    }
}