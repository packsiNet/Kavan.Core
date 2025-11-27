using ApplicationLayer.Extensions;
using ApplicationLayer.Features.Channels.Query;
using ApplicationLayer.Interfaces.Channels;
using MediatR;

namespace ApplicationLayer.Features.Channels.Handler;

public class GetChannelByIdHandler(IChannelService _service) : IRequestHandler<GetChannelByIdQuery, HandlerResult>
{
    public async Task<HandlerResult> Handle(GetChannelByIdQuery request, CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(request.Id);
        return result.ToHandlerResult();
    }
}