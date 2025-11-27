using ApplicationLayer.Extensions;
using ApplicationLayer.Features.ChannelPosts.Commands;
using ApplicationLayer.Interfaces.Channels;
using MediatR;

namespace ApplicationLayer.Features.ChannelPosts.Handler;

public class UpdateNewsPostHandler(IChannelPostService _service) : IRequestHandler<UpdateNewsPostCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(UpdateNewsPostCommand request, CancellationToken cancellationToken)
    {
        var result = await _service.UpdateNewsAsync(request.Id, request.Model);
        return result.ToHandlerResult();
    }
}