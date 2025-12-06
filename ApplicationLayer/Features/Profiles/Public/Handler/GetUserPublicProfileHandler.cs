using ApplicationLayer.Extensions;
using ApplicationLayer.Features.Profiles.Public.Query;
using ApplicationLayer.Interfaces.Profiles;
using MediatR;

namespace ApplicationLayer.Features.Profiles.Public.Handler;

public class GetUserPublicProfileHandler(IPublicProfileService _service) : IRequestHandler<GetUserPublicProfileQuery, HandlerResult>
{
    public async Task<HandlerResult> Handle(GetUserPublicProfileQuery request, CancellationToken cancellationToken)
    {
        var result = await _service.GetByUserIdAsync(request.UserId);
        return result.ToHandlerResult();
    }
}
