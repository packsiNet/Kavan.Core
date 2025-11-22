using ApplicationLayer.Extensions;
using ApplicationLayer.Features.Profiles.Organizations.Query;
using ApplicationLayer.Interfaces.Profiles;
using MediatR;

namespace ApplicationLayer.Features.Profiles.Organizations.Handler;

public class GetMyOrganizationProfileHandler(IOrganizationProfileService _service) : IRequestHandler<GetMyOrganizationProfileQuery, HandlerResult>
{
    public async Task<HandlerResult> Handle(GetMyOrganizationProfileQuery request, CancellationToken cancellationToken)
    {
        var result = await _service.GetMineAsync();
        return result.ToHandlerResult();
    }
}