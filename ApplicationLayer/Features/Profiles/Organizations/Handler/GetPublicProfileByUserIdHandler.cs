using ApplicationLayer.Extensions;
using ApplicationLayer.Features.Profiles.Organizations.Query;
using ApplicationLayer.Interfaces.Profiles;
using MediatR;

namespace ApplicationLayer.Features.Profiles.Organizations.Handler;

public class GetPublicProfileByUserIdHandler(IOrganizationProfileService _service) : IRequestHandler<GetPublicProfileByUserIdQuery, HandlerResult>
{
    public async Task<HandlerResult> Handle(GetPublicProfileByUserIdQuery request, CancellationToken cancellationToken)
    {
        var result = await _service.GetPublicByUserIdAsync(request.UserId);
        return result.ToHandlerResult();
    }
}